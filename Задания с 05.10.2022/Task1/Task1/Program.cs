namespace SimpleFTP;

public class Program
{
    /// <summary>
    /// args[0] - host
    /// args[1] - port
    /// args[2] - command name
    /// args[3] - path to file or directory
    /// </summary>
    static async void Main(string[] args)
    {
        FTPServer server = new(args[0], Int32.Parse(args[1]));
        FTPClient client = new(args[0], Int32.Parse(args[1]));

        server.Start();

        if (args[2] == "List")
        {
            var directoryInfo = await client.List(args[3]);
            Console.WriteLine("Information about directory:");
            Console.WriteLine();
            Console.WriteLine(directoryInfo);
        }
        else if (args[2] == "Get")
        {
            var file = await client.Get(args[3]);
            var fileContent = "";

            for (int i = 0; i < file.Length; i++)
            {
                fileContent += (char)file[i];
            }

            Console.WriteLine("Content in file:");
            Console.WriteLine();
            Console.WriteLine(fileContent);
        }

        server.Stop();
    }
}