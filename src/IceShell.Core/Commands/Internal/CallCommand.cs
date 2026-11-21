// SPDX-FileCopyrightText: 2023, 2026 WithLithum
// SPDX-License-Identifier: GPL-3.0-or-later

using IceShell.Core.Api;
using IceShell.Core.Api.Commands;
using IceShell.Core.Commands.Registry;
using IceShell.Core.Execution;

namespace IceShell.Core.Commands.Internal;

[CommandInfo("call")]
internal static class CallCommand
{
    internal static void RegisterTo(IRegistryCommandDispatcher dispatcher)
    {
        new CommandBuilder("call", Execute)
            .ValueParameter("file")
                .Position(0)
                .Required()
                .NoExpandVariables()
                .Build()
            .Build(dispatcher);
    }

    private static CommandResult Execute(IShell shell, ICommandDocument document, CommandContext context)
    {
        var full = Path.GetFullPath(context.GetValue("name")!);

        CommandChecks.FileExists(full);

        return BatchFile.Parse(File.ReadAllLines(full)).RunBatch(shell);
    }
}
