namespace MyNUnit.Tests;

public class MyNUnitTests
{
    private List<TestInfo> allTestsResults;
    private List<string> methodsToTestNames;
    private const string pathToProjectWithTests = "..\\..\\..\\..\\ProjectForTesting";

    [SetUp]
    public void Setup()
    {
        allTestsResults = new List<TestInfo>();

        methodsToTestNames = new List<string>();
        methodsToTestNames.Add("SuccessfulMethod");
        methodsToTestNames.Add("IgnoreMethod");
        methodsToTestNames.Add("IgnoreMethodThrowingException");
        methodsToTestNames.Add("ExpectedExceptionThrown");
        methodsToTestNames.Add("ExceptionOnFail");
        methodsToTestNames.Add("UnexpectedExceptionThrown");

        allTestsResults = GetInformationAboutAllTests();
    }

    private List<TestInfo> GetInformationAboutAllTests()
    {
        var report = MyNUnit.RunTestsAndGetReport(pathToProjectWithTests);
        var result = new List<TestInfo>();

        foreach (var list in report.Values)
        {
            foreach (var info in list)
            {
                result.Add(info);
            }
        }

        return result;
    }

    [Test]
    public void CorrectMethodsWasTestedTest()
    {
        var methodsNames = new List<string>();

        foreach (var testInfo in allTestsResults)
        {
            methodsNames.Add(testInfo.MethodName);
        }

        Assert.AreEqual(methodsToTestNames.Count, methodsNames.Intersect(methodsToTestNames).Count());
    }

    [Test]
    public void SuccessfulTestsPassedTest()
    {
        var successInfo = allTestsResults.Find(i => i.MethodName == "SuccessfulMethod");

        Assert.IsTrue(successInfo!.IsSuccessful);
    }

    [Test]
    public void TestToIgnoreTest()
    {

        var ignoredMethodInfo = allTestsResults.Find(i => i.MethodName == "IgnoreMethod");
        var ignoredMethodWithExceptionInfo = allTestsResults.Find(i => i.MethodName == "IgnoreMethodThrowingException");

        Assert.IsTrue(ignoredMethodInfo!.IsIgnored);
        Assert.IsTrue(ignoredMethodWithExceptionInfo!.IsIgnored);
    }

    [Test]
    public void MethodWithExpectedExceptionTest()
    {
        var methodWithExpectedExceptionInfo = allTestsResults.Find(i => i.MethodName == "ExpectedExceptionThrown");

        Assert.AreEqual(methodWithExpectedExceptionInfo.ExpectedException, methodWithExpectedExceptionInfo.ActualException);
        Assert.IsTrue(methodWithExpectedExceptionInfo.IsSuccessful);
    }

    [Test]
    public void MethodWithUnexpectedExceptionTest()
    {
        var methodWithUnexpectedExceptionInfo = allTestsResults.Find(i => i.MethodName == "UnexpectedExceptionThrown");

        Assert.AreNotEqual(methodWithUnexpectedExceptionInfo.ActualException, methodWithUnexpectedExceptionInfo.ExpectedException);
        Assert.IsFalse(methodWithUnexpectedExceptionInfo.IsSuccessful);
    }
}