// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Registry;

public class CommandRegistryEntry
{
    /// <summary>
    /// Gets or sets the identifier of this command.
    /// </summary>
    public string Identifier { get; set; }
    
    /// <summary>
    /// Gets or sets a list of platforms that this command supports.
    /// </summary>
    public ICollection<string>? Platforms { get; set; }
}