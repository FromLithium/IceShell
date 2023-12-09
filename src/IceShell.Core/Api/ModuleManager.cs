// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api;

using IceShell.Core.CLI.Languages;
using NexusKrop.IceShell.Core.CLI;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Reflection;
using IceShell.Core.Api.Commands;

/// <summary>
/// Provides module management interface and service for shell environments.
/// </summary>
public class ModuleManager : IModuleManager
{
    private readonly List<IModule> _modules = new();
    private readonly IRegistryCommandDispatcher _commandDispatcher;
    private bool _initialised;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleManager"/> class.
    /// </summary>
    /// <param name="commandDispatcher">The dispatcher.</param>
    public ModuleManager(IRegistryCommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    private static InvalidOperationException ExceptionInitialised()
        => new("This module manager have already been initialised.");

    private void CheckInitialised()
    {
        if (_initialised)
        {
            throw ExceptionInitialised();
        }
    }
    
    /// <inheritdoc />
    public void LoadDirectory(string directory)
    {
        CheckInitialised();
        
        if (!Directory.Exists(directory))
        {
            // Nothing to load
            return;
        }

        foreach (var file in Directory.GetFiles(directory, "*.dll"))
        {
            try
            {
                LoadAssembly(file);
            }
            catch (BadImageFormatException)
            {
                ConsoleOutput.PrintShellError($"Invalid plugin {file}");
            }
            catch (Exception ex)
            {
                ConsoleOutput.PrintShellError(ex.ToString());
#if !DEBUG
                throw;
#endif
            }
        }
    }

    /// <inheritdoc />
    public void AddModule(IModule module)
    {
        CheckInitialised();
        
        _modules.Add(module);
    }

    /// <inheritdoc />
    public void AddModule(Type type)
    {
        CheckInitialised();
        
        var moduleI = type.GetInterface(nameof(IModule));

        if (moduleI != typeof(IModule) ||
            Activator.CreateInstance(type) is not IModule module)
        {
            return;
        }

        AddModule(module);
    }
    
    /// <inheritdoc/>
    public void LoadAssembly(string file)
    {
#pragma warning disable S3885
        // This was never intended to be anywhere near predictable
        // It is up to the user to determine whether the modules are safe
        var assembly = Assembly.LoadFrom(file);
#pragma warning restore S3885

        LoadAssembly(assembly);
    }

    /// <inheritdoc/>
    public void LoadAssembly(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            AddModule(type);
        }
    }

    /// <inheritdoc/>
    public void Initialize()
    {
        CheckInitialised();
        
        foreach (var module in _modules)
        {
            try
            {
                module.Initialize(_commandDispatcher);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]{string.Format(LangMessage.Get("init_module_fail"), module.GetType().Name)}[/]");
                ConsoleOutput.PrintShellError(ex.ToString());
            }
        }

        _initialised = true;
    }
}
