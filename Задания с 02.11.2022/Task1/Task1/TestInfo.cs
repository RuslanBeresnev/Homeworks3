namespace MyNUnit;

/// <summary>
/// Information about each test
/// </summary>
public class TestInfo
{
    public TestInfo(string name, string result, string ignoreReason, long time)
    {
        Result = result;
        Name = name;
        IgnoreReason = ignoreReason;
        Time = time;
    }

    /// <summary>
    /// Test result
    /// </summary>
    public string Result { get; }

    /// <summary>
    /// Test name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Reason of ignore
    /// </summary>
    public string IgnoreReason { get; }

    /// <summary>
    /// Test run time
    /// </summary>
    public long Time { get; }
}