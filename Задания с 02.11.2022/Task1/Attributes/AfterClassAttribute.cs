namespace MyNUnitAttributes;

/// <summary>
/// Attribute for marking methods to be invoked after testing the class
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AfterClassAttribute : Attribute
{

}