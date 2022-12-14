namespace ConsoleChat;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Console chat server realization
/// </summary>
public class Server : ConsoleChatFunctions
{
    private TcpListener listener;

    public Server(int port)
    {
        listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
    }

    /// <summary>
    /// Starts the server
    /// </summary>
    public async void Start()
    {
        listener.Start();
        Console.WriteLine("Server starts. Getting connection with client ...");
        while (!cancellationTokenSource.Token.IsCancellationRequested)
        {
            var client = await listener.AcceptTcpClientAsync();
            Console.WriteLine("Connection established");
            Writer(client.GetStream());
            Reader(client.GetStream());
        }
        listener.Stop();
    }
}