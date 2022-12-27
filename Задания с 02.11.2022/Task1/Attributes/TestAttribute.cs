namespace MyNUnitAttributes;

/// <summary>
/// Attribute for marking test methods
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TestAttribute : Attribute
{
    /// <summary>
    /// Expected exception type
    /// </summary>
    public Type? ExpectedException { get; private set; }

    /// <summary>
    /// Massage reasoning the ignorance of the test
    /// </summary>
    public string IgnoreMessage { get; private set; }

    /// <summary>
    /// Whether the test should be ignored
    /// </summary>
    public bool IsIgnored
        => IgnoreMessage != "";

    public TestAttribute(string ignore = "", Type? expected = null)
    {
        ExpectedException = expected;
        IgnoreMessage = ignore;
    }
}