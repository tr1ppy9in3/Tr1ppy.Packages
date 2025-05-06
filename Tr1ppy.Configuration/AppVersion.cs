namespace Tr1ppy.Configuration;

public class AppVersion()
{
    public string Raw {  get; set; } = string.Empty;

    public string Major { get; set; } = string.Empty;

    public string Minor { get; set; } = string.Empty;

    public string Patch { get; set; } = string.Empty;

    public string Tag { get; set; } = string.Empty;


    public static AppVersion FromString(string versionString)
    {
        var version = new AppVersion()
        {
            Raw = versionString
        };

        string[] versionParts = versionString.Split(['-', '.'], StringSplitOptions.RemoveEmptyEntries);

        if (versionParts.Length > 0)
            version.Major = versionParts[0];

        if (versionParts.Length > 1)
            version.Minor = versionParts[1];

        if (versionParts.Length > 2)
            version.Patch = versionParts[2];

        if (versionParts.Length > 3)
            version.Tag = versionParts[3];

        return version;
     }

    public void CopyTo(AppVersion other)
    {
        other.Raw = Raw;
        other.Major = Major;
        other.Minor = Minor;
        other.Patch = Patch;
    }


    public override string ToString() => 
        $"{Major}.{Minor}.{Patch}-{Tag}";
    
}
