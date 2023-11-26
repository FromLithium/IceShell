// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests.Syntax;

using IceShell.Core.Syntax;
using IceShell.Parsing;
using IceShell.Parsing.Util;

public class WordBreakerTests
{
    [Test]
    public void RunParse_TwoNonQuotedWithSpace()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("hello world");
        
        Assert.Multiple(() =>
        {
           Assert.That(result, Has.Count.EqualTo(2));
           Assert.That(result[0], Is.EqualTo(new SyntaxWord("hello", WordQuoteType.None, 0)));
           Assert.That(result[1], Is.EqualTo(new SyntaxWord("world", WordQuoteType.None, 6)));
        });
    }

    [Test]
    public void RunParse_EmptyWord()
    {
        var breaker = new CommandWordBreaker();
        
        Assert.That(breaker.RunParse(""), Is.Empty);
    }

    #region Pair Quotes
    [Test]
    public void RunParse_OnePairQuote_Quote()
    {
        var breaker = new CommandWordBreaker();
        
        Assert.That(breaker.RunParse("\"word\""),
            Has.One.EqualTo(new SyntaxWord("word", WordQuoteType.Quotes, 0)));
    }
    
    [Test]
    public void RunParse_TwoPairQuote_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("\"hello\" \"world\"");
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0], Is.EqualTo(new SyntaxWord("hello", WordQuoteType.Quotes, 0)));
            Assert.That(result[1], Is.EqualTo(new SyntaxWord("world", WordQuoteType.Quotes, 8)));
        });
    }
    
    [Test]
    public void RunParse_TwoPairQuoteWithOneWordInMiddle_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("\"hello\" middle \"world\"");
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0], Is.EqualTo(new SyntaxWord("hello", WordQuoteType.Quotes, 0)));
            Assert.That(result[1], Is.EqualTo(new SyntaxWord("middle", WordQuoteType.None, 8)));
            Assert.That(result[2], Is.EqualTo(new SyntaxWord("world", WordQuoteType.Quotes, 15)));
        });
    }
    
        [Test]
    public void RunParse_TwoPairQuotesWithOneWordBefore_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("before \"hello\" \"world\"");
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0], Is.EqualTo(new SyntaxWord("before", WordQuoteType.None, 0)));
            Assert.That(result[1], Is.EqualTo(new SyntaxWord("hello", WordQuoteType.Quotes, 7)));
            Assert.That(result[2], Is.EqualTo(new SyntaxWord("world", WordQuoteType.Quotes, 15)));
        });
    }
    
    
    [Test]
    public void RunParse_TwoPairQuotesWithOneWordAfter_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("\"hello\" \"world\" after");
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0], Is.EqualTo(new SyntaxWord("hello", WordQuoteType.Quotes, 0)));
            Assert.That(result[1], Is.EqualTo(new SyntaxWord("world", WordQuoteType.Quotes, 8)));
            Assert.That(result[2], Is.EqualTo(new SyntaxWord("after", WordQuoteType.None, 16)));
        });
    }
    
    [Test]
    public void RunParse_TwoPairQuotesWithOneWordBeforeAndInMiddle_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("before \"hello\" middle \"world\"");
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(4));
            Assert.That(result[0], Is.EqualTo(new SyntaxWord("before", WordQuoteType.None, 0)));
            Assert.That(result[1], Is.EqualTo(new SyntaxWord("hello", WordQuoteType.Quotes, 7)));
            Assert.That(result[2], Is.EqualTo(new SyntaxWord("middle", WordQuoteType.None, 15)));
            Assert.That(result[3], Is.EqualTo(new SyntaxWord("world", WordQuoteType.Quotes, 22)));
        });
    }
    
    [Test]
    public void RunParse_TwoPairQuotesWithOneWordBeforeAfterAndInMiddle_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("before \"hello\" middle \"world\" after");
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(5));
            Assert.That(result[0], Is.EqualTo(new SyntaxWord("before", WordQuoteType.None, 0)));
            Assert.That(result[1], Is.EqualTo(new SyntaxWord("hello", WordQuoteType.Quotes, 7)));
            Assert.That(result[2], Is.EqualTo(new SyntaxWord("middle", WordQuoteType.None, 15)));
            Assert.That(result[3], Is.EqualTo(new SyntaxWord("world", WordQuoteType.Quotes, 22)));
            Assert.That(result[4], Is.EqualTo(new SyntaxWord("after", WordQuoteType.None, 30)));
        });
    }
    #endregion
    
    #region Parentheses
    [Test]
    public void RunParse_OneParentheses_Quote()
    {
        var breaker = new CommandWordBreaker();
        
        Assert.That(breaker.RunParse("(word)"),
            Has.One.EqualTo(new SyntaxWord("word", WordQuoteType.Parentheses, 0)));
    }
    
    [Test]
    public void RunParse_TwoParentheses_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("(hello) (world)");
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0], Is.EqualTo(new SyntaxWord("hello", WordQuoteType.Parentheses, 0)));
            Assert.That(result[1], Is.EqualTo(new SyntaxWord("world", WordQuoteType.Parentheses, 8)));
        });
    }
    
    [Test]
    public void RunParse_TwoParenthesesWithOneWordInMiddle_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("(hello) middle (world)");
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0], Is.EqualTo(new SyntaxWord("hello", WordQuoteType.Parentheses, 0)));
            Assert.That(result[1], Is.EqualTo(new SyntaxWord("middle", WordQuoteType.None, 8)));
            Assert.That(result[2], Is.EqualTo(new SyntaxWord("world", WordQuoteType.Parentheses, 15)));
        });
    }
    
    [Test]
    public void RunParse_TwoParenthesesWithOneWordBefore_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("before (hello) (world)");
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0], Is.EqualTo(new SyntaxWord("before", WordQuoteType.None, 0)));
            Assert.That(result[1], Is.EqualTo(new SyntaxWord("hello", WordQuoteType.Parentheses, 7)));
            Assert.That(result[2], Is.EqualTo(new SyntaxWord("world", WordQuoteType.Parentheses, 15)));
        });
    }
    
    
    [Test]
    public void RunParse_TwoParenthesesWithOneWordAfter_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("(hello) (world) after");
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0], Is.EqualTo(new SyntaxWord("hello", WordQuoteType.Parentheses, 0)));
            Assert.That(result[1], Is.EqualTo(new SyntaxWord("world", WordQuoteType.Parentheses, 8)));
            Assert.That(result[2], Is.EqualTo(new SyntaxWord("after", WordQuoteType.None, 16)));
        });
    }
    
    [Test]
    public void RunParse_TwoParenthesesWithOneWordBeforeAndInMiddle_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("before (hello) middle (world)");
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(4));
            Assert.That(result[0], Is.EqualTo(new SyntaxWord("before", WordQuoteType.None, 0)));
            Assert.That(result[1], Is.EqualTo(new SyntaxWord("hello", WordQuoteType.Parentheses, 7)));
            Assert.That(result[2], Is.EqualTo(new SyntaxWord("middle", WordQuoteType.None, 15)));
            Assert.That(result[3], Is.EqualTo(new SyntaxWord("world", WordQuoteType.Parentheses, 22)));
        });
    }
    
    [Test]
    public void RunParse_TwoParenthesesWithOneWordBeforeAfterAndInMiddle_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("before (hello) middle (world) after");
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(5));
            Assert.That(result[0], Is.EqualTo(new SyntaxWord("before", WordQuoteType.None, 0)));
            Assert.That(result[1], Is.EqualTo(new SyntaxWord("hello", WordQuoteType.Parentheses, 7)));
            Assert.That(result[2], Is.EqualTo(new SyntaxWord("middle", WordQuoteType.None, 15)));
            Assert.That(result[3], Is.EqualTo(new SyntaxWord("world", WordQuoteType.Parentheses, 22)));
            Assert.That(result[4], Is.EqualTo(new SyntaxWord("after", WordQuoteType.None, 30)));
        });
    }
    #endregion
    
    [Test]
    public void RunParse_OnePairQuotesWithOneWordBeforeAfterAndInMiddleAlsoOneParentheses_Quote()
    {
        var breaker = new CommandWordBreaker();
        var result = breaker.RunParse("before \"hello\" middle (world) after");
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(5));
            Assert.That(result[0], Is.EqualTo(new SyntaxWord("before", WordQuoteType.None, 0)));
            Assert.That(result[1], Is.EqualTo(new SyntaxWord("hello", WordQuoteType.Quotes, 7)));
            Assert.That(result[2], Is.EqualTo(new SyntaxWord("middle", WordQuoteType.None, 15)));
            Assert.That(result[3], Is.EqualTo(new SyntaxWord("world", WordQuoteType.Parentheses, 22)));
            Assert.That(result[4], Is.EqualTo(new SyntaxWord("after", WordQuoteType.None, 30)));
        });
    }
}