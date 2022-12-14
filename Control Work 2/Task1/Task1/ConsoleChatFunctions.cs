namespace ConsoleChat;

using System.Net.Sockets;

/// <summary>
/// Realization of general functions as message sending or message receiving or stop chating function
/// </summary>
public class ConsoleChatFunctions
{
    protected CancellationTokenSource cancellationTokenSource = new();

    /// <summary>
    /// Message sending realization
    /// </summary>
    protected void Writer(NetworkStream stream)
    {
        Task.Run(async () =>
        {
            var streamWriter = new StreamWriter(stream) { AutoFlush = true };
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                var messageToSend = Console.ReadLine();
                await streamWriter.WriteLineAsync(messageToSend);
            }

            await streamWriter.WriteLineAsync("exit");
            streamWriter.Close();
        });
    }

    /// <summary>
    /// Message receiving realization
    /// </summary>
    protected void Reader(NetworkStream stream)
    {
        Task.Run(async () =>
        {
            var streamReader = new StreamReader(stream);
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                var receivedMessage = await streamReader.ReadLineAsync();
                if (receivedMessage == "exit")
                {
                    StopChating();
                    break;
                }
                Console.WriteLine(receivedMessage);
            }

            streamReader.Close();
        });
    }

    /// <summary>
    /// Stops chating
    /// </summary>
    public void StopChating()
    {
        cancellationTokenSource.Cancel();
    }
}