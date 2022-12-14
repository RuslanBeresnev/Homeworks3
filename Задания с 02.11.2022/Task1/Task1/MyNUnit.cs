namespace MyNUnit;

using System.Reflection;
using System.Collections.Concurrent;
using System.Diagnostics;
using MyNUnitAttributes;

/// <summary>
/// Simple testing system realization
/// </summary>
public static class MyNUnit
{
    // Keys: classes; values: distribution of methods by attributes in class
    private static ConcurrentDictionary<Type, ClassMethodsByAttributes> methodsToTest = new ConcurrentDictionary<Type, ClassMethodsByAttributes>();
    // Keys: classes; values: list with information about each test in class
    private static ConcurrentDictionary<Type, ConcurrentBag<TestInfo>> resultsOfTests = new ConcurrentDictionary<Type, ConcurrentBag<TestInfo>>();

    /// <summary>
    /// Get paths of different assemblies from list of all assemblies paths
    /// </summary>
    private static List<string> DistinctAssemblies(List<string> paths)
    {
        var assembliesNames = new List<string>();
        var differentAssemblies = new List<string>();

        foreach (var assemblyPath in paths)
        {
            var assemblyName = Path.GetFileName(assemblyPath);

            if (!assembliesNames.Contains(assemblyName))
            {
                assembliesNames.Add(assemblyName);
                differentAssemblies.Add(assemblyPath);
            }
        }

        return differentAssemblies;
    }

    /// <summary>
    /// Loads all classes from the assemblies inside the specified directory
    /// </summary>
    public static IEnumerable<Type> GetAllClasses(string path)
    {
        var assemblyFiles = Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories).Concat(Directory.EnumerateFiles(path,
            "*.exe", SearchOption.AllDirectories)).ToList();
        assemblyFiles.RemoveAll(assemblyPath => assemblyPath.Contains("\\Task1.exe"));
        assemblyFiles.RemoveAll(assemblyPath => assemblyPath.Contains("\\Task1.dll"));
        assemblyFiles.RemoveAll(assemblyPath => assemblyPath.Contains("\\Attributes.dll"));

        var differentAssemblyFiles = DistinctAssemblies(assemblyFiles);
        return differentAssemblyFiles.AsParallel().Select(Assembly.LoadFrom).SelectMany(assembly => assembly.ExportedTypes).Where(type => type.IsClass);
    }

    /// <summary>
    /// Prepares the assemblies and runs all the tests
    /// </summary>
    private static void AnalyzePathAndExecuteTests(string path)
    {
        var classes = GetAllClasses(path);

        Parallel.ForEach(classes, oneClass =>
        {
            methodsToTest.TryAdd(oneClass, new ClassMethodsByAttributes(oneClass));
        });

        ExecuteAllTests();
    }

    /// <summary>
    /// Runs all the tests from the specified path and prints the results to the console
    /// </summary>
    public static void RunTestsAndPrintReport(string path)
    {
        AnalyzePathAndExecuteTests(path);
        PrintResults();
    }

    /// <summary>
    /// Runs all the tests and returns the results
    /// </summary>
    public static Dictionary<Type, List<TestInfo>> RunTestsAndGetReport(string path)
    {
        AnalyzePathAndExecuteTests(path);
        return GetDictionaryOfReports();
    }

    /// <summary>
    /// Sets up an execution of all test methods from their classes
    /// </summary>
    private static void ExecuteAllTests()
    {
        Parallel.ForEach(methodsToTest.Keys, type =>
        {
            resultsOfTests.TryAdd(type, new ConcurrentBag<TestInfo>());
            bool testsCancelled = false;

            foreach (var beforeClassMethod in methodsToTest[type].BeforeClassTestMethods)
            {
                try
                {
                    beforeClassMethod.Invoke(null, null);
                }
                catch
                {
                    testsCancelled = true;
                    break;
                }
            }

            Parallel.ForEach(methodsToTest[type].TestMethods, testMethod =>
                ExecuteTestMethod(type, testMethod, testsCancelled));

            foreach (var afterClassMethod in methodsToTest[type].AfterClassTestMethods)
            {
                try
                {
                    afterClassMethod.Invoke(null, null);
                }
                catch
                {
                    foreach (var testInfo in resultsOfTests[type])
                    {
                        testInfo.IsSuccessful = false;
                        testInfo.AdditionalInformation = "Test was failed because some method " +
                        "with AfterClass attrbution executed with exception";
                    }
                    break;
                }
            }
        });
    }

    /// <summary>
    /// Executes a test method and gets information about testing
    /// </summary>
    private static void ExecuteTestMethod(Type type, MethodInfo method, bool testsCancelled)
    {
        if (testsCancelled)
        {
            resultsOfTests[type].Add(new TestInfo(method.Name, "Method BeforeClass attribute executed with exception"));
            return;
        }

        var attribute = method.GetCustomAttribute<TestAttribute>();
        var isSuccessful = false;
        Type? thrownException = null;

        var emptyConstructor = type.GetConstructor(Type.EmptyTypes);

        if (attribute!.IsIgnored)
        {
            resultsOfTests[type].Add(new TestInfo(method.Name, attribute.IgnoreMessage));
            return;
        }

        if (emptyConstructor == null)
        {
            throw new FormatException($"{type.Name} must have parameterless constructor");
        }

        var instance = emptyConstructor.Invoke(null);

        foreach (var beforeTestMethod in methodsToTest[type].BeforeTestMethods)
        {
            beforeTestMethod.Invoke(instance, null);
        }

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            method.Invoke(instance, null);

            if (attribute.ExpectedException == null)
            {
                isSuccessful = true;
                stopwatch.Stop();
            }
        }
        catch (Exception testException)
        {
            thrownException = testException.InnerException!.GetType();

            if (thrownException == attribute.ExpectedException)
            {
                isSuccessful = true;
                stopwatch.Stop();
            }
        }
        finally
        {
            stopwatch.Stop();
            var ellapsedTime = stopwatch.Elapsed;
            resultsOfTests[type].Add(new TestInfo(method.Name, isSuccessful, attribute.ExpectedException, thrownException, ellapsedTime));
        }

        foreach (var afterTestMethod in methodsToTest[type].AfterTestMethods)
        {
            afterTestMethod.Invoke(instance, null);
        }
    }

    /// <summary>
    /// Prints all tests results
    /// </summary>
    private static void PrintResults()
    {
        Console.WriteLine("Testing result");
        Console.WriteLine("-----------------------------");
        Console.WriteLine($"Total test classes count: {methodsToTest.Keys.Count}");

        var testsCount = 0;

        foreach (var testedClass in methodsToTest.Keys)
        {
            testsCount += methodsToTest[testedClass].TestsCount;
        }

        Console.WriteLine($"Total test methods count: {testsCount}");

        foreach (var someClass in resultsOfTests.Keys)
        {
            Console.WriteLine("-----------------------------");
            Console.WriteLine($"Class: {someClass}");

            foreach (var testInfo in resultsOfTests[someClass])
            {
                Console.WriteLine();
                Console.WriteLine($"Tested method: {testInfo.MethodName}()");

                if (testInfo.IsIgnored)
                {
                    Console.WriteLine($"Ignored {testInfo.MethodName}() with message: {testInfo.ReasonToIgnore}");
                    continue;
                }

                if (testInfo.ExpectedException == null)
                {
                    if (testInfo.ActualException != null)
                    {
                        Console.WriteLine($"Unexpected exception: {testInfo.ActualException}");
                    }
                }
                else
                {
                    Console.WriteLine($"Expected exception: {testInfo.ExpectedException}");
                    Console.WriteLine($"Thrown exception: {testInfo.ActualException}");
                }

                Console.WriteLine($"Elapsed time: {testInfo.Time}");

                if (testInfo.IsSuccessful)
                {
                    Console.WriteLine($"{testInfo.MethodName}() test has passed");
                }
                else
                {
                    Console.WriteLine($"{testInfo.MethodName}() test has failed");
                }
            }
        }
    }

    /// <summary>
    /// Represents ConcurrentBag as List
    /// </summary>
    private static Dictionary<Type, List<TestInfo>> GetDictionaryOfReports()
    {
        var result = new Dictionary<Type, List<TestInfo>>();

        foreach (var type in resultsOfTests.Keys)
        {
            result.Add(type, new List<TestInfo>());

            foreach (var testInfo in resultsOfTests[type])
            {
                result[type].Add(testInfo);
            }
        }

        return result;
    }
}