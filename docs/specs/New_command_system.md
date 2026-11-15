# New Command System

This is the specification of a new command system for IceShell. This system is named "Simple v2".

## Terminology

- _combinator_: See [Command Combinators](./Combinations.md).
- _command name_: The first word of a command line (e.g. if `foo bar`, then `foo` is the command name).
- _dispatcher_: The command dispatcher, responsible for locating and invoking commands.
- _shell_: The shell (`IShell`) implementation, responsible for taking user input.

## Shell

The shell is responsible for the following tasks:

- Retrieving user input command line.
- Break the user input down into `SyntaxWord` list by invoking the word breaker (lexer).
- Resolving environment variables and other special notation in the word list.
- Passing the word list to the dispatcher.

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

The implementation would use a `switch` expression, or if it needs to be, a hash map, depending on the amount of internal commands; however whatever else appropriate is accepted.

### Executing internal commands

The dispatcher executes an internal command by calling its `Action` with a list of words broken from the input command line.

The return value of the `Action` is the exit code of the internal command. If an exception is thrown by the `Action`, the exit code is set to `255` and the exception details should be printed to standard error.

### Executing external commands

See [external commands](#external-commands).

## Internal commands

An internal command is declared in code as an instance of `InternalCommand`, which is a delegate with the following signature:

```csharp
delegate int InternalCommand(IShell shell,
    ICommandDocument document,
    IReadOnlyList<SyntaxWord> args);
```

Internal commands SHOULD NOT throw exceptions. The dispatcher, however, MUST treat exceptions from internal command functions as unexpected errors.

## External commands

External commands are executable programs specified by name or path.

File paths, when used as a command name, MUST be interpreted as external commands. If configured, a command name not corresponding to an internal command can also be interpreted as an external command.

Arguments MUST be passed to external commands by using the Argument List, not the argument string. The content of each word is passed as one entry to the external command, disregarding the quote status.

### Locating external commands

The dispatcher MUST resolve a command name to the full path of an executable by trying the following methods in order, until one of them succeeds:

- If the command name is an absolute path, **use the absolute path**.
- If the command name is a _relative_ file path (contains file path separator or the dot '`.`' character), **resolve its full path using the current directory as the base path**.
- If the PATHEXT environment variable exists, then for each extension in PATHEXT, in order, append that extension to the command name and search for the resulting file - first in the current directory, then in each directory listed in PATH - stopping at the first file found.
- Search for a file with same name as the command name - first in the current directory, then in each directory listed in PATH - stopping at the first file found.

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

- The previous system handled some combinators but not all of the DOS combinators.
  - It is advisable that the combinator implementation is left out in the initial implementation, but there should be room for implementing combinators at a later point.
- As the customisable alias system isn't present in the old command system, it can also be left out in the initial run.
- Modularisation and extensibility factors are considered feature creep for IceShell. This factor MUST NOT be considered for the new command system, and the system itself have been planned for removal.
  
## References

- [A breakdown of the old command system](../dev/Command_system.md)
