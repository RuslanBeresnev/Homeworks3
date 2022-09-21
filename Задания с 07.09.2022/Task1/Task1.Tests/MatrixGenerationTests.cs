namespace Task1.Tests;

/// <summary>
/// Тестирование генерации матриц
/// </summary>
public class MatrixGenerationTests
{
    [Test]
    public void GenerateLargeMatrixTest()
    {
        MatrixGeneration.GenerateMatrix("../../../First Matrix.txt", 1000, 1000, 100f, 1000f);
        var matrix = MatrixMultiplication.GetMatrixFromFile("../../../First Matrix.txt");

        if (matrix.GetLength(0) != 1000 || matrix.GetLength(1) != 1000)
        {
            Assert.Fail();
        }

        for (int i = 0; i < 1000; i++)
        {
            for (int j = 0; j < 1000; j++)
            {
                if (matrix[i, j] < 100f || matrix[i, j] > 1000f)
                {
                    Assert.Fail();
                }
            }
        }
    }
}