// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Tests.CommandTests;

using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.Commands;
using IceShell.Core.Commands.Bundled;
using IceShell.Core.Commands.Simple;
using IceShell.Core.Settings;
using IceShell.Parsing;
using IceShell.Parsing.Util;
using IceShell.Tests.Utilities;
using NSubstitute;

public class SimpleCommandDispatcherTests
{
    [Fact]
    public void Execute_InternalCommand_ReceivesArguments()
    {
        var dispatcher = new SimpleCommandDispatcher();
        IReadOnlyList<SyntaxWord>? received = null;

        dispatcher.Register("test", (_, _, args) =>
        {
            received = args;
            return 42;
        });

        var words = new[]
        {
            new SyntaxWord("test"),
            new SyntaxWord("hello"),
            new SyntaxWord("world")
        };

        var exitCode = dispatcher.Execute(Substitute.For<IShell>(),
            Substitute.For<ICommandDocument>(),
            words);

        Assert.Multiple(
            () => Assert.Equal(42, exitCode),
            () => Assert.NotNull(received),
            () => Assert.Equal(2, received!.Count),
            () => Assert.Equal("hello", received![0].Content),
            () => Assert.Equal("world", received![1].Content));
    }

    [Fact]
    public void Execute_InternalCommandThrows_Returns255()
    {
        var dispatcher = new SimpleCommandDispatcher();
        dispatcher.Register("test", (_, _, _) => throw new InvalidOperationException("Test"));

        var exitCode = dispatcher.Execute(Substitute.For<IShell>(),
            Substitute.For<ICommandDocument>(),
            [new SyntaxWord("test")]);

        Assert.Equal(255, exitCode);
    }

    [Fact]
    public void Execute_Alias_ExpandsArguments()
    {
        var dispatcher = new SimpleCommandDispatcher();
        IReadOnlyList<SyntaxWord>? received = null;

        dispatcher.Register("test", (_, _, args) =>
        {
            received = args;
            return 0;
        });
        dispatcher.SetAlias("t", "test hello");

        var exitCode = dispatcher.Execute(Substitute.For<IShell>(),
            Substitute.For<ICommandDocument>(),
            [new SyntaxWord("t"), new SyntaxWord("world")]);

        Assert.Multiple(
            () => Assert.Equal(0, exitCode),
            () => Assert.NotNull(received),
            () => Assert.Equal(2, received!.Count),
            () => Assert.Equal("hello", received![0].Content),
            () => Assert.Equal("world", received![1].Content));
    }

    [Fact]
    public void Execute_QuotedCommandName_DoesNotUseInternalCommand()
    {
        var dispatcher = new SimpleCommandDispatcher();
        var called = false;

        dispatcher.Register("test", (_, _, _) =>
        {
            called = true;
            return 0;
        });

        var exitCode = dispatcher.Execute(Substitute.For<IShell>(),
            Substitute.For<ICommandDocument>(),
            [new SyntaxWord("test", WordQuoteType.Quotes)]);

        Assert.Multiple(
            () => Assert.False(called),
            () => Assert.NotEqual(0, exitCode));
    }

    [Fact]
    public void Execute_DotPrefixedName_DoesNotUseInternalCommand()
    {
        var dispatcher = new SimpleCommandDispatcher();
        var called = false;

        dispatcher.Register(".test", (_, _, _) =>
        {
            called = true;
            return 0;
        });

        var exitCode = dispatcher.Execute(Substitute.For<IShell>(),
            Substitute.For<ICommandDocument>(),
            [new SyntaxWord(".test")]);

        Assert.Multiple(
            () => Assert.False(called),
            () => Assert.NotEqual(0, exitCode));
    }

    [UnixFact]
    public void Execute_ExternalCommand_PassesArgumentList()
    {
        var script = Path.GetTempFileName();
        var output = Path.GetTempFileName();

        try
        {
            File.WriteAllText(script,
                "#!/bin/sh\nout=\"$1\"\nshift\nprintf '%s\\n' \"$@\" > \"$out\"\n");
#pragma warning disable CA1416
            File.SetUnixFileMode(script,
                UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
#pragma warning restore CA1416

            var dispatcher = new SimpleCommandDispatcher();
            var exitCode = dispatcher.Execute(Substitute.For<IShell>(),
                Substitute.For<ICommandDocument>(),
                [
                    new SyntaxWord(script),
                    new SyntaxWord(output),
                    new SyntaxWord("hello world"),
                    new SyntaxWord("second")
                ]);

            Assert.Multiple(
                () => Assert.Equal(0, exitCode),
                () => Assert.Equal(["hello world", "second"], File.ReadAllLines(output)));
        }
        finally
        {
            File.Delete(script);
            File.Delete(output);
        }
    }

    [Fact]
    public void Shell_ExpandsEnvironmentVariablesInWords()
    {
        const string variableName = "ICESHELL_SIMPLE_V2_TEST";
        Environment.SetEnvironmentVariable(variableName, "expanded");

        try
        {
            var dispatcher = new SimpleCommandDispatcher();
            string? received = null;

            dispatcher.Register("test", (_, _, args) =>
            {
                received = args[0].Content;
                return 0;
            });

            var shell = new Shell(new ShellSettings
            {
                ExpandVariablesForAllValues = true
            }, dispatcher);
            var result = shell.Execute($"test %{variableName}%");

            Assert.Multiple(
                () => Assert.Equal(0, result.ExitCode),
                () => Assert.Equal("expanded", received));
        }
        finally
        {
            Environment.SetEnvironmentVariable(variableName, null);
        }
    }

    [Fact]
    public void Shell_DoesNotExpandVariablesWhenDisabled()
    {
        const string variableName = "ICESHELL_SIMPLE_V2_NO_EXPAND_TEST";
        Environment.SetEnvironmentVariable(variableName, "expanded");

        try
        {
            var dispatcher = new SimpleCommandDispatcher();
            string? received = null;

            dispatcher.Register("test", (_, _, args) =>
            {
                received = args[0].Content;
                return 0;
            });

            var shell = new Shell(new ShellSettings
            {
                ExpandVariablesForAllValues = false
            }, dispatcher);
            var result = shell.Execute($"test %{variableName}%");

            Assert.Multiple(
                () => Assert.Equal(0, result.ExitCode),
                () => Assert.Equal($"%{variableName}%", received));
        }
        finally
        {
            Environment.SetEnvironmentVariable(variableName, null);
        }
    }

    [Fact]
    public void Shell_SetCommand_PreservesVariableName()
    {
        const string variableName = "ICESHELL_SIMPLE_V2_SET_TEST";
        Environment.SetEnvironmentVariable(variableName, null);

        try
        {
            var dispatcher = new SimpleCommandDispatcher();
            DefaultCommands.Register(dispatcher);

            var shell = new Shell(new ShellSettings
            {
                ExpandVariablesForAllValues = true
            }, dispatcher);
            var result = shell.Execute($"set %{variableName}% value");

            Assert.Multiple(
                () => Assert.Equal(0, result.ExitCode),
                () => Assert.Equal("value", Environment.GetEnvironmentVariable(variableName)));
        }
        finally
        {
            Environment.SetEnvironmentVariable(variableName, null);
        }
    }

    [Fact]
    public void AliasCommand_CreatesAndInvokesAlias()
    {
        var dispatcher = new SimpleCommandDispatcher();
        IReadOnlyList<SyntaxWord>? received = null;

        dispatcher.Register("test", (_, _, args) =>
        {
            received = args;
            return 0;
        });
        AliasCommand.RegisterTo(dispatcher);

        var shell = new Shell(new ShellSettings(), dispatcher);

        Assert.Equal(0, shell.Execute("alias t test hello").ExitCode);
        Assert.Equal(0, shell.Execute("t world").ExitCode);

        Assert.Multiple(
            () => Assert.NotNull(received),
            () => Assert.Equal(2, received!.Count),
            () => Assert.Equal("hello", received![0].Content),
            () => Assert.Equal("world", received![1].Content));
    }

    [Fact]
    public void AliasCommand_PreservesQuotedArguments()
    {
        var dispatcher = new SimpleCommandDispatcher();
        IReadOnlyList<SyntaxWord>? received = null;

        dispatcher.Register("test", (_, _, args) =>
        {
            received = args;
            return 0;
        });
        AliasCommand.RegisterTo(dispatcher);

        var shell = new Shell(new ShellSettings(), dispatcher);

        Assert.Equal(0, shell.Execute("alias t test \"hello world\"").ExitCode);
        Assert.Equal(0, shell.Execute("t").ExitCode);

        Assert.Multiple(
            () => Assert.NotNull(received),
            () => Assert.Single(received!),
            () => Assert.Equal("hello world", received![0].Content),
            () => Assert.Equal(WordQuoteType.Quotes, received![0].QuoteType));
    }
}
