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
using Vanara.PInvoke;

[CommandInfo("vol")]
internal class VolumeInfoCommand : IShellCommand
{
    [Value("volume", false, 0)]
    public string? VolumePath { get; set; }

    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        VolumePath ??= Environment.CurrentDirectory;

        if (Directory.Exists(VolumePath) && VolumePath.Length > 3)
        {
            VolumePath = VolumePath[..3];
        }

        var realVolumePath = VolumePath.EndsWith('\\') ? VolumePath : $"{VolumePath}\\";

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
