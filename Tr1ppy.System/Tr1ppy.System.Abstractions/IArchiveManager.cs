namespace Tr1ppy.System.Abstractions;

public interface IArchiveManager
{
    /// <summary>
    /// 
    /// </summary>
    static Lazy<IEnumerable<string>> Extensions { get; } = new Lazy<IEnumerable<string>>(() =>
    {
        return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "zip",  "rar",  "7z",    "tar",    "gz",
            "tgz",  "bz2",  "tbz",   "tbz2",   "xz",
            "txz",  "Z",    "lzma",  "cab",    "arj",
            "lzh",  "lha",  "ace",   "cpio",   "arc",
            "zoo",  "wim",  "dar",   "pea",    "lz",
            "lz4",  "zst",  "br",    "iso",    "img",
            "dmg",  "vhd",  "vhdx",  "vdi",    "vmdk",
            "nrg",  "mdf",  "ccd",   "cue",    "bin",
            "apk",  "jar",  "war",   "ear",    "xpi",
            "msi",  "deb",  "rpm",   "nupkg",  "ipa",
            "chm",  "egg",  "whl",   "unitypackage", "vsix",
            "r00",  "r01",  "r02",   "r03",    "r04",
            "r05",  "r06",  "r07",   "r08",    "r09",
            "part01.rar", "part02.rar", "s7z", "zip.001", "zip.002",
            "z01",  "z02",  "paq",   "paq8p",  "paq9a",
            "sfv",  "md5",  "sha1",  "sha256", "sha512",
            "bup",  "bak",  "dlc"
        };
    });

    void Extract(string archivePath, string destinationDirectory);

    void IsEmpty(string archivePath);

    IEnumerable<string> GetContentNames(string archivePath);
}
