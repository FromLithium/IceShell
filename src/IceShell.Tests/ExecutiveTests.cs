namespace IceShell.Tests;

using IceShell.Core;
using System;

public class ExecutiveTests
{
    [Fact]
    public void TryParsePrompt_CurrentDate()
    {
        Assert.True(Executive.TryParsePrompt("$D", out var result));
        Assert.Equal(DateTime.Now.ToShortDateString(), result);
    }

    [Fact]
    public void TryParsePrompt_CurrentTime()
    {
        Assert.True(Executive.TryParsePrompt("$T", out var result));
        Assert.Equal(DateTime.Now.ToShortTimeString(), result);
    }

    [Fact]
    public void TryParsePrompt_CurrentDirectory()
    {
        Assert.True(Executive.TryParsePrompt("$P", out var result));
        Assert.Equal(Directory.GetCurrentDirectory(), result);
    }

    [Fact]
    public void TryParsePrompt_CurrentDirectoryAndGreaterThan()
    {
        Assert.True(Executive.TryParsePrompt("$P$G", out var result));
        Assert.Equal($"{Directory.GetCurrentDirectory()}>", result);
    }

    [Fact]
    public void TryParsePrompt_CurrentDirectoryAndGreaterThanWithColon()
    {
        Assert.True(Executive.TryParsePrompt("$P$G:", out var result));
        Assert.Equal($"{Directory.GetCurrentDirectory()}>:", result);
    }

    [Fact]
    public void TryParsePrompt_CurrentDirectoryWithRandomSymbols()
    {
        Assert.True(Executive.TryParsePrompt("$P$G:(())", out var result));
        Assert.Equal($"{Directory.GetCurrentDirectory()}>:(())", result);
    }

    [Fact]
    public void TryParsePrompt_Fail_PlaceHolderNeverEnds()
    {
        Assert.False(Executive.TryParsePrompt("$", out _));
    }

    [Fact]
    public void TryParsePrompt_Fail_InvalidPlaceholder()
    {
        Assert.False(Executive.TryParsePrompt("$%", out _));
    }
}
