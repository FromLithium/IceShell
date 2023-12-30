// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Platform.Windows;

using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Commands;
using IceShell.Core.Commands.Attributes;
using Spectre.Console;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;
using Vanara.PInvoke;

[CommandInfo("vol")]
internal static class VolumeInfoCommand
{
    internal static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("vol", Routine)
            .ValueParameter("volume", 0, true)
            .Build(dispatcher);
    }

    private static CommandResult Routine(IShell shell, ICommandDocument document, CommandContext context)
    {
        var volumePath = context.GetValue("volume")!;
        
        volumePath ??= Environment.CurrentDirectory;

        if (Directory.Exists(volumePath) && volumePath.Length > 3)
        {
            volumePath = volumePath[..3];
        }

        var realVolumePath = volumePath.EndsWith('\\') ? volumePath : $"{volumePath}\\";

        if (!Kernel32.GetVolumeInformation(realVolumePath,
            out var volumeName,
            out var serialNumber,
            out var maximumComponentLength,
            out var flags,
            out var fileSystemName))
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError());
        }

        var table = new Table();
        table.AddColumns("Key", "Value");
        table.AddRow("Volume Name", Markup.Escape(string.IsNullOrWhiteSpace(volumeName) ?
            "<none>" :
            volumeName));
        table.AddRow("Maximum Component Length", maximumComponentLength.ToString());
        table.AddRow("Serial Number", serialNumber.ToString("x8"));
        table.AddRow("File System", Markup.Escape(fileSystemName));

        AnsiConsole.Write(table);

        return CommandResult.Ok();
    }
}
