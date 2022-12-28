namespace MD5;

using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Multi-threading check-sum realization
/// </summary>
public static class MultiMD5
{
    private static MD5 md5 = MD5.Create();

    /// <summary>
    /// Calculate directory hash (Multu-threading realization)
    /// </summary>
    /// <param name="path">Path to directory</param>
    public static byte[] ComputeDirectoryHash(string path)
    {
        var directoryName = path.Split("/")[path.Split("/").Length - 1];
        var directoryNameInBytes = Encoding.UTF8.GetBytes(directoryName);

        var subDirectories = Directory.GetDirectories(path);
        var filesInDirectory = Directory.GetFiles(path);

        var threadsCount = subDirectories.Length + filesInDirectory.Length;
        var threads = new Thread[threadsCount];

        var counter = 0;
        var filesAndDirectoriesInBytes = new List<byte[]>();
        foreach (var fileName in filesInDirectory)
        {
            threads[counter] = new Thread(() =>
            {
                filesAndDirectoriesInBytes.Add(ComputeFileHash(fileName));
            });
            counter++;
        }
        foreach (var dirName in subDirectories)
        {
            threads[counter] = new Thread(() =>
            {
                filesAndDirectoriesInBytes.Add(ComputeDirectoryHash(dirName));
            });
            counter++;
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
        foreach (var thread in threads)
        {
            thread.Join();
        }

        byte[] checkSum = directoryNameInBytes;
        for (int i = 0; i < threadsCount; i++)
        {
            checkSum = checkSum.Concat<byte>(filesAndDirectoriesInBytes[i]).ToArray<byte>();
        }

        return md5.ComputeHash(checkSum);
    }

    /// <summary>
    /// Calculate file hash
    /// </summary>
    /// <param name="path">Path to file</param>
    /// <returns></returns>
    public static byte[] ComputeFileHash(string path)
    {
        var file = File.ReadAllBytes(path);
        return md5.ComputeHash(file);
    }
}