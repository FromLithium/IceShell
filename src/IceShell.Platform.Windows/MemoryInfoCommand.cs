// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Platform.Windows;

using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Commands;
using Spectre.Console;
using System.Management;
using System.Runtime.Versioning;

[CommandInfo("mem")]
[SupportedOSPlatform("windows")]
public class MemoryInfoCommand : IShellCommand
{
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        var wql = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
        var searcher = new ManagementObjectSearcher(wql);
        var results = searcher.Get();

        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();

        foreach (var result in results)
        {
            grid.AddRow($"{result["TotalVisibleMemorySize"]} KB", "total visible memory");
            grid.AddRow($"{result["FreePhysicalMemory"]} KB", "physical memory available");
            grid.AddRow(" ", " ");
            grid.AddRow($"{result["TotalVirtualMemorySize"]} KB", "total virtual memory");
            grid.AddRow($"{result["FreeVirtualMemory"]} KB", "virtual memory available");
        }

        AnsiConsole.Write(grid);
        return CommandResult.Ok();
    }
}
