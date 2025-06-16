namespace Tr1ppy.Services.Attributes;

/// <summary>
/// Indicates that a service class must be manually registered and should be excluded from any automatic registration processes.
/// </summary>
/// <remarks>
/// This attribute is used to explicitly mark service classes that should not participate in automated dependency injection registration mechanisms.
/// It's typically applied to services requiring custom configuration, conditional registration, or a different lifecycle than what automated processes provide.
/// </remarks>

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class MustManuallyRegisterServiceAttribute : Attribute
{
}
