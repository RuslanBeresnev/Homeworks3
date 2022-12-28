namespace MyNUnitAttributes;

/// <summary>
/// Attribute for marking methods to be invoked before each test method
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class BeforeAttribute : Attribute
{

}