// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Utilities;

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands.Registry;
using IceShell.Core.FileSystem;
using IceShell.Core.Utilities;
using Microsoft.Extensions.FileSystemGlobbing;
using NexusKrop.IceCube.Util.Enumerables;

/// <summary>
/// Specifies a routine for file operation with a source and a destination.
/// </summary>
public delegate void DestinationFileCommandDelegate(string source, string destination);

/// <summary>
/// Provides context services for destination based file commands.
/// </summary>
public class DestinationFileCommandContext
{
    private const string DestinationFileMore = "generic_destination_file_more";
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DestinationFileCommandContext"/> class.
    /// </summary>
    /// <param name="context"></param>
    public DestinationFileCommandContext(CommandContext context)
    {
        Force = context.GetSwitch('f');

        var words = context.Arguments.OriginalWords;
        Destination = words[^1].Content;

        Sources = words.SkipLast(1).Select(x => x.Content);
    }
    
    /// <summary>
    /// Gets or sets the destination.
    /// </summary>
    public string Destination { get; }
    
    /// <summary>
    /// Gets the source files.
    /// </summary>
    public IEnumerable<string> Sources { get; }
    
    /// <summary>
    /// Gets a value indicating whether to override destination files.
    /// </summary>
    public bool Force { get; }
    
    /// <summary>
    /// Executes the specified operation.
    /// </summary>
    /// <param name="action">The operation to execute.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The results</returns>
    public CommandResult Do(DestinationFileCommandDelegate action, CancellationToken token = default)
    {
        if (!Sources.Any())
        {
            return CommandResult.WithMissingValue("sources");
        }
        
        var matcher = new Matcher();

        Sources.Select(t => PathSearcher.ExpandVariables(t))
            .ForEach(x => matcher.AddInclude(x));

        var destIsDir = Directory.Exists(Destination) || Destination!.EndsWith(Path.DirectorySeparatorChar);
        
        if (destIsDir)
        {
            CommandChecks.DirectoryExists(Destination);
        }
        else if (Sources.HaveAtLeast(2))
        {
            return CommandResult.WithError(CommandErrorCode.BadArgument, LangMessage.Get(DestinationFileMore));
        }
        
        var realFiles = matcher.GetResultsInFullPath(Directory.GetCurrentDirectory());

        foreach (var file in realFiles)
        {
            if (!File.Exists(file))
            {
                continue;
            }

            if (token.IsCancellationRequested)
            {
                return CommandResult.WithInterrupted();
            }

            // Sources and destination are already separate here so we do not need to
            // make another loop.
            action(file, destIsDir 
                ? Path.Combine(Destination, Path.GetFileName(file))
                : Destination);
        }

        return CommandResult.Ok();
    }
}