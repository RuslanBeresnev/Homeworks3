namespace ProjectForTesting;

using MyNUnitAttributes;

/// <summary>
/// Class with BeforeClass and AfterClass attributes to check their correctness
/// </summary>
public class TestsWithBeforeClassAndAfterClasses
{
    [Test]
    public void SimpleMethod()
    {
        // pass
    }

    [BeforeClass]
    public static void WrongBeforeClassMethod()
    {
        throw new Exception();
    }

    [AfterClass]
    public static void WrongAfterClassMethod()
    {
        throw new Exception();
    }
}