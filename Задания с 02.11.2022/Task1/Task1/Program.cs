using MyNUnit;

if (args.Length == 0 || args.Length > 1)
{
    throw new InvalidDataException("Program must have one input argument");
}

Console.WriteLine("Test Running ...");
MyNUnit.MyNUnit.RunTestsAndPrintReport(args[0]);