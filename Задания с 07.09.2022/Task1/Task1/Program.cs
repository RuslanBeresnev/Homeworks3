﻿namespace Task1;

/// <summary>
/// Основной поток программы
/// </summary>
public class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            if (args[0] == "-statistics")
            {
                // В файле "Test Parameters.txt" заданы параметры для тестов времени умножения матриц:
                // В каждой строке отдельный тест из 6 парамметров: Высота и Ширина первой матрицы, Высота и Ширина второй матрицы,
                // Минимальное и Максимальное генерируемое значение в матрицах

                const int LAUNCHES_COUNT = 10;
                GetStatistics.PrintStatistics("../../../Tests Parameters.txt", LAUNCHES_COUNT, "../../../Statistics Output.txt");
                Console.WriteLine("Результат сравнения записан в файл \"Statistics Output.txt\"");
            }
            return;
        }

        var firstMatrix = MatrixGeneration.GetMatrixFromFile("../../../First Matrix.txt");
        var secondMatrix = MatrixGeneration.GetMatrixFromFile("../../../Second Matrix.txt");
        var resultMatrix = MatrixMultiplication.MultiThreadedMatrixMultiplication(firstMatrix, secondMatrix);
        MatrixGeneration.WriteMatrixToFile("../../../Result Matrix.txt", resultMatrix);
        Console.WriteLine("Результат перемножения матриц записан в файл \"Result Matrix.txt\"");
    }
}