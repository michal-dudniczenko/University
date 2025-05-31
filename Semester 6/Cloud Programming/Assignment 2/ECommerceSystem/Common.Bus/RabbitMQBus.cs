using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Common.Domain.Bus;
using Common.Domain.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Common.Domain;

namespace Common.Bus;

public sealed class RabbitMQBus : IEventBus
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RabbitMQBus(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Publish<T>(T @event) where T : Event
    {
        var factory = new ConnectionFactory() { 
            HostName = Domain.Constants.RABBITMQ_IP,
            UserName = Domain.Constants.RABBITMQ_USERNAME,
            Password = Domain.Constants.RABBITMQ_PASSWORD,
            VirtualHost = Domain.Constants.RABBITMQ_VIRTUAL_HOST,
        };
        using (var connection = await factory.CreateConnectionAsync())
        using (var channel = await connection.CreateChannelAsync())
        {
            var eventName = @event.GetType().Name;
            var exchangeName = eventName;

            // create fanout exchange with name equal to event name
            await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Fanout, durable: false, autoDelete: false);

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            // publish message to that fanout exchange
            await channel.BasicPublishAsync(exchange: exchangeName, routingKey: "", body: body);

        }

    }

    public async Task Subscribe<T, TH, TS>()
    where T : Event
    where TH : IEventHandler<T>
    where TS : Service
    {
        var factory = new ConnectionFactory()
        {
            HostName = Domain.Constants.RABBITMQ_IP,
            UserName = Domain.Constants.RABBITMQ_USERNAME,
            Password = Domain.Constants.RABBITMQ_PASSWORD,
            VirtualHost = Domain.Constants.RABBITMQ_VIRTUAL_HOST,
        };
        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        var exchangeName = typeof(T).Name;
        var queueName = $"{typeof(TS).Name}-{typeof(TH).Name}";

        await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Fanout, durable: false, autoDelete: false);
        await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: true);
        await channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: "");

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            var message = Encoding.UTF8.GetString(args.Body.ToArray());
            T @event;

            try
            {
                @event = JsonConvert.DeserializeObject<T>(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to deserialize message: {ex.Message}");
                return;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetService<TH>();

            if (handler == null)
            {
                Console.WriteLine($"Handler not found for {typeof(T).Name}");
                return;
            }

            try
            {
                await handler.Handle(@event);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling event {typeof(T).Name}: {ex.Message}");
            }
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
    }

}
