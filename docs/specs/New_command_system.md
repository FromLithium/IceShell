# New Command System

This is the specification of a new command system for IceShell. This system is named "Simple v2".

## Terminology

- _dispatcher_: The command dispatcher, responsible for locating and invoking commands.
- _combinator_: See [Command Combinators](./Combinations.md).

## Dispatcher

The command dispatcher is responsible for the following tasks:

- Breaking down user input command line into words suitable for passing to commands.
- Locating and invoking internal and external commands.
- Handling redirections.
- In the future, handle combinators, although this can be left out in the initial implementation.

Do not reuse the old dispatcher class or interface when implementing the new dispatcher system. Use a new class and a new interface. If required, mark the old dispatcher as obsolete.

### Locating internal commands

The new system supports a **finite** set of internal commands built into the core, identified by command name.

Internal commands MUST take precedence over external commands. An external can be explicitly invoked by specifying it as a path to the executable.

The implementation would use a `switch` expression; however whatever else appropriate is accepted.

```csharp
InternalCommand? FindInternalCommand(string commandName)
{
    return commandName switch
    {
        "alias" => AliasCommand.Command,
        "echo" => EchoCommand.Command,
        // ...
        _ => null
    }
}
```

### Executing internal commands

The dispatcher executes an internal command by calling its `Action` with a list of words broken from the input command line.

The return value of the `Action` is the exit code of the internal command. If an exception is thrown by the `Action`, the exit code is set to `255` and the exception details should be printed to standard error.

### Executing external commands

See [external commands](#external-commands).

## Internal commands

An internal command is declared in code as an instance of `InternalCommand`.

The `InternalCommand` record is defined as follows:

```csharp
record InternalCommand
{
    string Name { get; }

    Func<IReadOnlyList<SyntaxWord>, int> Action { get; }
}
```

### Execution logic

The logic of a command is a delegate conforming to the following signature:

```csharp
int Execute(IReadOnlyList<SyntaxWord> args);
```

Internal commands SHOULD NOT throw exceptions.

## External commands

External commands are executable programs within the current directory, or a directory in `PATH`.

File paths, when used as a command name, MUST be interpreted as external commands. If configured,
a command name not corresponding to an internal command can also be interpreted as an external
command.

Arguments MUST be passed to external commands by using the Argument List, not the argument string. The content of each word is passed as one entry to the external command, disregarding the quote status.

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
