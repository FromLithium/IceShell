// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api;

using IceShell.Core.CLI.Languages;
using NexusKrop.IceShell.Core.CLI;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using IceShell.Core.Api.Commands;

/// <summary>
/// Provides module management interface and service for shell environments.
/// </summary>
public class ModuleManager : IModuleManager
{
    private readonly List<IModule> _modules = [];
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
    private static NotSupportedException ExceptionNotSupported()
        => new("Loading external modules are no longer supported.");

    private void CheckInitialised()
    {
        if (_initialised)
        {
            throw ExceptionInitialised();
        }
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Requires reflection on the specified directory.")]
    [Obsolete("Loading external modules are now not supported.", true)]
    public void LoadDirectory(string directory)
    {
        throw ExceptionNotSupported();
    }

    /// <inheritdoc />
    public void AddModule(IModule module)
    {
        CheckInitialised();

        _modules.Add(module);
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Requires reflection on the specified type.")]
    public void AddModule(Type type)
    {
        CheckInitialised();

        Type? moduleI = type.GetInterface(nameof(IModule));

        if (moduleI != typeof(IModule) ||
            Activator.CreateInstance(type) is not IModule module)
        {
            return;
        }

        AddModule(module);
    }

    /// <inheritdoc/>
    [Obsolete("Loading external modules are now not supported.", true)]
    [RequiresUnreferencedCode("Requires reflection on the specified assembly.")]
    public void LoadAssembly(string file)
    {
        throw ExceptionNotSupported();
    }

    /// <inheritdoc/>
    [Obsolete("Loading external modules are now not supported.", true)]
    [RequiresUnreferencedCode("Requires reflection on the specified assembly.")]
    public void LoadAssembly(Assembly assembly)
    {
        throw ExceptionNotSupported();
    }

    /// <inheritdoc/>
    public void Initialize()
    {
        CheckInitialised();

        foreach (IModule module in _modules)
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
