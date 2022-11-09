namespace SimpleFTP;

using System.Net;
using System.Net.Sockets;
using System.IO;

/// <summary>
/// FTP-server realization
/// </summary>
public static class FTPServer
{
    public static async Task StartServer(IPAddress ip, int port)
    {
        var listener = new TcpListener(ip, port);
        listener.Start();

        while (true)
        {
            var socket = await listener.AcceptSocketAsync();

            Task.Run(async () =>
            {
                var stream = new NetworkStream(socket);
                var reader = new StreamReader(stream);
                var data = await reader.ReadLineAsync();

                var writer = new StreamWriter(stream);
                writer.AutoFlush = true;

                int commandNumber = int.Parse(data.Split(" ")[0]);
                string path = data.Split(" ")[1];
                if (commandNumber == 1)
                {
                    var response = PerformListRequest(path);
                    // pass
                }
                else if (commandNumber == 2)
                {
                    (long size, byte[]? bytes) = await PerformGetRequest(path);
                    // pass
                }

                socket.Close();
            });
        }
    }

    /// <summary>
    /// Server's directory enumeration realization
    /// </summary>
    private static string PerformListRequest(string path)
    {
        if (!Directory.Exists(path))
        {
            return "-1";
        }

        var response = "";
        var directories = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);
        response += (directories.Length + files.Length).ToString();

        foreach (var directory in directories)
        {
            response += " " + path + "/" + directory.Split("/")[directory.Split("/").Length - 1] + " true";
        }

        foreach (var file in files)
        {
            response += " " + path + "/" + file.Split("/")[file.Split("/").Length - 1] + " false";
        }

        return response;
    }

    /// <summary>
    /// Get file from server's directory
    /// </summary>
    private static async Task<(long, byte[]?)> PerformGetRequest(string path)
    {
        if (!File.Exists(path))
        {
            return (-1, null);
        }

        // Исправить !!!

        var fileBytes = await File.ReadAllBytesAsync(path);
        var size = fileBytes.Length;

        return (size, fileBytes);
    }
}