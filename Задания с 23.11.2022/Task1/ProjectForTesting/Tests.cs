namespace ProjectForTesting;

using MyNUnitAttributes;

/// <summary>
/// Class with tests to check correctness of MyNUnit working
/// </summary>
public class Tests
{
    [Test]
    public void SuccessfulMethod()
    {
        // pass
    }

    [Test("Reason")]
    public void IgnoreMethod()
    {
        // pass
    }

    [Test("Reason")]
    public void IgnoreMethodThrowingException()
    {
        throw new Exception();
    }

    [Test("", typeof(ArgumentNullException))]
    public void ExpectedExceptionThrown()
    {
        throw new ArgumentNullException();
    }

    [Test]
    public void ExceptionOnFail()
    {
        throw new Exception();
    }

    [Test("", typeof(ArgumentNullException))]
    public void UnexpectedExceptionThrown()
    {
        throw new Exception();
    }
}