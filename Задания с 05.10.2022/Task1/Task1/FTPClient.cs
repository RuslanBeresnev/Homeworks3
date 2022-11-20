namespace SimpleFTP;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// FTP-Client realization
/// </summary>
public class FTPClient
{
    private IPAddress ip;
    private int port;

    public FTPClient(string host, int port)
    {
        ip = IPAddress.Parse(host);
        this.port = port;
    }

    /// <summary>
    /// Requests file downloading from the server
    /// </summary>
    public async Task<string> Get(string path)
    {
        using (var client = new TcpClient())
        {
            await client.ConnectAsync(ip, port);
            var stream = client.GetStream();
            var streamWriter = new StreamWriter(stream);
            var streamReader = new StreamReader(stream);

            await streamWriter.WriteLineAsync("2 " + path);
            await streamWriter.FlushAsync();

            var response = await streamReader.ReadLineAsync();
            if (response!.Split(' ')[0] == "-1")
            {
                throw new FileNotFoundException();
            }

            streamReader.Close();
            streamWriter.Close();
            return response;
        }
    }

    /// <summary>
    /// Requests list of files in server's directory
    /// </summary>
    public async Task<string> List(string path)
    {
        using (var client = new TcpClient())
        {
            await client.ConnectAsync(ip, port);
            var stream = client.GetStream();
            var streamWriter = new StreamWriter(stream);
            var streamReader = new StreamReader(stream);

            await streamWriter.WriteLineAsync("1 " + path);
            await streamWriter.FlushAsync();

            var response = await streamReader.ReadLineAsync();
            if (response!.Split(' ')[0] == "-1")
            {
                throw new DirectoryNotFoundException();
            }

            streamReader.Close();
            streamWriter.Close();
            return response;
        }
    }
}