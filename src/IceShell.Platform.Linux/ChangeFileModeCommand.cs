namespace IceShell.Platform.Linux;

using System.Runtime.Versioning;
using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Attributes;
using IceShell.Core.Commands.Registry;

[SupportedOSPlatform("linux")]
internal static class ChangeFileModeCommand
{
    internal static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("chmod", Routine)
            .ValueParameter("target", 0, true)
            .ValueParameter("mode", 0, true)
            .Build(dispatcher);
    }
    
    private static CommandResult Routine(IShell shell, ICommandDocument document, CommandContext context)
    {
        var targetFile = context.GetValue("target");
        var targetMode = context.GetValue("mode");
        var searchResults = Executive.GlobFiles(targetFile!);

        if (!TryParseUnixFileMode(targetMode!, out var mode))
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