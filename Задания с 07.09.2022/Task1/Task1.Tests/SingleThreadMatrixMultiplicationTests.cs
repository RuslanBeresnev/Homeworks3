namespace Task1.Tests;

/// <summary>
/// Тестирование однопоточного умножения матриц
/// </summary>
public class SingleThreadMatrixMultiplicationTests
{
    [Test]
    public void CorrectCaseMatrixMultiplicationTest()
    {
        var firstMatrix = MatrixGeneration.GetMatrixFromFile("Correct Case First Matrix.txt");
        var secondMatrix = MatrixGeneration.GetMatrixFromFile("Correct Case Second Matrix.txt");
        var resultMatrix = MatrixMultiplication.SingleThreadedMatrixMultiplication(firstMatrix, secondMatrix);
        var correctResultMatrix = MatrixGeneration.GetMatrixFromFile("Correct Case Result Matrix.txt");
        Assert.True(MatrixMultiplication.IsFirstAndSecondMatricesEquals(resultMatrix, correctResultMatrix));
    }   

    [Test]
    public void InCorrectCaseMatrixMultiplicationTest()
    {
        var firstMatrix = MatrixGeneration.GetMatrixFromFile("Incorrect Case First Matrix.txt");
        var secondMatrix = MatrixGeneration.GetMatrixFromFile("Incorrect Case Second Matrix.txt");
        Assert.Throws<InvalidDataException>(() => MatrixMultiplication.SingleThreadedMatrixMultiplication(firstMatrix, secondMatrix));
    }
}