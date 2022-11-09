namespace SimpleFTP;

using System.Net.Sockets;

/// <summary>
/// Realization of FTP-client
/// </summary>
public static class FTPClient
{
    /// <summary>
    /// Start accepting requests from user
    /// </summary>
    public static void StartClient(int port)
    {
        using (var client = new TcpClient("localhost", port))
        {
            var stream = client.GetStream();
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            while (true)
            {
                var request = Console.ReadLine();
                if (request == null)
                {
                    continue;
                }
                if (request.Split(" ")[0] == "1")
                {
                    List(request, writer);
                }
                else if (request.Split(" ")[0] == "2")
                {
                    Get(request, writer);
                }
                else
                {
                    continue;
                }
            }
        }
    }

    /// <summary>
    /// Client's List-request realization
    /// </summary>
    private static void List(string request, StreamWriter writer)
    {
        writer.WriteLine(request);

        // pass
    }

    /// <summary>
    /// Client's Get-request realization
    /// </summary>
    private static void Get(string request, StreamWriter writer)
    {
        writer.WriteLine(request);

        // pass
    }
}