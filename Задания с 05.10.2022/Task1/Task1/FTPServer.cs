namespace SimpleFTP;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// FTP-Server realization
/// </summary>
public class FTPServer
{
    private TcpListener listener;
    private CancellationTokenSource cancellationTokenSource = new();

    public FTPServer(string host, int port)
    {
        listener = new TcpListener(IPAddress.Parse(host), port);
    }

    /// <summary>
    /// Starts the server
    /// </summary>
    public async void Start()
    {
        listener.Start();
        while (!cancellationTokenSource.Token.IsCancellationRequested)
        {
            var client = await listener.AcceptTcpClientAsync();
            await Task.Run(() => WorkProcess(client));
        }
        listener.Stop();
    }

    /// <summary>
    /// Data parsing to command and path to file
    /// </summary>
    private (string, string) ParseData(string data)
    {
        return (data.Split()[0], data.Split()[1]);
    }

    /// <summary>
    /// Server work process
    /// </summary>
    private async Task WorkProcess(TcpClient client)
    {
        var stream = client.GetStream();
        var streamReader = new StreamReader(stream);
        var streamWriter = new StreamWriter(stream) { AutoFlush = true };

        var data = await streamReader.ReadLineAsync();
        if (data == null)
        {
            throw new ArgumentException("Request to server cannot be null");
        }
        var (command, path) = ParseData(data);

        switch (command)
        {
            case "1":
                await List(path, streamWriter);
                break;

            case "2":
                await Get(path, streamWriter);
                break;

            default:
                throw new ArgumentException("Command not exists");
        }

        streamReader.Close();
        streamWriter.Close();
    }

    /// <summary>
    /// List command realization
    /// </summary>
    private async Task List(string path, StreamWriter streamWriter)
    {
        if (!Directory.Exists(path))
        {
            await streamWriter.WriteLineAsync("-1");
            return;
        }

        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);
        var response = (directories.Length + files.Length).ToString();

        foreach (var fileName in files)
        {
            response += " " + fileName + " false";
        }

        foreach (var directoryName in directories)
        {
            response += " " + directoryName + " true";
        }

        await streamWriter.WriteLineAsync(response);
    }

    /// <summary>
    /// Get command realization
    /// </summary>
    private async Task Get(string path, StreamWriter writer)
    {
        if (!File.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            return;
        }

        await writer.WriteAsync($"{new FileInfo(path).Length}");
        var file = await File.ReadAllBytesAsync(path, cancellationTokenSource.Token);
        foreach (var oneByte in file)
        {
            await writer.WriteAsync(" " + oneByte.ToString());
        }
        await writer.WriteLineAsync();
        await writer.FlushAsync();
    }

    /// <summary>
    /// Stops the server
    /// </summary>
    public void Stop()
    {
        cancellationTokenSource.Cancel();
    }
}