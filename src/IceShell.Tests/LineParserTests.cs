// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests;

using IceShell.Parsing;

public class LineParserTests
{
    [Test]
    public void BasicParseTest()
    {
        var parser = new LineParser();
        var retVal = parser.ParseLine("command /F /V:\"value in prop\" /Q:varr normal_value \"quoted value\"");

        Assert.That(retVal, Is.EqualTo(new SyntaxStatement[]
        {
            new("command", false, -1),
            new("/F", false, -1),
            new("/V:", false, -1),
            new("value in prop", true, -1),
            new("/Q:varr", false, -1),
            new("normal_value", false, -1),
            new("quoted value", true, -1)
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

        Assert.That(retVal[0], Is.EqualTo(new SyntaxStatement("Hey\\Hey", false, -1)));
    }

    [Test]
    public void Parser_LinePosition()
    {
        var parser = new LineParser();
        var retVal = parser.ParseLine("Hello World");
        Assert.Multiple(() =>
        {
            Assert.That(retVal.Count, Is.EqualTo(2));
            Assert.That(retVal[0], Is.EqualTo(new SyntaxStatement("Hello", false, 0)));
            Assert.That(retVal[1], Is.EqualTo(new SyntaxStatement("World", false, 6)));
        });
    }
}
