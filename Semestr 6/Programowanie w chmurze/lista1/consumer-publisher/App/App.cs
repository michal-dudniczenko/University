namespace App;

using RabbitMQ.Client;
using Domain;

public class App
{
    public async static Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnet run <consumer-publisher-name>");
            return;
        }

        string name = args[0];

        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        ConsumerPublisher<Typ3Event, Typ4Event> worker = new(name, channel);

        await worker.Run();
    }
}
