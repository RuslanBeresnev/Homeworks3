namespace Task1;

/// <summary>
/// Реализация генерации матриц со случайными значениями
/// </summary>
public static class MatrixGeneration
{
    /// <summary>
    /// Сгенерировать матрицу определённых размеров
    /// </summary>
    /// <param name="fileName">Файл, куда будет записана матрица</param>
    /// <param name="width">Количество столбцов в матрице</param>
    /// <param name="height">Количество строк в матрице</param>
    /// <param name="minValue">Минимальное float-значение, которое может быть сгенерировано в матрице</param>
    /// <param name="maxValue">Максимальное float-значение, которое может быть сгенерировано в матрице</param>
    public static void GenerateMatrix(string fileName, int height, int width, float minValue, float maxValue)
    {
        var matrix = new float[height, width];
        var random = new Random();

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                matrix[i, j] = ((float) random.NextDouble()) * (maxValue - minValue) + minValue;
            }
        }

        MatrixMultiplication.WriteMatrixToFile(fileName, matrix);
    }
}