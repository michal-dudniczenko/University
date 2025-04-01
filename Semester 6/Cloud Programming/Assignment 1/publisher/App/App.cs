namespace App;

using RabbitMQ.Client;
using Domain;

public class App
{
    public async static Task Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: dotnet run <publisher-name> <event type 1-4>");
            return;
        }

        string name = args[0];

        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        IWorker? worker = GetWorker(int.Parse(args[1]), name, channel);

        if (worker == null)
        {
            return;
        }

        await worker.Run();
    }

    private static IWorker? GetWorker(int type, string name, IChannel channel)
    {
        switch (type)
        {
            case 1:
                return new Publisher<Typ1Event>(name, channel);
            case 2:
                return new Publisher<Typ2Event>(name, channel, true);
            case 3:
                return new Publisher<Typ3Event>(name, channel, true);
            case 4:
                return new Publisher<Typ4Event>(name, channel);
            default:
                return null;
        }
    }
}
