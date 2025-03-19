namespace App;

using RabbitMQ.Client;
using Domain;

public class App
{
    public async static Task Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: dotnet run <consumer-name> <event type 1-4>");
            return;
        }

        string name = args[0];

        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        IWorker? worker = GetWorker(int.Parse(args[1]), name, channel);

        if (worker == null) {
            Console.WriteLine("Invalid event type");
            return;
        }

        await worker.Run();
    }

    private static IWorker? GetWorker(int type, string name, IChannel channel) {
        switch (type) {
            case 1:
                return new Consumer<Typ1Event>(name, channel);
            case 2:
                return new Consumer<Typ2Event>(name, channel);
            case 3:
                return new Consumer<Typ3Event>(name, channel);
            case 4:
                return new Consumer<Typ4Event>(name, channel);
            default:
                return null;
        }
    }
}
