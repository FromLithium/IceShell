# Command system

This is a breakdown of the third iteration of the command processing system, currently used as of
14 November 2026.

## Command registration

The `RegistryCommandDispatcher`, the default dispatcher of the shell, maintains a list of commands.

Each command is registered to the dispatcher as a `CommandRegistryEntry`. Each entry contains a
list of their _arguments_ and _options_, and configures how it is parsed by the command parser.

## Processing input and parsing

The shell retrieves input from a line editor, currently `ReadLine.Reboot` (now archived and made
deprecated in favour of an extremely heavy library `Terminaux`). The shell validates this input and
pass it to an `IRegistryCommandDispatcher` implementation (by default, `RegistryCommandDispatcher`)
to parse, and then pass the parse result, `CommandSection` or `CommandSectionCompound`, to the same
dispatcher to execute.

The parsing process involves running `WordBreaker` (a _lexer_) and `CommandParser` (a _parser_) to
break the input down to an AST regardless of the kind of the command. The parser is _aware_ if a
command exists or not, and any command not existing in the dispatcher is parsed as an _external_
command and everything that do is parsed as an _internal_ command.

## Execution

The command dispatcher differentiates internal and external commands through the parse result.

Each of the following sequences have two different versions for non-piped and piped execution.

### Internal command

If the command is _internal_ (a shell built-in), the dispatcher assembles a new `CommandContext`
and passes the context along with the next action to execute and the next command to feed pipe
into.

Commands may pass the result of the execution, `CommandResult`, back to the dispatcher by returning
one, or by throwing `CommandInterruptException`. Either way, the dispatcher will pass the result to
its caller as a return value.

### External command

If the command is _external_ (an executable program), the dispatcher performs the following things
in order to assemble a `ProcessStartInfo`:

1. Converts the AST argument list into a list of arguments to pass to the program.
2. Search for the executable in `PATH`.
   - If the requested executable cannot be found, `CommandFormatException` is thrown to report a
     missing command error.
3. Expand environment variables in command arguments.
4. Configure the working directory and other settings for the `StartInfo`.

The external program will then be executed. The dispatcher will block the main thread to wait for
the program to exit, and then assemble a new `CommandResult` from the exit code of the program.

## Transferring data

### Redirection

The `CommandResult` returned by each command may additionally return an `OutStream`. When this is
provided, the dispatcher copies the output from `OutStream` to the input of the next command.

## Problems

The primary problem of the "registry" command system is **over-complication**. IceShell suffers
from scope creep to the point it was no longer sustainable at the end of 2023.

Three design failures can be found from the command system itself.

The first design failure is that it shifted the responsibility of parsing commands from the
commands themselves to the dispatcher — a problem inherited from the _complex_ command
system. This makes external command invocation just as complex as internal command invocation, and
also contributes to making the dispatcher routine far more error-prone.

The second design failure is that piping and redirections were _not_ implemented by redirecting
standard streams but introducing a `TextWriter` and `NextAction` as abstractions for commands to
consume immediately. This contributes to both the complexity of both the dispatcher and command
implementations.

Finally, the third design failure is that, ironically, the transmission of failure state is overly
complex. Shell builtins communicated error state _not only_ via returned exit codes, but also via
_error codes_, _exceptions_ and _messages_.

In effect, the failures made commands _inherent programming constructs_ like in PowerShell (which
is an _object shell_ designed to interact with BCL), but IceShell **is a text shell** and never
needed such complex internal state transfers and command system.

## History

Throughout the history of IceShell, three distinct command systems were developed to accommodate
the needs of the project.

### "Simple" command system

The first command system is known as the _simple_ command system. Each command is a class that
implements `ICommand`, which takes an instance of `Shell` and an array of string arguments for
executions.

Additionally, each implementation class have an attribute (`CommandAttribute`) that declares the
name and the number of arguments the command takes.

Only four builtin commands were implemented in this system, and none of them implemented any
support for complex arguments, such as options. The last commit to feature this command system was
`93c352d29`, the second commit in Git history.

### "Complex" command system

With more complex commands planned for inclusion, the need for structured command argument parsing
resulted in the second command system, called the _complex_ command system.

This command system is an iteration of the previous command system concept, with a brand new
structured command parsing system built right into the command dispatching system.

As IceShell envisioned for modularisation, the need for another new command system surfaces as the
"complex" system relied on reflection to discover commands in each module.

### "Registry" command system

The "registry" system, the one described in detail in this document, inherited much of the parsing
and other pieces of infrastructure from the "complex" system.

Different from the "complex" system is that it treats each command as a delegate associated with an
entry, which removed the need for reflective discovery of commands.

With this system, modules programmatically register commands and feeds them into the dispatcher.

## References

- [`CommandContext`](../../src/IceShell.Core/Api/Commands/CommandContext.cs)
- [`CommandRegistryEntry`](../../src/IceShell.Core/Commands/Registry/CommandRegistryEntry.cs)
- [`CommandResult`](../../src/IceShell.Core/Api/CommandResult.cs)
- [`CommandSectionCompound`](../../src/IceShell.Core/Commands/CommandSectionCompound.cs)
- [`IRegistryCommandDispatcher`](../../src/IceShell.Core/Api/Commands/IRegistryCommandDispatcher.cs)
(interface)
- [`RegistryCommandDispatcher`](../../src/IceShell.Core/Commands/Registry/RegistryCommandDispatcher.cs) (implementation)
- [`Shell`](../../src/IceShell.Core/Shell.cs) (implementation)
