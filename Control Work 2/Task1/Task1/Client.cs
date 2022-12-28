namespace ConsoleChat;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Console chat client realization
/// </summary>
public class Client : ConsoleChatFunctions
{
    private IPAddress ip;
    private int port;

    public Client(string host, int port)
    {
        ip = IPAddress.Parse(host);
        this.port = port;
    }

    /// <summary>
    /// Set connection between port on server and client
    /// </summary>
    public async Task Connect()
    {
        Console.WriteLine("Connecting to server ...");
        using (var client = new TcpClient())
        {
            await client.ConnectAsync(ip, port);
            Console.WriteLine("Connection established");
            var stream = client.GetStream();

            Writer(client.GetStream());
            Reader(client.GetStream());
        }
    }
}