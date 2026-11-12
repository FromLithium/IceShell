// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests.Syntax;

using IceShell.Core.Api;
using IceShell.Core.Commands.Parsing;
using IceShell.Core.Commands.Registry;
using IceShell.Core.Syntax;
using IceShell.Parsing;

public class CommandParsingProviderTests
{
    [Fact]
    public void Parse_AlwaysProvideOriginalWords()
    {
        // Note that the command is not variable values by default,
        // and the variable values is not enabled here.
        new CommandBuilder("test", (_, _, _) => CommandResult.Ok())
            .ValueParameter("message-first", 0, true)
            .ValueParameter("message-second", 1, true)
            .Build(out var entry, out _);

        var invocation = CommandParser.ParseSingleCommand(new CommandWordBreaker().RunParse("test Hello World"));

        var provider = new CommandParsingProvider(invocation, entry, null);
        var result = provider.Parse(null);
        
        Assert.Collection(result.OriginalWords,
            x1 => Assert.Equal("Hello", x1.Content),
            x2 => Assert.Equal("World", x2.Content));
    }
}