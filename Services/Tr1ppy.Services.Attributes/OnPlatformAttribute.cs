using System.Runtime.InteropServices;

namespace Tr1ppy.Services.Attributes;

/// <summary>
/// Specifies supported operating system platforms.
/// This enum is marked with the <see cref="FlagsAttribute"/>, allowing a combination of its members.
/// </summary>
/// <remarks>
/// Use bitwise OR operations to combine multiple platforms (e.g., <c>SupportedOSPlatform.Windows | SupportedOSPlatform.Linux</c>).
/// </remarks>
[Flags]
public enum SupportedOSPlatform
{
    /// <summary>
    /// Represents the Windows operating system.
    /// </summary>
    Windows = 1 << 0,
    /// <summary>
    /// Represents the Linux operating system.
    /// </summary>
    Linux = 1 << 1,
    /// <summary>
    /// Represents the macOS operating system.
    /// </summary>
    MacOS = 1 << 2,

    /// <summary>
    /// Represents all supported operating systems.
    /// </summary>
    All = ~0
}

internal static class SupportOsPlatformExtensions
{
    internal static IEnumerable<OSPlatform> ToOSPlatforms(this SupportedOSPlatform supportedOsPlatform)
    {
        List<OSPlatform> platforms = new(Enum.GetValues(typeof(SupportedOSPlatform)).Length);

        if (supportedOsPlatform.HasFlag(SupportedOSPlatform.All))
        {
            yield return OSPlatform.Windows;
            yield return OSPlatform.Linux;
            yield return OSPlatform.OSX;
            yield break;
        }


        if (supportedOsPlatform.HasFlag(SupportedOSPlatform.Windows))
            yield return OSPlatform.OSX;

        if (supportedOsPlatform.HasFlag(SupportedOSPlatform.Linux))
            yield return OSPlatform.Linux;

        if (supportedOsPlatform.HasFlag(SupportedOSPlatform.MacOS))
            yield return OSPlatform.OSX;
    }
}

/// <summary>
/// Conditionally marks a class for registration or use based on the current operating system platform.
/// </summary>
/// <remarks>
/// <para>
/// This attribute indicates that a class is supported by or intended for specific operating system platforms.
/// It enables runtime logic to adapt or filter services based on the current environment.
/// </para>
/// <para>
/// Classes marked with this attribute will typically only be considered for auto-registration
/// if the specified <see cref="Platform"/> matches or includes the currently running operating system.
/// If the current OS does not meet this condition, the service will be skipped by auto-registration processes.
/// </para>
/// <para>
/// This attribute can only be applied to <see cref="AttributeTargets.Class"/> and can be applied only <c>once</c> per class.
/// If a class needs to support multiple platforms, these should be combined using bitwise OR in a single attribute instance
/// (e.g., <c>[OnPlatform(SupportedOSPlatform.Windows | SupportedOSPlatform.Linux)]</c>).
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class OnPlatformAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OnPlatformAttribute"/> class with the specified platform(s).
    /// </summary>
    /// <param name="platform">
    /// The <see cref="SupportedOSPlatform"/> enum value indicating the platform(s) this class is intended for.
    /// </param>
    public OnPlatformAttribute(SupportedOSPlatform platform)
    {
        Platform = platform;
    }

    /// <summary>
    /// Gets the operating system platform(s) associated with this attribute instance.
    /// </summary>
    public SupportedOSPlatform Platform { get; }
    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public bool IsSupportByCurrentPlatform(Type serviceType)
    {
        OSPlatform currentOSPlatform;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            currentOSPlatform = OSPlatform.Windows;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            currentOSPlatform = OSPlatform.Linux;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            currentOSPlatform = OSPlatform.OSX;
        }
        else
        {
            return false;
        }

        return Platform.ToOSPlatforms().Contains(currentOSPlatform);
    }
}