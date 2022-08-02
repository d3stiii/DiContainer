namespace DiContainer; 

/// <summary>
/// Attribute to mark method in which dependencies will be injected
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
public sealed class InjectAttribute : Attribute { }