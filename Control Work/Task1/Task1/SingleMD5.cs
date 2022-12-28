namespace MD5;

using System.Security.Cryptography;
using System.Text;
using System.Linq;

/// <summary>
/// Single-threading check-sum realization
/// </summary>
public static class SingleMD5
{
    private static MD5 md5 = MD5.Create();

    /// <summary>
    /// Calculate directory hash (Single-threading realization)
    /// </summary>
    /// <param name="path">Path to directory</param>
    public static byte[] ComputeDirectoryHash(string path)
    {
        var directoryName = path.Split("/")[path.Split("/").Length - 1];
        var directoryNameInBytes = Encoding.UTF8.GetBytes(directoryName);

        var subDirectories = Directory.GetDirectories(path);
        var filesInDirectory = Directory.GetFiles(path);

        byte[] checkSum = directoryNameInBytes;
        foreach (var fileName in filesInDirectory)
        {
            checkSum = checkSum.Concat<byte>(ComputeFileHash(fileName)).ToArray<byte>();
        }
        foreach (var dirName in subDirectories)
        {
            checkSum = checkSum.Concat<byte>(ComputeDirectoryHash(dirName)).ToArray<byte>();
        }

        return md5.ComputeHash(checkSum);
    }

    /// <summary>
    /// Calculate file hash (Single-threading realization)
    /// </summary>
    /// <param name="path">Path to file</param>
    /// <returns></returns>
    public static byte[] ComputeFileHash(string path)
    {
        var file = File.ReadAllBytes(path);
        return md5.ComputeHash(file);
    }
}