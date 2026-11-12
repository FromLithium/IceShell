// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests;

using IceShell.Core.Syntax;
using IceShell.Parsing;
using IceShell.Parsing.Util;

public class CommandParserTests
{
    private bool MatchSegment(SyntaxSegment segment, SyntaxNextAction nextAction, string? cmdName = null, Predicate<SyntaxOption>? optionPredicate = null, Predicate<SyntaxWord>? valuePredicate = null)
    {
        if (segment.NextAction != nextAction)
        {
            return false;
        }

        if (cmdName != null && (segment.InternalCommand == null || segment.InternalCommand.Name != cmdName))
        {
            return false;
        }

        if (optionPredicate != null && segment.InternalCommand != null)
        {
            foreach (var option in segment.InternalCommand.Options)
            {
                if (!optionPredicate(option))
                {
                    return false;
                }
            }
        }

        if (valuePredicate != null && segment.InternalCommand != null)
        {
            foreach (var value in segment.InternalCommand.Values)
            {
                if (!valuePredicate(value))
                {
                    return false;
                }
            }
        }

        return true;
    }

    [Fact]
    public void ParseCompound_Simple()
    {
        var statements = SyntaxWord.Of("test", "argument", ">", "test1", "arg2");

        var parsed = CommandParser.ParseCompound(statements, _ => true, null);

        Assert.Multiple(
            () => Assert.Equal(2, parsed.Segments.Count),

            () => Assert.True(MatchSegment(parsed.Segments[0],
                nextAction: SyntaxNextAction.Redirect,
                cmdName: "test",
                optionPredicate: null,
                valuePredicate: null)),

            () => Assert.Collection(parsed.Segments,
                x1 => Assert.Single(x1.InternalCommand?.Values!, 
                    w => w.Content == "argument"),
                x2 => Assert.Multiple(
                    () => Assert.Equal(SyntaxNextAction.None, x2.NextAction),
                    () => Assert.Equal("test1", x2.InternalCommand?.Name),
                    () => Assert.Empty(x2.InternalCommand!.Options),
                    () => Assert.Single(x2.InternalCommand!.Values,
                        w => w.Content == "arg2")
                ))
        );
    }

    [Fact]
    public void ParseOption_NoValueOption()
    {
        var statement = new SyntaxWord("/T");
        var retVal = CommandParser.ParseOption(statement, null, out _);

        Assert.Equal(new SyntaxOption('T', null), retVal);
    }

    [Fact]
    public void ParseOption_SimpleOption()
    {
        var statement = new SyntaxWord("/T:test_value");
        var retVal = CommandParser.ParseOption(statement, null, out _);

        Assert.Equal(new SyntaxOption('T', "test_value"), retVal);
    }

    [Fact]
    public void ParseOption_CompoundOption()
    {
        var statementFirst = new SyntaxWord("/T:");
        var statementSecond = new SyntaxWord("Test text", WordQuoteType.Quotes);

        var retVal = CommandParser.ParseOption(statementFirst, statementSecond, out _);

        Assert.Equal(new SyntaxOption('T', "Test text"), retVal);
    }

    [Fact]
    public void ParseOption_OptionAndUnixPath()
    {
        var statements = new SyntaxWord[]{
            new("command"),
            new("/O"),
            new("/bin/bash") };

        var retVal = CommandParser.ParseSingleCommand(statements);
        Assert.Multiple(
            () => Assert.Equal("command", retVal.Name),
            () => Assert.Single(retVal.Values, new SyntaxWord("/bin/bash")),
            () => Assert.Single(retVal.Options, new SyntaxOption('O', null))
        );
    }

    [Fact]
    public void ParseCommand_Empty()
    {
        var retVal = CommandParser.ParseSingleCommand(Array.Empty<SyntaxWord>());
        Assert.Multiple(
            () => Assert.Empty(retVal.Name),
            () => Assert.Empty(retVal.Options),
            () => Assert.Empty(retVal.Values)
        );
    }

    [Fact]
    public void ParseCommand_OptionsOnly()
    {
        var statements = new CommandWordBreaker(true).RunParse("command /T:test_value /Q:\"test value\" /X");
        var retVal = CommandParser.ParseSingleCommand(statements);

        Assert.Multiple(
            () => Assert.Equal("command", retVal.Name),
            () => Assert.Collection(retVal.Options,
                x1 => Assert.Equal(new SyntaxOption('T', "test_value"), x1),
                x2 => Assert.Equal(new SyntaxOption('Q', "test value"), x2),
                x3 => Assert.Equal(new SyntaxOption('X', null), x3)
            ),
            () => Assert.Empty(retVal.Values)
        );
    }

    [Fact]
    public void ParseCommand_ValuesOnly()
    {
        var statements = new CommandWordBreaker(true).RunParse("command \"quoted value\" simple_value");
        var retVal = CommandParser.ParseSingleCommand(statements);

        Assert.Multiple(
            () => Assert.Equal("command", retVal.Name),
            () => Assert.Collection(retVal.Values, 
                x1 => Assert.Equal(new("quoted value", WordQuoteType.Quotes, 8), x1),
                x2 => Assert.Equal(new("simple_value", WordQuoteType.None, 23), x2)
            ),
            () => Assert.Empty(retVal.Options)
        );
    }

    [Fact]
    public void ParseCommand_QuotedOptions()
    {
        var statements = new CommandWordBreaker(true).RunParse("command \"/X\" /M");
        var retVal = CommandParser.ParseSingleCommand(statements);

        Assert.Multiple(
            () => Assert.Equal("command", retVal.Name),
            () => Assert.Single(retVal.Values, new SyntaxWord("/X", WordQuoteType.Quotes, 8)),
            () => Assert.Single(retVal.Options, new SyntaxOption('M', null))
        );
    }

    [Fact]
    public void ParseCommand_EndOfOptions()
    {
        var statements = new CommandWordBreaker().RunParse("command /A /B -- /C");
        var retVal = CommandParser.ParseSingleCommand(statements);

        Assert.Multiple(
            () => Assert.Equal("command", retVal.Name),
            () => Assert.Collection(retVal.Options,
                x1 => Assert.Equal(new SyntaxOption('A', null), x1),
                x2 => Assert.Equal(new SyntaxOption('B', null), x2)
            ),
            () => Assert.Single(retVal.Values, new SyntaxWord("/C", startsFrom: 17))
        );
    }
}
