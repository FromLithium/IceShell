# New Command System

This is the specification of a new command system for IceShell. This system is named "Simple v2".

## Terminology

- _combinator_: Pipes, redirections and conditions. See [Command Combinators](./Combinations.md) for details.
- _command name_: The first word of a command line (e.g. if `foo bar`, then `foo` is the command name).
- _dispatcher_: The command dispatcher, responsible for locating and invoking commands.
- _shell_: The shell (`IShell`) implementation, responsible for taking user input.
- _word_: A `SyntaxWord`, which is broken from user input by the word breaker.

## Shell

The shell is responsible for the following tasks:

- Retrieving user input command line.
- Break the user input down into `SyntaxWord` list by invoking the word breaker (lexer).
- Resolving environment variables and other special notation in the word list.
- Passing the word list, shell and document to the dispatcher.

## Dispatcher

The command dispatcher is responsible for the following tasks:

- Locating and invoking internal and external commands.
- Handling redirections.
- In the future, handle combinators, although this can be left out in the initial implementation.

Do not reuse the old dispatcher class or interface when implementing the new dispatcher system. Use a new class and a new interface. If required, mark the old dispatcher as obsolete.

### Processing word list

The dispatcher MUST retrieve the command name from the word list and determine the type of the command to search first.

The command name is the first entry in the word list.

In the following cases, the dispatcher MUST treat the command name as the name of an external command only:

- The command name word is quoted. That is, the quote type is not `None`.
- The name of the command starts with a dot (`.`).
- The name of the command contains a directory separator.

In all other cases, the dispatcher MUST treat the command name as an internal command, unless:

- There is no internal command with that name.
- The dispatcher has been configured to resolve a non-file path command name to an external command.

### Locating internal commands

The new system supports a **finite** set of internal commands built into the core, identified by command name.

Internal commands MUST take precedence over external commands. An external can be explicitly invoked by specifying it as a path to the executable.

### Executing internal commands

The dispatcher executes an internal command by calling its `Action` with a list of words broken from the input command line.

The return value of the `Action` is the exit code of the internal command. If an exception is thrown by the `Action`, the exit code is set to `255` and the exception details should be printed to standard error.

### Executing external commands

The `ExecutableSearcher` class provide implementations for locating external executables.

If the returned file is not executable, and the dispatcher is so configured, the file MAY be executed by `ShellExecute`. In this case, the process MUST NOT be waited for exit.

Arguments MUST be passed to external commands by using the Argument List, not the argument string. The content of each `SyntaxWord` is passed as one entry to the external command regardless of how the word was quoted.

The dispatcher MUST NOT add quote characters when assembling argument list.

## Internal commands

An internal command is declared in code as an instance of `InternalCommand`, which is a delegate with the following signature:

```csharp
delegate int InternalCommand(IShell shell,
    ICommandDocument document,
    IReadOnlyList<SyntaxWord> args);
```

The shell is responsible for invoking word breaker to get `args` and passing itself (`IShell`) and the `ICommandDocument` instance to the dispatcher.

Internal commands SHOULD NOT throw exceptions. The dispatcher, however, MUST treat exceptions from internal command functions as unexpected errors.

## Aliases

An alias is a user defined command that maps to a command line. For example:

```bat
alias MY_ALIAS command1 /A /B
```

An alias is executed by taking the arguments used to invoke an alias and appending it to the alias
value, then invoking the combined command as a regular command.

Aliases take priority over internal and external commands.

Given the complexity of the alias system, the alias commands should be implemented as unstructured commands.

## Compatibility

To aid transition to the new command system, a glue layer MUST be provided to commands written for the old "_registry_" command system.

A new utility class, `CommandCompat` is introduced for this purpose. This class MUST have a public method:

```csharp
InternalCommand FromRegistry(CommandRegistryEntry entry, CommandDelegate delegate);
```

This method MUST wrap the provided `delegate` and perform actions previously handled by the `RegistryCommandDispatcher`. Namely, the wrapper method MUST at least:

- Invoke structured command parsing for provided command words.
- Handle exceptions from the previous delegate.
- Convert the command result to an exit code.
- Replicate the previous behaviour of error codes.

## Other notes

- Modularisation and extensibility factors are considered feature creep for IceShell. This factor MUST NOT be considered for the new command system, and the system itself have been planned for removal.
  
## References

- [A breakdown of the old command system](../dev/Command_system.md)
