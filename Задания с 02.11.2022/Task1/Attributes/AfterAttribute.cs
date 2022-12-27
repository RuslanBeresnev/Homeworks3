namespace MyNUnitAttributes;

/// <summary>
/// Attribute for marking methods to be invoked after each test method
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AfterAttribute : Attribute
{

}