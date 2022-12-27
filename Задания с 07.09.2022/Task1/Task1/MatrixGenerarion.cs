namespace Task1;

/// <summary>
/// Реализация генерации матриц со случайными значениями
/// </summary>
public static class MatrixGeneration
{
    private static Random random = new();

    /// <summary>
    /// Считать матрицу из файла
    /// </summary>
    public static float[,] GetMatrixFromFile(string fileName)
    {
        var allLines = File.ReadAllLines(fileName);
        var matrixHeight = allLines.Length;
        var matrixWidth = allLines[0].Split(" ").Length;

        var matrix = new float[matrixHeight, matrixWidth];
        using var streamReader = new StreamReader(fileName);

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
    public static void WriteMatrixToFile(string fileName, float[,] matrix)
    {
        using var streamWriter = new StreamWriter(fileName);

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

    /// <summary>
    /// Сгенерировать матрицу определённых размеров
    /// </summary>
    /// <param name="width">Количество столбцов в матрице</param>
    /// <param name="height">Количество строк в матрице</param>
    /// <param name="minValue">Минимальное float-значение, которое может быть сгенерировано в матрице</param>
    /// <param name="maxValue">Максимальное float-значение, которое может быть сгенерировано в матрице</param>
    public static float[,] GenerateMatrix(int height, int width, float minValue, float maxValue)
    {
        var matrix = new float[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                matrix[i, j] = ((float) random.NextDouble()) * (maxValue - minValue) + minValue;
            }
        }

        return matrix;
    }
}