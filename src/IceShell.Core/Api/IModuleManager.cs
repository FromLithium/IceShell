// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Api;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

/// <summary>
/// Defines an interface for module management.
/// </summary>
public interface IModuleManager
{
    /// <summary>
    /// Adds an instance of <see cref="IModule"/> to the module registration.
    /// </summary>
    /// <param name="module">The module to add.</param>
    /// <exception cref="InvalidOperationException">This instance is already initialised.</exception>
    void AddModule(IModule module);

    /// <summary>
    /// Adds a <see cref="Type"/> representing a module to the module registration.
    /// </summary>
    /// <param name="type">The type to add.</param>
    /// <exception cref="InvalidOperationException">This instance is already initialised.</exception>
    [RequiresUnreferencedCode("Requires reflection on the specified type.")]
    void AddModule(Type type);

    /// <summary>
    /// Loads all modules from the specified assembly file.
    /// </summary>
    /// <param name="file">The assembly file to load modules from.</param>
    /// <exception cref="InvalidOperationException">This instance is already initialised.</exception>
    [RequiresUnreferencedCode("Requires reflection on the specified file.")]
    void LoadAssembly(string file);

    /// <summary>
    /// Loads all modules from the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to load modules from.</param>
    /// <exception cref="InvalidOperationException">This instance is already initialised.</exception>
    [RequiresUnreferencedCode("Requires reflection on the specified assembly.")]
    void LoadAssembly(Assembly assembly);

    /// <summary>
    /// Searches for modules in the specified directory, and load the searched modules.
    /// </summary>
    /// <param name="directory">The directory to load modules from.</param>
    /// <exception cref="InvalidOperationException">This instance is already initialised.</exception>
    [RequiresUnreferencedCode("Requires reflection on the specified directory.")]
    void LoadDirectory(string directory);

    /// <summary>
    /// Initializes all modules in module registrations.
    /// </summary>
    /// <exception cref="InvalidOperationException">This instance is already initialised.</exception>
    void Initialize();
}
