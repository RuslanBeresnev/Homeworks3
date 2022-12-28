namespace ConsoleChat;

/// <summary>
/// Starts server or client
/// </summary>
public class Program
{
    static async void Main(string[] args)
    {
        if (args.Length == 1)
        {
            var server = new Server(Int32.Parse(args[0]));
            server.Start();
        }
        else if (args.Length == 2)
        {
            var client = new Client(args[0], Int32.Parse(args[1]));
            await client.Connect();
        }
    }
}