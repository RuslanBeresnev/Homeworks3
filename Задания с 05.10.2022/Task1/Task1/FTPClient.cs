namespace SimpleFTP;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Realization of FTP-client
/// </summary>
public class FTPClient
{
    private int port;
    private IPAddress ip;

    public FTPClient(int port, IPAddress ip)
    {
        this.port = port;
        this.ip = ip;

        StartRequestsAccepting();
    }

    /// <summary>
    /// Start accepting requests from user
    /// </summary>
    public void StartRequestsAccepting()
    {
        while (true)
        {
            var request = Console.ReadLine();
            // pass
        }
    }
}