// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;

/// <summary>
/// Searches for lines of strings containing the specified keyword.
/// </summary>
[CommandInfo("find")]
public static class FindCommand
{
    /// <summary>
    /// Constructs this command and registers this command to the specified dispatcher.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to register to.</param>
    public static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("find", Routine)
            .ValueParameter("keyword", 0, true)
            .Build(dispatcher);
    }
    
    private static CommandResult Routine(IShell shell, ICommandDocument document, CommandContext context)
    {
        var keyword = context.GetValue("keyword");

        if (keyword == null)
        {
            throw new InvalidOperationException("Keyword should not be null.");
        }
        
        if (context.Retrieval == null)
        {
            return CommandResult.Ok();
        }

        var output = new MemoryStream();
        var writer = new StreamWriter(output, leaveOpen: true);

        while (true)
        {
            var line = context.Retrieval.ReadLine();

            if (line == null)
            {
                break;
            }

            if (line.Contains(keyword))
            {
                writer.WriteLine(line);
            }
        }

        writer.Close();

        output.Position = 0;
        return CommandResult.Ok(new StreamReader(output));
    }
}
