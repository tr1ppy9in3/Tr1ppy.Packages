using System.Security.AccessControl;

namespace Tr1ppy.System.Abstractions.Models;

[Flags]
public enum FileSystemPermissions
{
    None = 0,

    UserRead = 0x100,
    UserWrite = 0x80,
    UserExecute = 0x40,

    GroupRead = 0x20,
    GroupWrite = 0x10,
    GroupExecute = 0x8,

    OthersRead = 0x4,
    OthersWrite = 0x2,
    OthersExecute = 0x1,


    UserReadWrite = UserRead | UserWrite,
    UserReadExecute = UserRead | UserExecute,
    UserWriteExecute = UserWrite | UserExecute,
    UserAll = UserRead | UserWrite | UserExecute,

    GroupReadWrite = GroupRead | GroupWrite,
    GroupReadExecute = GroupRead | GroupExecute,
    GroupWriteExecute = GroupWrite | GroupExecute,
    GroupAll = GroupRead | GroupWrite | GroupExecute,

    OthersReadWrite = OthersRead | OthersWrite,
    OthersReadExecute = OthersRead | OthersExecute,
    OthersWriteExecute = OthersWrite | OthersExecute,
    OthersAll = OthersRead | OthersWrite | OthersExecute,

    AllRead = UserRead | GroupRead | OthersRead,
    AllWrite = UserWrite | GroupWrite | OthersWrite,
    AllExecute = UserExecute | GroupExecute | OthersExecute,

    All = UserAll | GroupAll | OthersAll
}

public static class FileSystemPermissionsExtensions
{
    public static string ToChmodString(this FileSystemPermissions permission)
    {
        int user = 0;
        if (permission.HasFlag(FileSystemPermissions.UserRead)) user += 4;
        if (permission.HasFlag(FileSystemPermissions.UserWrite)) user += 2;
        if (permission.HasFlag(FileSystemPermissions.UserExecute)) user += 1;

        int group = 0;
        if (permission.HasFlag(FileSystemPermissions.GroupRead)) group += 4;
        if (permission.HasFlag(FileSystemPermissions.GroupWrite)) group += 2;
        if (permission.HasFlag(FileSystemPermissions.GroupExecute)) group += 1;

        int others = 0;
        if (permission.HasFlag(FileSystemPermissions.OthersRead)) others += 4;
        if (permission.HasFlag(FileSystemPermissions.OthersWrite)) others += 2;
        if (permission.HasFlag(FileSystemPermissions.OthersExecute)) others += 1;

        return $"{user}{group}{others}";
    }

    public static FileSystemRights ToWindowsRights(this FileSystemPermissions permission)
    {
        FileSystemRights rights = FileSystemRights.Synchronize;

        bool hasAnyRead = 
            permission.HasFlag(FileSystemPermissions.UserRead) ||
            permission.HasFlag(FileSystemPermissions.GroupRead) ||
            permission.HasFlag(FileSystemPermissions.OthersRead);


        bool hasAnyWrite = 
            permission.HasFlag(FileSystemPermissions.UserWrite) ||
            permission.HasFlag(FileSystemPermissions.GroupWrite) ||
            permission.HasFlag(FileSystemPermissions.OthersWrite);

        bool hasAnyExecute = 
            permission.HasFlag(FileSystemPermissions.UserExecute) ||
            permission.HasFlag(FileSystemPermissions.GroupExecute) ||
            permission.HasFlag(FileSystemPermissions.OthersExecute);

        if (hasAnyRead && hasAnyExecute)
        {
            rights |= FileSystemRights.ReadAndExecute;
        }
        else if (hasAnyRead)
        {
            rights |= FileSystemRights.ReadData;
        }
        else if (hasAnyExecute)
        {
            rights |= FileSystemRights.ExecuteFile;
        }

        if (hasAnyWrite)
        {

            rights |= FileSystemRights.WriteData | 
                      FileSystemRights.AppendData |  
                      FileSystemRights.WriteAttributes |
                      FileSystemRights.WriteExtendedAttributes;
        }

        if (permission.HasFlag(FileSystemPermissions.UserAll) && permission.HasFlag(FileSystemPermissions.GroupAll) && permission.HasFlag(FileSystemPermissions.OthersAll))
        {
            rights = FileSystemRights.FullControl; 
        }
        else if (permission.HasFlag(FileSystemPermissions.UserAll) || permission.HasFlag(FileSystemPermissions.GroupAll) || permission.HasFlag(FileSystemPermissions.OthersAll))
        {

        }


        return rights;
    }
}