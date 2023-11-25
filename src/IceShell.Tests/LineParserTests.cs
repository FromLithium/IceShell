// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests;

using IceShell.Parsing;
using IceShell.Parsing.Util;

public class LineParserTests
{
    [Test]
    public void BasicParseTest()
    {
        var parser = new LineParser();
        var retVal = parser.ParseLine("command /F /V:\"value in prop\" /Q:varr normal_value \"quoted value\"");

        Assert.That(retVal, Is.EqualTo(new SyntaxWord[]
        {
            new("command", startsFrom: 0),
            new("/F", startsFrom: 8),
            new("/V:", startsFrom: 11),
            new("value in prop", WordQuoteType.Quotes, startsFrom: 15),
            new("/Q:varr", startsFrom: 30),
            new("normal_value", startsFrom: 38),
            new("quoted value", WordQuoteType.Quotes, startsFrom: 52)
        }));
    }

    [Test]
    public void Parser_EmptyLine()
    {
        var parser = new LineParser();
        var retVal = parser.ParseLine(string.Empty);

        Assert.That(retVal, Is.Empty);
    }

    [Test]
    public void Parser_NoEscapeOutsideQuote()
    {
        var parser = new LineParser();
        var retVal = parser.ParseLine("Hey\\Hey");

        Assert.That(retVal[0], Is.EqualTo(new SyntaxWord("Hey\\Hey", startsFrom: 0)));
    }

    [Test]
    public void Parser_LinePosition()
    {
        var parser = new LineParser();
        var retVal = parser.ParseLine("Hello World");
        Assert.Multiple(() =>
        {
            Assert.That(retVal, Has.Count.EqualTo(2));
            Assert.That(retVal[0], Is.EqualTo(new SyntaxWord("Hello", startsFrom: 0)));
            Assert.That(retVal[1], Is.EqualTo(new SyntaxWord("World", startsFrom: 6)));
        });
    }

    [Test]
    public void Parser_DoubleQuote_Quote()
    {
        var parser = new LineParser();
        var retVal = parser.ParseLine("\"Hello World!\"");
        Assert.That(retVal, Has.One.EqualTo(new SyntaxWord("Hello World!", WordQuoteType.Quotes, 1)));
    }
    
    [Test]
    public void Parser_Parentheses_Quote()
    {
        var parser = new LineParser();
        var retVal = parser.ParseLine("(say Hello World)");
        Assert.That(retVal, Has.One.EqualTo(new SyntaxWord("say Hello World", WordQuoteType.Parentheses, 1)));
    }
}
