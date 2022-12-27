namespace MyNUnit.Tests;

public class MyNUnitTests
{
    private List<TestInfo> baseTestsResults;
    private List<TestInfo> additionalTestsResults;

    private List<string> baseMethodsToTestNames;

    private const string pathToProjectWithBaseTests = "..\\..\\..\\..\\ProjectForTesting";
    private const string pathToProjectWithAdditionalTests = "..\\..\\..\\..\\ProjectForAdditionalTesting";

    [SetUp]
    public void Setup()
    {
        baseTestsResults = new List<TestInfo>();
        additionalTestsResults = new List<TestInfo>();

        baseMethodsToTestNames = new List<string>();
        baseMethodsToTestNames.Add("SuccessfulMethod");
        baseMethodsToTestNames.Add("IgnoreMethod");
        baseMethodsToTestNames.Add("IgnoreMethodThrowingException");
        baseMethodsToTestNames.Add("ExpectedExceptionThrown");
        baseMethodsToTestNames.Add("ExceptionOnFail");
        baseMethodsToTestNames.Add("UnexpectedExceptionThrown");

        baseTestsResults = GetInformationAboutTestsInTestedProject(pathToProjectWithBaseTests);
        additionalTestsResults = GetInformationAboutTestsInTestedProject(pathToProjectWithAdditionalTests);
    }

    private List<TestInfo> GetInformationAboutTestsInTestedProject(string pathToProject)
    {
        var report = MyNUnit.RunTestsAndGetReport(pathToProject);
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

        foreach (var testInfo in baseTestsResults)
        {
            methodsNames.Add(testInfo.MethodName);
        }

        Assert.That(baseMethodsToTestNames.Count == methodsNames.Intersect(baseMethodsToTestNames).Count());
    }

    [Test]
    public void SuccessfulTestsPassedTest()
    {
        var successInfo = baseTestsResults.Find(i => i.MethodName == "SuccessfulMethod");

        Assert.IsTrue(successInfo!.IsSuccessful);
    }

    [Test]
    public void TestToIgnoreTest()
    {

        var ignoredMethodInfo = baseTestsResults.Find(i => i.MethodName == "IgnoreMethod");
        var ignoredMethodWithExceptionInfo = baseTestsResults.Find(i => i.MethodName == "IgnoreMethodThrowingException");

        Assert.IsTrue(ignoredMethodInfo!.IsIgnored);
        Assert.IsTrue(ignoredMethodWithExceptionInfo!.IsIgnored);
    }

    [Test]
    public void MethodWithExpectedExceptionTest()
    {
        var methodWithExpectedExceptionInfo = baseTestsResults.Find(i => i.MethodName == "ExpectedExceptionThrown");

        Assert.That(methodWithExpectedExceptionInfo!.ExpectedException == methodWithExpectedExceptionInfo.ActualException);
        Assert.IsTrue(methodWithExpectedExceptionInfo.IsSuccessful);
    }

    [Test]
    public void MethodWithUnexpectedExceptionTest()
    {
        var methodWithUnexpectedExceptionInfo = baseTestsResults.Find(i => i.MethodName == "UnexpectedExceptionThrown");

        Assert.That(methodWithUnexpectedExceptionInfo!.ActualException != methodWithUnexpectedExceptionInfo.ExpectedException);
        Assert.IsFalse(methodWithUnexpectedExceptionInfo.IsSuccessful);
    }

    [Test]
    public void MethodWithBeforeClassAttributeExecutedWithExceptionTest()
    {
        var simpleMethodInfo = additionalTestsResults.Find(i => i.MethodName == "SimpleMethod");
        Assert.IsTrue(simpleMethodInfo!.IsIgnored);
    }

    [Test]
    public void MethodWithAfterClassAttributeExecutedWithExceptionTest()
    {
        var simpleMethodInfo = additionalTestsResults.Find(i => i.MethodName == "SimpleMethod");
        Assert.IsFalse(simpleMethodInfo!.IsSuccessful);
        Assert.That(simpleMethodInfo!.AdditionalInformation != "");
    }
}