// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

using IceShell.Batching;
using IceShell.Core;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Bundled;
using IceShell.Platform.Linux;
using IceShell.Platform.Windows;
using IceShell.Settings;
using Spectre.Console;

if (args.Length > 1)
{
    Console.WriteLine("Invalid arguments for IceShell launcher");
    Console.WriteLine("Usage: iceshell [command]");
    return 2;
}

var userPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".iceShell");
Directory.CreateDirectory(userPath);

var configPath = Path.Combine(userPath, "config.ini");

ShellSettings? sh = null;

// Initialize configuration
if (File.Exists(configPath))
{
    try
    {
        sh = ShellSettings.LoadFromFile(configPath);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Unable to load config file!");
        Console.WriteLine(ex);
        Console.WriteLine();
    }
}

// Create if possible
if (sh == null)
{
    try
    {
        sh = new ShellSettings();
        sh.Save(configPath);
    }
    catch (Exception ex)
    {
        AnsiConsole.WriteLine("[red]Unable to create configuration![/]");
        AnsiConsole.WriteException(ex);
        sh = new ShellSettings();
    }
}

LanguageService.Instance.Load();
LanguageService.SetInstanceConfig(sh);
Executive.Attach(sh);

var shell = new Shell(sh);

shell.ModuleManager.AddModule(new BatchingModule());

if (sh.AddPlatformModules)
{
    if (OperatingSystem.IsWindows())
    {
        shell.ModuleManager.AddModule(new WindowsPlatformModule());
    }

    if (OperatingSystem.IsLinux())
    {
        shell.ModuleManager.AddModule(new LinuxPlatformModule());
    }
}

DefaultCommands.Register(shell.Dispatcher);

var retCode = -255;

if (args.Length == 0)
{
    retCode = shell.RunInteractive();
}
else if (args.Length == 1)
{
    shell.ModuleManager.Initialize();
    retCode = shell.Execute(args[0]).ExitCode;
}

Executive.Exit(retCode);
return retCode;
