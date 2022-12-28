namespace MD5;

using System.Diagnostics;
using System.Linq;

public class Program
{
    static void Main(string[] args)
    {
        const int LAUNCHES_COUNT = 100;
        var singleMD5WorkingTimes = new float[100];
        var multiMD5WorkingTimes = new float[100];
        var time = new Stopwatch();

        for (int i = 0; i < LAUNCHES_COUNT; i++)
        {
            time.Restart();
            SingleMD5.ComputeDirectoryHash("../../../TestDirectory");
            time.Stop();
            singleMD5WorkingTimes[i] = (float)time.Elapsed.TotalMilliseconds;

            time.Restart();
            MultiMD5.ComputeDirectoryHash("../../../TestDirectory");
            time.Stop();
            multiMD5WorkingTimes[i] = (float)time.Elapsed.TotalMilliseconds;
        }

        Console.WriteLine($"Матожидание времени работы для однопоточного MD5 на директории TestDirectory (в миллисекундах): " +
            $"{singleMD5WorkingTimes.Average()}");
        Console.WriteLine($"Матожидание времени работы для многопоточного MD5 на директории TestDirectory (в миллисекундах): " +
            $"{multiMD5WorkingTimes.Average()}");
    }
}