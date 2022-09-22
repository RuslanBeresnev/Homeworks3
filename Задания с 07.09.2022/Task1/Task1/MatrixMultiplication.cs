namespace Task1;

/// <summary>
/// Реализация матричного умножения
/// </summary>
public static class MatrixMultiplication
{
    /// <summary>
    /// Проверить матрицы на одинаковость
    /// </summary>
    public static bool IsFirstAndSecondMatricesEquals(float[,] firstMatrix, float[,] secondMatrix)
    {
        if (firstMatrix.GetLength(0) != secondMatrix.GetLength(0) || firstMatrix.GetLength(1) != secondMatrix.GetLength(1))
        {
            return false;
        }

        for (int row = 0; row < firstMatrix.GetLength(0); row++)
        {
            for (int column = 0; column < firstMatrix.GetLength(1); column++)
            {
                if (firstMatrix[row, column] != secondMatrix[row, column])
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Найти произведение двух матриц с помощью одного потока
    /// </summary>
    public static float[,] SingleThreadedMatrixMultiplication(float[,] firstMatrix, float[,] secondMatrix)
    {
        if (firstMatrix.GetLength(1) != secondMatrix.GetLength(0))
        {
            throw new InvalidDataException("Матрицы с такими параметрами нельзя перемножить");
        }

        var resultMatrix = new float[firstMatrix.GetLength(0), secondMatrix.GetLength(1)];
        for (int row = 0; row < firstMatrix.GetLength(0); row++)
        {
            for (int column = 0; column < secondMatrix.GetLength(1); column++)
            {
                for (int i = 0; i < firstMatrix.GetLength(1); i++)
                {
                    resultMatrix[row, column] += firstMatrix[row, i] * secondMatrix[i, column];
                }
            }
        }

        return resultMatrix;
    }

    /// <summary>
    /// Найти произведение двух матриц с помощью нескольких потоков
    /// </summary>
    public static float[,] MultiThreadedMatrixMultiplication(float[,] firstMatrix, float[,] secondMatrix)
    {
        if (firstMatrix.GetLength(1) != secondMatrix.GetLength(0))
        {
            throw new InvalidDataException("Матрицы с такими параметрами нельзя перемножить");
        }

        var threadsCount = 8;
        var threads = new Thread[threadsCount];
        var chunkSize = firstMatrix.GetLength(0) / threadsCount + 1;
        var resultMatrix = new float[firstMatrix.GetLength(0), secondMatrix.GetLength(1)];

        for (var i = 0; i < threadsCount; i++)
        {
            var localI = i;
            threads[i] = new Thread(() =>
            {
                for (int row = localI * chunkSize; row < (localI + 1) * chunkSize && row < firstMatrix.GetLength(0); row++)
                {
                    for (int column = 0; column < secondMatrix.GetLength(1); column++)
                    {
                        for (int j = 0; j < firstMatrix.GetLength(1); j++)
                        {
                            resultMatrix[row, column] += firstMatrix[row, j] * secondMatrix[j, column];
                        }
                    }
                }
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
        foreach (var thread in threads)
        {
            thread.Join();
        }

        return resultMatrix;
    }
}