namespace IceShell.Platform.Linux;

using System.Runtime.Versioning;
using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Commands;
using IceShell.Core.Commands.Attributes;

[CommandInfo("chmod")]
[SupportedOSPlatform("linux")]
[GreedyString]
public class ChangeFileModeCommand : IShellCommand
{
    [Value("target", true, 0)]
    public string TargetFile { get; set; }
    
    [Value("mode", true, 0)]
    public string TargetMode { get; set; }
    
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        var searchResults = Executive.GlobFiles(TargetFile);

        if (!TryParseUnixFileMode(TargetMode, out var mode))
        {
            return CommandResult.WithError(CommandErrorCode.BadArgument);
        }

        foreach (var file in searchResults)
        {
            File.SetUnixFileMode(file, mode);
        }

        return CommandResult.Ok();
    }

    private static bool TryParseUnixFileMode(string targetMode, out UnixFileMode mode)
    {
        if (IsTargetModeNumeric(targetMode))
        {
            return TryParseNumericFileMode(targetMode, out mode);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    internal static bool TryParseSingleDigit(char ch, out SinglePermission permission)
    {
        if (!int.TryParse(ch.ToString(), out var digit))
        {
            permission = SinglePermission.None;
            return false;
        }

        permission = (SinglePermission)digit;
        return true;
    }

    internal static bool TryParseNumericFileMode(string targetMode, out UnixFileMode mode)
    {
        if (!TryParseSingleDigit(targetMode[0], out var owner) ||
            !TryParseSingleDigit(targetMode[1], out var group) ||
            !TryParseSingleDigit(targetMode[2], out var other))
        {
            mode = UnixFileMode.None;
            return false;
        }

        var outMode = UnixFileMode.None;

        outMode |= owner switch
        {
            SinglePermission.Execute => UnixFileMode.UserExecute,
            SinglePermission.Write => UnixFileMode.UserWrite,
            SinglePermission.Read => UnixFileMode.UserRead,
            SinglePermission.All => UnixFileMode.UserExecute | UnixFileMode.UserWrite | UnixFileMode.UserRead,
            _ => UnixFileMode.None
        };

        outMode |= group switch
        {
            SinglePermission.Execute => UnixFileMode.GroupExecute,
            SinglePermission.Write => UnixFileMode.GroupWrite,
            SinglePermission.Read => UnixFileMode.GroupRead,
            SinglePermission.All => UnixFileMode.GroupExecute | UnixFileMode.GroupWrite | UnixFileMode.GroupRead,
            _ => UnixFileMode.None
        };
        
        outMode |= other switch
        {
            SinglePermission.Execute => UnixFileMode.OtherExecute,
            SinglePermission.Write => UnixFileMode.OtherWrite,
            SinglePermission.Read => UnixFileMode.OtherRead,
            SinglePermission.All => UnixFileMode.OtherExecute | UnixFileMode.OtherWrite | UnixFileMode.OtherRead,
            _ => UnixFileMode.None
        };

        mode = outMode;
        return true;
    }

    private static bool IsTargetModeNumeric(string targetMode)
    {
        var counter = 0;
        
        foreach (var ch in targetMode)
        {
            if (counter >= 3)
            {
                return false;
            }

            if (!char.IsDigit(ch))
            {
                return false;
            }
            
            counter++;
        }

        return true;
    }
}