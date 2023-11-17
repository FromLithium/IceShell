// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

namespace IceShell.Batching;

using IceShell.Core;
using IceShell.Core.Api;
using IceShell.Core.CLI.Languages;
using IceShell.Core.Commands;
using IceShell.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using IceShell.Core.Api.Commands;

public class BatchFile : ICommandExecutor
{
    public BatchFile(IList<CommandSectionCompound> lines, IDictionary<string, int> labels)
    {
        _lines = lines;
        _labels = labels;
    }

    private readonly IList<CommandSectionCompound> _lines;
    private readonly IDictionary<string, int> _labels;

    public bool SupportsJump => true;

    public int CurrentLine { get; private set; }

    public static BatchFile Parse(IEnumerable<string> lines, IRegistryCommandDispatcher dispatcher)
    {
        var retVal = new List<CommandSectionCompound>();
        var labels = new Dictionary<string, int>();
        var lineNum = 0;

        foreach (var line in lines)
        {
            lineNum++;

            // Double comments
            if (line.StartsWith("::"))
            {
                retVal.Add(CommandSectionCompound.Empty());
                continue;
            }

            // Label
            if (line.StartsWith(':'))
            {
                labels.Add(line[1..], lineNum);
                retVal.Add(CommandSectionCompound.Empty());
                continue;
            }

            retVal.Add(dispatcher.Parse(line));
        }

        return new(retVal, labels);
    }

    public CommandResult RunBatch(IShell shell)
    {
        while (CurrentLine < _lines.Count)
        {
            var line = _lines[CurrentLine];
            CurrentLine++;

            if (!line.Any())
            {
                continue;
            }

            var execResult = shell.Execute(line, this);

            if (execResult.ExitCode != 0)
            {
                return execResult;
            }
        }

        return CommandResult.Ok();
    }

    public void Jump(string label)
    {
        if (!_labels.TryGetValue(label, out var labelLine))
        {
            throw new CommandFormatException(string.Format(LangMessage.Get("batch_goto_no_such_label"), label));
        }

        if (labelLine > _lines.Count)
        {
            throw new InvalidOperationException("Label is not correctly parsed? Over line amount!");
        }

        CurrentLine = labelLine - 1;
    }
}
