namespace Tr1ppy.Services.Attributes;

/// <summary>
/// Specifies the lifetime of a service in a dependency injection container.
/// This enum is intended for use with auto-registration mechanisms.
/// </summary>
/// <remarks>
/// <para>
/// These values map to common dependency injection lifetimes found in frameworks like Microsoft.Extensions.DependencyInjection.
/// </para>
/// <list type="bullet">
/// <item>
/// <term><see cref="ServiceLifetime.Transient"/></term>
/// <description>A new instance of the service is created every time it is requested.</description>
/// </item>
/// <item>
/// <term><see cref="ServiceLifetime.Scoped"/></term>
/// <description>
/// A single instance of the service is created per scope (e.g., per web request, per database transaction).
/// Within the same scope, the same instance is returned.
/// </description>
/// </item>
/// <item>
/// <term><see cref="ServiceLifetime.Singleton"/></term>
/// <description>
/// A single instance of the service is created when it's first requested or when the DI container is initialized.
/// This single instance is then used throughout the application's lifetime.
/// </description>
/// </item>
/// <item>
/// <term><see cref="ServiceLifetime.None"/></term>
/// <description>
/// Indicates that no specific lifetime is specified, or that the service should not be registered automatically.
/// This value typically means the service will not be automatically picked up by the auto-registration process
/// unless explicitly handled.
/// </description>
/// </item>
/// </list>
/// </remarks>
public enum ServiceLifetime
{
    /// <summary>
    /// Specifies that a single instance of the service is created per scope.
    /// For example, in web applications, a scoped service is instantiated once per client request.
    /// </summary>
    Scoped = 1,

    /// <summary>
    /// Specifies that a single instance of the service is created and used throughout the application's lifetime.
    /// This is often used for services that manage global state or are expensive to create.
    /// </summary>
    Singleton = 2,

    /// <summary>
    /// Specifies that a new instance of the service is created every time it is requested.
    /// This is suitable for lightweight, stateless services.
    /// </summary>
    Transient = 3
}

/// <summary>
/// Marks a class for automatic registration in a dependency injection container.
/// </summary>
/// <remarks>
/// <para>
/// This attribute is used by auto-registration mechanisms to discover services that need to be registered
/// with a specific lifetime in the application's service collection.
/// </para>
/// <para>
/// By default, services marked with this attribute will be registered with a <see cref="ServiceLifetime.Transient"/> lifetime.
/// You can explicitly specify a different lifetime using the constructor parameter.
/// </para>
/// <para>
/// This attribute can only be applied to classes and cannot be applied multiple times to the same class.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AutoregisteredServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient) : Attribute
{
    /// <summary>
    /// Gets the <see cref="ServiceLifetime"/> specified for the service.
    /// </summary>
    public ServiceLifetime Lifetime { get; } = lifetime;
}
