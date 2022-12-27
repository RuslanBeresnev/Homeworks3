namespace MyNUnitAttributes;

/// <summary>
/// Attribute for marking methods to be invoked before testing the class
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class BeforeClassAttribute : Attribute
{

}