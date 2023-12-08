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
    [Test]
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
        
        Assert.Multiple(() =>
        {
            Assert.That(result.OriginalWords, Has.Count.EqualTo(2));
            Assert.That(result.OriginalWords[0].Content, Is.EqualTo("Hello"));
            Assert.That(result.OriginalWords[1].Content, Is.EqualTo("World"));
        });
    }
}