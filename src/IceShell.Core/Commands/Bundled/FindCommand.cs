namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using IceShell.Core.Commands.Attributes;
using NexusKrop.IceShell.Core.Commands.Complex;

/// <summary>
/// Searches for lines of strings containing the specified keyword.
/// </summary>
[CommandInfo("find")]
public class FindCommand : IShellCommand
{
    /// <summary>
    /// Gets or sets the keyword to search for.
    /// </summary>
    [Value("keyword", true, 0)]
    public string Keyword { get; set; } = "";

    /// <inheritdoc/>
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
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

            if (line.Contains(Keyword))
            {
                writer.WriteLine(line);
            }
        }

        writer.Close();

        output.Position = 0;
        return CommandResult.Ok(new StreamReader(output));
    }
}
