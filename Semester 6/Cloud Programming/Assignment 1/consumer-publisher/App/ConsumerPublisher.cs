using System.Text;
using Domain;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;

namespace App;

class ConsumerPublisher<ConsumeType, PublishType>(string name, IChannel channel) where ConsumeType : Event where PublishType : Event
{
    private string Name { get; set; } = name;
    private IChannel Channel { get; set; } = channel;
    private int counter = 0;

    public async Task Run()
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger logger = factory.CreateLogger("Consumer-Publisher");

        var consumeEventType = typeof(ConsumeType);
        var publishEventType = typeof(PublishType);

        await Channel.QueueDeclareAsync(queue: consumeEventType.Name, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await Channel.QueueDeclareAsync(queue: publishEventType.Name, durable: false, exclusive: false, autoDelete: false, arguments: null);
        
        await Channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(Channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            // receiving event
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            ConsumeType? eventReceived = JsonConvert.DeserializeObject<ConsumeType>(message);

            logger.LogInformation($"{Name} Received {eventReceived}  at: {DateTime.Now.ToString("HH:mm:ss")}");

            // working on received event
            await Task.Delay(5000);
            await Channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

            // publishing event
            PublishType? eventToSend = Activator.CreateInstance(publishEventType, Name, counter) as PublishType;
            counter++;
            var publishBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventToSend));
            await Channel.BasicPublishAsync(exchange: string.Empty, routingKey: publishEventType.Name, body: publishBody);
            logger.LogInformation($"{Name} Sent {eventToSend} at: {DateTime.Now.ToString("HH:mm:ss")}");
        };

        await Channel.BasicConsumeAsync(consumeEventType.Name, autoAck: false, consumer: consumer);
        Console.ReadLine();
    }
}
