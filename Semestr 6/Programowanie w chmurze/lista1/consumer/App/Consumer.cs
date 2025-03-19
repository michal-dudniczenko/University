using System.Text;
using Domain;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;

namespace App;

class Consumer<EventType>(string name, IChannel channel) : IWorker where EventType : Event
{
    private string Name { get; set; } = name;
    private IChannel Channel { get; set; } = channel;

    public async Task Run()
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger logger = factory.CreateLogger("Consumer");

        var eventType = typeof(EventType);
        await Channel.QueueDeclareAsync(queue: eventType.Name, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await Channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(Channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            EventType? eventReceived = JsonConvert.DeserializeObject<EventType>(message);
            logger.LogInformation($"{Name} Received {eventReceived} at: {DateTime.Now.ToString("HH:mm:ss")}");
            
            await Task.Delay(5000);
            await Channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
        };

        await Channel.BasicConsumeAsync(eventType.Name, autoAck: false, consumer: consumer);
        Console.ReadLine();
    }
}
