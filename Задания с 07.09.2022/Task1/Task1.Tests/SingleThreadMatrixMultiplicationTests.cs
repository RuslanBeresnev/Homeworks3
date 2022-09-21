namespace Task1.Tests;

/// <summary>
/// “естирование однопоточного умножени€ матриц
/// </summary>
public class SingleThreadMatrixMultiplicationTests
{
    [Test]
    public void CorrectCaseMatrixMultiplicationTest()
    {
        MatrixMultiplication.SingleThreadedMatrixMultiplication("../../../Correct Case First Matrix.txt",
            "../../../Correct Case Second Matrix.txt", "../../../Result Of Single Thread.txt");
        Assert.True(File.ReadAllBytes("../../../Result Of Single Thread.txt").SequenceEqual(File.ReadAllBytes("../../../Correct Case Result Matrix.txt")));
    }

    [Test]
    public void InCorrectCaseMatrixMultiplicationTest()
    {
        Assert.Throws<InvalidDataException>(() =>
            MatrixMultiplication.SingleThreadedMatrixMultiplication("../../../Incorrect Case First Matrix.txt",
                "../../../Incorrect Case Second Matrix.txt", "../../../Result Of Single Thread.txt"));
    }
}