namespace Task1;

using System.Diagnostics;

/// <summary>
/// Реализация статистики при сравнении работы двух способов умножения матриц
/// </summary>
internal class Statistics
{
    /// <summary>
    /// Посчитать среднеквадратичное отклонение для массива равновероятных величин
    /// </summary>
    private static float CalculateStandardDeviation(float[] values)
    {
        var average = values.Average();
        var sum = values.Sum(value => (value - average) * (value - average));
        var standardDeviation = (float)Math.Sqrt(sum / values.Length);
        return standardDeviation;
    }

    /// <summary>
    /// Вывести в файл всю статистику
    /// </summary>
    internal static void PrintStatistics(string testsDataFile, int launchesCount, string statisticsOutputFile)
    {
        var time = new Stopwatch();
        var testsDataMatrix = MatrixGeneration.GetMatrixFromFile(testsDataFile);
        using var streamWriter = new StreamWriter(statisticsOutputFile);

        streamWriter.WriteLine("№          | Матожидание(однопоточный метод)   | Среднекв. отклонение(однопоточный метод)   | Матожидание(многопоточный метод)  | Среднекв. отклонение(многопоточный метод)");

        for (int testNumber = 0; testNumber < testsDataMatrix.GetLength(0); testNumber++)
        {
            var firstMatrix = MatrixGeneration.GenerateMatrix((int)testsDataMatrix[testNumber, 0], (int)testsDataMatrix[testNumber, 1],
                testsDataMatrix[testNumber, 4], testsDataMatrix[testNumber, 5]);
            var secondMatrix = MatrixGeneration.GenerateMatrix((int)testsDataMatrix[testNumber, 2], (int)testsDataMatrix[testNumber, 3],
                testsDataMatrix[testNumber, 4], testsDataMatrix[testNumber, 5]);

            var singleThreadMethodWorkingTimes = new float[launchesCount];
            var multiThreadMethodWorkingTimes = new float[launchesCount];

            for (int launchNumber = 0; launchNumber < launchesCount; launchNumber++)
            {
                time.Restart();
                MatrixMultiplication.SingleThreadedMatrixMultiplication(firstMatrix, secondMatrix);
                time.Stop();
                singleThreadMethodWorkingTimes[launchNumber] = (float)time.Elapsed.TotalSeconds;

                time.Restart();
                MatrixMultiplication.MultiThreadedMatrixMultiplication(firstMatrix, secondMatrix);
                time.Stop();
                multiThreadMethodWorkingTimes[launchNumber] = (float)time.Elapsed.TotalSeconds;
            }

            var singleThreadMethodMathematicalExpectation = singleThreadMethodWorkingTimes.Average();
            var multiThreadMethodMathematicalExpectation = multiThreadMethodWorkingTimes.Average();

            var singleThreadMethodStandardDeviation = CalculateStandardDeviation(singleThreadMethodWorkingTimes);
            var multiThreadMethodStandardDeviation = CalculateStandardDeviation(multiThreadMethodWorkingTimes);

            streamWriter.Write($"     {testNumber + 1}     |");
            streamWriter.Write($"            {Math.Round(singleThreadMethodMathematicalExpectation, 1)} секунд             |               {Math.Round(singleThreadMethodStandardDeviation, 3)} секунд                 |");
            streamWriter.WriteLine($"            {Math.Round(multiThreadMethodMathematicalExpectation, 1)} секунд               |          {Math.Round(multiThreadMethodStandardDeviation, 3)} секунд           ");
        }
    }
}