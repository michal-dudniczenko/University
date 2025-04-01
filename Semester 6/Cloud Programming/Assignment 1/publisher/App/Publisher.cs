using System.Text;
using Domain;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;

namespace App;

class Publisher<EventType>(string name, IChannel channel, bool isRandomDelay = false) : IWorker where EventType : Event
{
    private string Name { get; set; } = name;
    private IChannel Channel { get; set; } = channel;
    private int counter = 0;
    private bool IsRandomDelay { get; set; } = isRandomDelay;

    public async Task Run()
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger logger = factory.CreateLogger("Publisher");

        var eventType = typeof(EventType);
        await Channel.QueueDeclareAsync(queue: eventType.Name, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await Channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

        while (true)
        {
            EventType? eventToSend = Activator.CreateInstance(eventType, Name, counter) as EventType;
            counter++;
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventToSend));
            await Channel.BasicPublishAsync(exchange: string.Empty, routingKey: eventType.Name, body: body);
            logger.LogInformation($"{Name} Sent {eventToSend} at: {DateTime.Now.ToString("HH:mm:ss")}");

            if (IsRandomDelay)
            {
                int randomDelay = new Random().Next(1000, 5000);
                await Task.Delay(randomDelay);
            } else
            {
                await Task.Delay(3000);
            }
        }
    }
}
