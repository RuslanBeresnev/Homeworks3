namespace SimpleFTP;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// FTP-server realization
/// </summary>
public class FTPServer
{
    private int port;
    private IPAddress ip;

    public FTPServer(int port, IPAddress ip)
    {
        this.port = port;
        this.ip = ip;

        StartServer();
    }

    private async void StartServer()
    {
        var listener = new TcpListener(ip, port);
        listener.Start();
        var socket = await listener.AcceptSocketAsync();

        Task.Run(async () =>
        {
            // pass
        });
    }

    /// <summary>
    /// Server's directory enumeration realization
    /// </summary>
    public void PerformListRequest()
    {
        // pass
    }

    /// <summary>
    /// Get file from server's directory
    /// </summary>
    public void PerformGetRequest()
    {
        // pass
    }
}