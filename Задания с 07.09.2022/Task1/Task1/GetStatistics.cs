using System.Diagnostics;

namespace Task1;

/// <summary>
/// Реализация статистики при сравнении работы двух способов умножения матриц
/// </summary>
internal class GetStatistics
{
    /// <summary>
    /// Посчитать математическое ожидание для массива равновероятных величин
    /// </summary>
    private static float CalculateMathematicalExpectation(float[] values)
    {
        var result = 0f;
        for (int i = 0; i < values.Length; i++)
        {
            result += values[i] * (1f / values.Length);
        }
        return result;
    }

    /// <summary>
    /// Посчитать среднеквадратичное отклонение для массива равновероятных величин
    /// </summary>
    private static float CalculateStandardDeviation(float[] values)
    {
        var mathematicalExpectation = CalculateMathematicalExpectation(values);
        var sumOfSquaredDeviations = 0f;

        for (int i = 0; i < values.Length; i++)
        {
            sumOfSquaredDeviations += (float) Math.Pow(values[i] - mathematicalExpectation, 2);
        }
        var standardDeviation = (float)Math.Sqrt(sumOfSquaredDeviations / values.Length);

        return standardDeviation;
    }

    /// <summary>
    /// Вывести в файл всю статистику
    /// </summary>
    internal static void PrintStatistics(string testsDataFile, int launchesCount, string statisticsOutputFile)
    {
        var time = new Stopwatch();
        var testsDataMatrix = MatrixGeneration.GetMatrixFromFile(testsDataFile);
        StreamWriter streamWriter = new StreamWriter(statisticsOutputFile);

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
                singleThreadMethodWorkingTimes[launchNumber] = (float) time.Elapsed.TotalSeconds;

                time.Restart();
                MatrixMultiplication.MultiThreadedMatrixMultiplication(firstMatrix, secondMatrix);
                time.Stop();
                multiThreadMethodWorkingTimes[launchNumber] = (float) time.Elapsed.TotalSeconds;
            }

            var singleThreadMethodMathematicalExpectation = CalculateMathematicalExpectation(singleThreadMethodWorkingTimes);
            var multiThreadMethodMathematicalExpectation = CalculateMathematicalExpectation(multiThreadMethodWorkingTimes);

            var singleThreadMethodStandardDeviation = CalculateStandardDeviation(singleThreadMethodWorkingTimes);
            var multiThreadMethodStandardDeviation = CalculateStandardDeviation(multiThreadMethodWorkingTimes);

            streamWriter.WriteLine($"Статистика для теста под номером: {testNumber}");
            streamWriter.WriteLine("--------------------------------------------------------------------------------");
            streamWriter.WriteLine("Матожидание для однопоточного метода " +
                $"умножения матриц (секунды): {singleThreadMethodMathematicalExpectation}");
            streamWriter.WriteLine("Среднеквадратичное отклонение для однопоточного метода " +
                $"умножения матриц (секунды): {singleThreadMethodStandardDeviation}");
            streamWriter.WriteLine("Матожидание для многопоточного метода " +
                $"умножения матриц (секунды): {multiThreadMethodMathematicalExpectation}");
            streamWriter.WriteLine("Среднеквадратичное отклонение для многопоточного метода " +
                $"умножения матриц (секунды): {multiThreadMethodStandardDeviation}");
            streamWriter.WriteLine("Ускорение при использовании многопоточного метода " +
                $"(во сколько раз быстрее): {singleThreadMethodMathematicalExpectation / multiThreadMethodMathematicalExpectation}");
            streamWriter.WriteLine();
        }

        streamWriter.Close();
    }
}