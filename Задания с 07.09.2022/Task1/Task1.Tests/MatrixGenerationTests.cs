namespace Task1.Tests;

/// <summary>
/// Тестирование генерации матриц
/// </summary>
public class MatrixGenerationTests
{
    [Test]
    public void GenerateLargeMatrixTest()
    {
        var matrix = MatrixGeneration.GenerateMatrix(1000, 1000, 100f, 1000f);

        Assert.That(matrix.GetLength(0) == 1000);
        Assert.That(matrix.GetLength(1) == 1000);

        for (int i = 0; i < 1000; i++)
        {
            for (int j = 0; j < 1000; j++)
            {
                Assert.That(matrix[i, j] >= 100f);
                Assert.That(matrix[i, j] <= 1000f);
            }
        }
    }
}