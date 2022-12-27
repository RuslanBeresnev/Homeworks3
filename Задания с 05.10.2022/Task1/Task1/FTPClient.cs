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
    public async Task<byte[]> Get(string path)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(ip, port);

        using var stream = client.GetStream();
        using var streamWriter = new StreamWriter(stream);
        using var streamReader = new StreamReader(stream);

        await streamWriter.WriteLineAsync("2 " + path);
        await streamWriter.FlushAsync();

        var response = await streamReader.ReadToEndAsync();
        var splittedResponse = response.Split(' ');

        if (splittedResponse[0] == "-1")
        {
            throw new FileNotFoundException();
        }

        var downloadedFile = new byte[Int32.Parse(splittedResponse[0])];
        for (int i = 1; i < downloadedFile.Length + 1; i++)
        {
            downloadedFile[i - 1] = byte.Parse(splittedResponse[i]);
        }

        return downloadedFile;
    }

    /// <summary>
    /// Requests list of files in server's directory
    /// </summary>
    public async Task<string> List(string path)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(ip, port);

        using var stream = client.GetStream();
        using var streamWriter = new StreamWriter(stream);
        using var streamReader = new StreamReader(stream);

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