namespace Task1.Tests;

public class MultiThreadMatrixMultiplicationTests
{
    [Test]
    public void CorrectCaseMatrixMultiplicationTest()
    {
        MatrixMultiplication.MultiThreadedMatrixMultiplication("../../../Correct Case First Matrix.txt",
            "../../../Correct Case Second Matrix.txt", "../../../Result Of Multi Thread.txt");
        Assert.True(File.ReadAllBytes("../../../Result Of Multi Thread.txt").SequenceEqual(File.ReadAllBytes("../../../Correct Case Result Matrix.txt")));
    }

    [Test]
    public void InCorrectCaseMatrixMultiplicationTest()
    {
        Assert.Throws<InvalidDataException>(() =>
            MatrixMultiplication.MultiThreadedMatrixMultiplication("../../../Incorrect Case First Matrix.txt",
                "../../../Incorrect Case Second Matrix.txt", "../../../Result Of Multi Thread.txt"));
    }
}