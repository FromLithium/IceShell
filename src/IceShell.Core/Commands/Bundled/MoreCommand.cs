// Copyright (C) WithLithum & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Core.Commands.Bundled;

using IceShell.Core.Api;
using NexusKrop.IceShell.Core.CLI;
using System;
using System.Collections.Generic;

[CommandInfo("more")]
internal class MoreCommand : IShellCommand
{
    public CommandResult Execute(IShell shell, ICommandExecutor executor, ExecutionContext context)
    {
        if (context.Retrieval == null)
        {
            return CommandResult.WithError(CommandErrorCode.RequiresPipelineInput);
        }

        var bufferHeight = Console.BufferHeight;
        var bufferWidth = Console.BufferWidth;

        var list = new List<string>(bufferHeight);
        string? makeNext = null;

        // Cut the string into lines.
        while (true)
        {
            string? line = makeNext ?? context.Retrieval.ReadLine();

            if (line == null)
            {
                break;
            }

            makeNext = null;

            if (line.Length > bufferWidth)
            {
                makeNext = line[(bufferWidth - 1)..];
                line = line[..(bufferWidth - 1)];
            }

            list.Add(line);
        }

        var page = 0;
        var lastPage = false;
        var declLine = bufferHeight - 1;
        var moreDisplayed = false;

        while (!lastPage)
        {
            var beginAtLine = declLine * page;
            var endAtLine = declLine * (page + 1);

            if (endAtLine > list.Count)
            {
                endAtLine = list.Count;
                lastPage = true;
            }

            if (moreDisplayed)
            {
                moreDisplayed = false;
                Console.CursorTop--;

                ConsoleOutput.WipeEntireLine();
            }

            for (int i = beginAtLine; i < endAtLine; i++)
            {
                Console.WriteLine(list[i]);
            }

            if (!lastPage)
            {
                Console.Write("More...");
                moreDisplayed = true;
                Console.ReadKey(true);
                page++;
            }

            Console.WriteLine();
        }

        return CommandResult.Ok();
    }
}
