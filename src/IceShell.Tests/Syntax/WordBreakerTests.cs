// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests.Syntax;

using IceShell.Core.Syntax;
using IceShell.Parsing;
using IceShell.Parsing.Util;

public class WordBreakerTests
{
    [Fact]
    public void RunParse_TwoNonQuotedWithSpace()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("hello world");
        
        Assert.Collection(result,
            x1 => Assert.Equal(new SyntaxWord("hello", WordQuoteType.None, 0), x1),
            x2 => Assert.Equal(new SyntaxWord("world", WordQuoteType.None, 6), x2));
    }

    [Fact]
    public void RunParse_EmptyWord()
    {
        var breaker = new CommandWordBreaker();
        
        Assert.Empty(breaker.RunParse(""));
    }

    [Fact]
    public void RunParse_Final()
    {
        var breaker = new CommandWordBreaker();
        _ = breaker.RunParse("Hello World!");
        
        Assert.Throws<InvalidOperationException>(() => breaker.RunParse("Hello Again!"));
    }

    [Fact]
    public void RunParse_Error_QuoteNeverEnd()
    {
        var breaker = new CommandWordBreaker();
        
        Assert.Throws<FormatException>(() => breaker.RunParse("\"Hello World!"));
    }
    
    [Fact]
    public void RunParse_Error_WrongRightRoundBracketInQuotePair()
    {
        var breaker = new CommandWordBreaker();
        
        Assert.Throws<FormatException>(() => breaker.RunParse("\"Hello World!)"));
    }
    
    [Fact]
    public void RunParse_ShouldPass_EscapedRightRoundBracketInQuotePair()
    {
        var breaker = new CommandWordBreaker();
        
        var exception = Record.Exception(() => breaker.RunParse("\"Hello World!\\)\""));

        Assert.Null(exception);
    }

    [Fact]
    public void RunParse_ValuedQuoteOptionTest()
    {
        var breaker = new CommandWordBreaker(true);
        var result = breaker.RunParse("/X:\"value\"");

        Assert.Collection(result,
            x1 => Assert.Equal(new SyntaxWord("/X:", WordQuoteType.None, 0), x1),
            x2 => Assert.Equal(new SyntaxWord("value", WordQuoteType.Quotes, 3), x2));
    }
    
    #region Pair Quotes
    [Fact]
    public void RunParse_OnePairQuote_Quote()
    {
        var breaker = new CommandWordBreaker();
        
        Assert.Single(breaker.RunParse("\"word\""),
            new SyntaxWord("word", WordQuoteType.Quotes, 0));        
    }
    
    [Fact]
    public void RunParse_TwoPairQuote_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("\"hello\" \"world\"");
        
        Assert.Collection(result,
            x1 => Assert.Equal(new SyntaxWord("hello", WordQuoteType.Quotes, 0), x1),
            x2 => Assert.Equal(new SyntaxWord("world", WordQuoteType.Quotes, 8), x2));
    }
    
    [Fact]
    public void RunParse_TwoPairQuoteWithOneWordInMiddle_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("\"hello\" middle \"world\"");
        
        Assert.Collection(result,
            x1 => Assert.Equal(new SyntaxWord("hello", WordQuoteType.Quotes, 0), x1),
            x2 => Assert.Equal(new SyntaxWord("middle", WordQuoteType.None, 8), x2),
            x3 => Assert.Equal(new SyntaxWord("world", WordQuoteType.Quotes, 15), x3));
    }
    
    [Fact]
    public void RunParse_TwoPairQuotesWithOneWordBefore_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("before \"hello\" \"world\"");
        
        Assert.Collection(result,
            x1 => Assert.Equal(new SyntaxWord("before", WordQuoteType.None, 0), x1),
            x2 => Assert.Equal(new SyntaxWord("hello", WordQuoteType.Quotes, 7), x2),
            x3 => Assert.Equal(new SyntaxWord("world", WordQuoteType.Quotes, 15), x3));
    }
    
    [Fact]
    public void RunParse_TwoPairQuotesWithOneWordAfter_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("\"hello\" \"world\" after");
        
        Assert.Collection(result,
            x1 => Assert.Equal(new SyntaxWord("hello", WordQuoteType.Quotes, 0), x1),
            x2 => Assert.Equal(new SyntaxWord("world", WordQuoteType.Quotes, 8), x2),
            x3 => Assert.Equal(new SyntaxWord("after", WordQuoteType.None, 16), x3));
    }
    
    [Fact]
    public void RunParse_TwoPairQuotesWithOneWordBeforeAndInMiddle_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("before \"hello\" middle \"world\"");
        
        Assert.Collection(result,
            x1 => Assert.Equal(new SyntaxWord("before", WordQuoteType.None, 0), x1),
            x2 => Assert.Equal(new SyntaxWord("hello", WordQuoteType.Quotes, 7), x2),
            x3 => Assert.Equal(new SyntaxWord("middle", WordQuoteType.None, 15), x3),
            x4 => Assert.Equal(new SyntaxWord("world", WordQuoteType.Quotes, 22), x4));
    }
    
    [Fact]
    public void RunParse_TwoPairQuotesWithOneWordBeforeAfterAndInMiddle_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("before \"hello\" middle \"world\" after");
        
        Assert.Collection(result,
            x1 => Assert.Equal(new SyntaxWord("before", WordQuoteType.None, 0), x1),
            x2 => Assert.Equal(new SyntaxWord("hello", WordQuoteType.Quotes, 7), x2),
            x3 => Assert.Equal(new SyntaxWord("middle", WordQuoteType.None, 15), x3),
            x4 => Assert.Equal(new SyntaxWord("world", WordQuoteType.Quotes, 22), x4),
            x5 => Assert.Equal(new SyntaxWord("after", WordQuoteType.None, 30), x5));
    }

    [Fact]
    public void RunParse_Error_DoubleQuoteToEndParentheses_Quote()
    {
        var breaker = new CommandWordBreaker();
        
        Assert.Throws<FormatException>(() => breaker.RunParse("(Hello World!\""));
    }
    
    [Fact]
    public void RunParse_ShouldPass_EscapedDoubleQuoteInParentheses_Quote()
    {
        var breaker = new CommandWordBreaker();
        
        var exception = Record.Exception(() => breaker.RunParse("(Hello World!\\\")"));

        Assert.Null(exception);
    }
    #endregion
    
    #region Parentheses
    [Fact]
    public void RunParse_OneParentheses_Quote()
    {
        var breaker = new CommandWordBreaker();
        
        Assert.Single(breaker.RunParse("(word)"),
            new SyntaxWord("word", WordQuoteType.Parentheses, 0));
    }
    
    [Fact]
    public void RunParse_TwoParentheses_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("(hello) (world)");
        
        Assert.Collection(result,
            x1 => Assert.Equal(new SyntaxWord("hello", WordQuoteType.Parentheses, 0), x1),
            x2 => Assert.Equal(new SyntaxWord("world", WordQuoteType.Parentheses, 8), x2));
    }
    
    [Fact]
    public void RunParse_TwoParenthesesWithOneWordInMiddle_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("(hello) middle (world)");
        
        Assert.Collection(result,
            x1 => Assert.Equal(new SyntaxWord("hello", WordQuoteType.Parentheses, 0), x1),
            x2 => Assert.Equal(new SyntaxWord("middle", WordQuoteType.None, 8), x2),
            x3 => Assert.Equal(new SyntaxWord("world", WordQuoteType.Parentheses, 15), x3));
    }
    
    [Fact]
    public void RunParse_TwoParenthesesWithOneWordBefore_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("before (hello) (world)");
        
        Assert.Collection(result,
            x1 => Assert.Equal(new SyntaxWord("before", WordQuoteType.None, 0), x1),
            x2 => Assert.Equal(new SyntaxWord("hello", WordQuoteType.Parentheses, 7), x2),
            x3 => Assert.Equal(new SyntaxWord("world", WordQuoteType.Parentheses, 15), x3));
    }
    
    
    [Fact]
    public void RunParse_TwoParenthesesWithOneWordAfter_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("(hello) (world) after");
        
        Assert.Collection(result,
            x1 => Assert.Equal(new SyntaxWord("hello", WordQuoteType.Parentheses, 0), x1),
            x2 => Assert.Equal(new SyntaxWord("world", WordQuoteType.Parentheses, 8), x2),
            x3 => Assert.Equal(new SyntaxWord("after", WordQuoteType.None, 16), x3));
    }
    
    [Fact]
    public void RunParse_TwoParenthesesWithOneWordBeforeAndInMiddle_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("before (hello) middle (world)");
        
        Assert.Collection(result,
            x1 => Assert.Equal(new SyntaxWord("before", WordQuoteType.None, 0), x1),
            x2 => Assert.Equal(new SyntaxWord("hello", WordQuoteType.Parentheses, 7), x2),
            x3 => Assert.Equal(new SyntaxWord("middle", WordQuoteType.None, 15), x3),
            x4 => Assert.Equal(new SyntaxWord("world", WordQuoteType.Parentheses, 22), x4));
    }
    
    [Fact]
    public void RunParse_TwoParenthesesWithOneWordBeforeAfterAndInMiddle_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("before (hello) middle (world) after");
        
        Assert.Collection(result,
            x1 => Assert.Equal(new SyntaxWord("before", WordQuoteType.None, 0), x1),
            x2 => Assert.Equal(new SyntaxWord("hello", WordQuoteType.Parentheses, 7), x2),
            x3 => Assert.Equal(new SyntaxWord("middle", WordQuoteType.None, 15), x3),
            x4 => Assert.Equal(new SyntaxWord("world", WordQuoteType.Parentheses, 22), x4),
            x5 => Assert.Equal(new SyntaxWord("after", WordQuoteType.None, 30), x5));
    }
    #endregion
    
    [Fact]
    public void RunParse_OnePairQuotesWithOneWordBeforeAfterAndInMiddleAlsoOneParentheses_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("before \"hello\" middle (world) after");
        
        Assert.Collection(result,
            x1 => Assert.Equal(new SyntaxWord("before", WordQuoteType.None, 0), x1),
            x2 => Assert.Equal(new SyntaxWord("hello", WordQuoteType.Quotes, 7), x2),
            x3 => Assert.Equal(new SyntaxWord("middle", WordQuoteType.None, 15), x3),
            x4 => Assert.Equal(new SyntaxWord("world", WordQuoteType.Parentheses, 22), x4),
            x5 => Assert.Equal(new SyntaxWord("after", WordQuoteType.None, 30), x5));
    }

    [Fact]
    public void RunParse_PairQuoteEscape_Quote()
    {
        var breaker = new CommandWordBreaker();
        Assert.Single(breaker.RunParse("\"\\\"Hello World!\\\"\""),
            new SyntaxWord("\"Hello World!\"", WordQuoteType.Quotes, 0));
    }
}