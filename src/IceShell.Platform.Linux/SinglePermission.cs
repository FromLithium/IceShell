namespace IceShell.Platform.Linux;

[Flags]
internal enum SinglePermission
{
    None = 0,
    Execute = 1,
    Write = 2,
    Read = 4,
    All = Execute | Write | Read
}