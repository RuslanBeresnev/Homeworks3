namespace Task1;

/// <summary>
/// Реализация матричного умножения
/// </summary>
public static class MatrixMultiplication
{
    /// <summary>
    /// Считать матрицу из файла
    /// </summary>
    private static float[,] GetMatrixFromFile(string fileName)
    {
        var allLines = File.ReadAllLines(fileName);
        var matrixHeight = allLines.Length;
        var matrixWidth = allLines[0].Split(" ").Length;

        var matrix = new float[matrixHeight, matrixWidth];
        StreamReader streamReader = new StreamReader(fileName);

        var line = streamReader.ReadLine();
        var lineIndex = 0;
        while (line != null)
        {
            var valueIndex = 0;
            foreach (var value in line.Split(" ").Select(x => float.Parse(x)))
            {
                matrix[lineIndex, valueIndex] = value;
                valueIndex++;
            }

            line = streamReader.ReadLine();
            lineIndex++;
        }

        streamReader.Close();
        return matrix;
    }

    /// <summary>
    /// Записать матрицу в файл
    /// </summary>
    private static void WriteMatrixToFile(string fileName, float[,] matrix)
    {
        using (StreamWriter streamWriter = new StreamWriter(fileName))
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                var row = "";
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    row += matrix[i, j].ToString();

                    if (j != matrix.GetLength(1) - 1)
                    {
                        row += " ";
                    }
                }
                streamWriter.WriteLine(row);
            }
        }
    }

    /// <summary>
    /// Скалярное произведение двух векторов
    /// </summary>
    private static float ScalarProductOfVectors(float[] firstVector, float[] secondVector)
    {
        if (firstVector.Length != secondVector.Length)
        {
            throw new InvalidDataException("Размерность векторов не одинакова");
        }

        var result = 0f;
        for (int i = 0; i < firstVector.Length; i++)
        {
            result += firstVector[i] * secondVector[i];
        }

        return result;
    }

    /// <summary>
    /// Получить по индексу ряд из двумерной матрицы
    /// </summary>
    private static float[] GetRowFromMatrix(float[,] matrix, int rowIndex)
    {
        var row = new float[matrix.GetLength(1)];
        for (int i = 0; i < row.Length; i++)
        {
            row[i] = matrix[rowIndex, i];
        }
        return row;
    }

    /// <summary>
    /// Получить по индексу столбец из двумерной матрицы
    /// </summary>
    private static float[] GetColumnFromMatrix(float[,] matrix, int columnIndex)
    {
        var column = new float[matrix.GetLength(0)];
        for (int i = 0; i < column.Length; i++)
        {
            column[i] = matrix[i, columnIndex];
        }
        return column;
    }

    /// <summary>
    /// Проверить корректность размерностей матриц при матричном умножении
    /// </summary>
    private static bool CheckDimensionOfMatricesIsCorrect(float[,] firstMatrix, float[,] secondMatrix)
    {
        if (firstMatrix.GetLength(1) != secondMatrix.GetLength(0))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Найти произведение двух матриц с помощью одного потока
    /// </summary>
    /// <param name="firstFile">Название файла с первой матрицей</param>
    /// <param name="secondFile">Название файла со второй матрицей</param>
    /// <param name="resultFile">Название файла, куда будет записана матрица-результат</param>
    public static void SingleThreadedMatrixMultiplication(string firstFile, string secondFile, string resultFile)
    {
        var firstMatrix = GetMatrixFromFile(firstFile);
        var secondMatrix = GetMatrixFromFile(secondFile);
        var resultMatrix = new float[firstMatrix.GetLength(0), secondMatrix.GetLength(1)];

        if (!CheckDimensionOfMatricesIsCorrect(firstMatrix, secondMatrix))
        {
            throw new InvalidDataException("Матрицы с такими параметрами нельзя перемножить");
        }

        for (int i = 0; i < resultMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < resultMatrix.GetLength(1); j++)
            {
                var valueRow = GetRowFromMatrix(firstMatrix, i);
                var valueColumn = GetColumnFromMatrix(secondMatrix, j);

                resultMatrix[i, j] = ScalarProductOfVectors(valueRow, valueColumn);
            }
        }

        WriteMatrixToFile(resultFile, resultMatrix);
    }

    /// <summary>
    /// Найти произведение двух матриц с помощью нескольких потоков
    /// </summary>
    /// <param name="firstFile">Название файла с первой матрицей</param>
    /// <param name="secondFile">Название файла со второй матрицей</param>
    /// <param name="resultFile">Название файла, куда будет записана матрица-результат</param>
    public static void MultiThreadedMatrixMultiplication(string firstFile, string secondFile, string resultFile)
    {
        var firstMatrix = GetMatrixFromFile(firstFile);
        var secondMatrix = GetMatrixFromFile(secondFile);

        if (!CheckDimensionOfMatricesIsCorrect(firstMatrix, secondMatrix))
        {
            throw new InvalidDataException("Матрицы с такими параметрами нельзя перемножить");
        }

        var threadsCount = 8;
        var threads = new Thread[threadsCount];
        var chunkSize = (firstMatrix.GetLength(0) * secondMatrix.GetLength(1)) / threadsCount + 1;
        var resultMatrix = new float[firstMatrix.GetLength(0), secondMatrix.GetLength(1)];

        for (var i = 0; i < threadsCount; i++)
        {
            threads[i] = new Thread(() =>
            {
                var localI = i;
                var resultMatrixElementsCount = resultMatrix.GetLength(0) * resultMatrix.GetLength(1);
                for (var j = localI * chunkSize; j < (localI + 1) * chunkSize && j < resultMatrixElementsCount; j++)
                {
                    var valueRowNumber = j / resultMatrix.GetLength(1);
                    var valueColumnNumber = j % resultMatrix.GetLength(1);

                    var valueRow = GetRowFromMatrix(firstMatrix, valueRowNumber);
                    var valueColumn = GetColumnFromMatrix(secondMatrix, valueColumnNumber);

                    resultMatrix[valueRowNumber, valueColumnNumber] = ScalarProductOfVectors(valueRow, valueColumn);
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

        WriteMatrixToFile(resultFile, resultMatrix);
    }
}