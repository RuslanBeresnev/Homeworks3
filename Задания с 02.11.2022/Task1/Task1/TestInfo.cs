﻿namespace MyNUnit;

/// <summary>
/// Represents information after method testing
/// </summary>
public class TestInfo
{
    /// <summary>
    /// Name of a method
    /// </summary>
    public string MethodName { get; private set; }

    /// <summary>
    /// Exception that is expected
    /// </summary>
    public Type? ExpectedException { get; private set; }

    /// <summary>
    /// Exception that was thrown during testing
    /// </summary>
    public Type? ActualException { get; private set; }

    /// <summary>
    /// Whether a test method should be ignored
    /// </summary>
    public bool IsIgnored { get; private set; }

    /// <summary>
    /// Whether a test was successfull
    /// </summary>
    public bool IsSuccessful { get; private set; }

    /// <summary>
    /// Reason of test ignore
    /// </summary>
    public string ReasonToIgnore { get; private set; }

    /// <summary>
    /// Amount of time testing has taken
    /// </summary>
    public TimeSpan Time { get; private set; }

    /// <summary>
    /// Constructor for ignored test methods
    /// </summary>
    public TestInfo(string name, string ignoranceReason)
    {
        IsIgnored = true;

        MethodName = name;
        ReasonToIgnore = ignoranceReason;
    }

    /// <summary>
    /// Constructor for unignored test methods
    /// </summary>
    public TestInfo(string name, bool isSuccessful, Type? expectedException, Type? actualException, TimeSpan time)
    {
        IsIgnored = false;
        ReasonToIgnore = "";

        MethodName = name;
        IsSuccessful = isSuccessful;
        ExpectedException = expectedException;
        ActualException = actualException;
        Time = time;
    }
}