// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Platform.Windows;

using System.Runtime.InteropServices;
using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands;
using IceShell.Core.Commands.Registry;
using Vanara.PInvoke;

internal static class WindowsClipCommand
{
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("clip", Routine)
            .Build(dispatcher);
    }

    private static CommandResult Routine(IShell shell, ICommandExecutor executor, CommandContext context)
    {
        if (context.Retrieval == null)
        {
            return CommandResult.WithError(CommandErrorCode.RequiresPipelineInput);
        }
        
        using (var retrieval = context.Retrieval)
        {
            if (!User32.OpenClipboard(HWND.NULL))
            {
                return CommandResult.WithError(CommandErrorCode.GenericCommandFail, 
                    LangMessage.Get("command.clip.windows.creation_failed"));                
            }

            var data = retrieval.ReadToEnd();
            
            User32.EmptyClipboard();
            User32.SetClipboardData(13, Marshal.StringToHGlobalUni(data));
            User32.CloseClipboard();
        }
        
        return CommandResult.Ok();
    }
}