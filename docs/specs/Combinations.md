# Command combinators

The key words "MUST", "MUST NOT", "REQUIRED", "SHALL", "SHALL NOT", "SHOULD", "SHOULD NOT",
"RECOMMENDED", "NOT RECOMMENDED", "MAY", and "OPTIONAL" in this document are to be interpreted as
described in BCP 14 [RFC2119] [RFC8174] when, and only when, they appear in all capitals, as shown
here.

## Terminology

- _command segment_: A single command or a [pipe segment](#pipe-).

## And (`&&`)

```bat
command1 && command2
```

Executes the command segment to the right of the combinator only if the command segment to the left
finished with exit code of `0`.

Given the example above, the dispatcher MUST:

- Execute `command1` and wait for it to finish.
- Execute `command2` only if the exit code of `command1` is zero.

## Or (`||`)

```bat
command1 && command2
```

Executes the command segment to the right of the combinator only if the command segment to the left
finished with a non-zero exit code.

Given the example above, the dispatcher MUST:

- Execute `command1` and wait for it to finish.
- Execute `command2` only if the exit code of `command1` is not zero.

## Pipe (`|`)

```bat
command1 | command2
```

A _pipe segment_ is two or more command lines separated by a pipe symbol (`|`).

The dispatcher invokes a pipe segment by starting the commands concurrently, and then, for each
pipe symbol, redirects the standard output of the program to its left to the program to its right.

For example, given:

```bat
command1 | command2 | command3
```

The dispatcher would:

- Start `command1` with only `stdout` redirected.
- Start `command2` and `command3` with both `stdin` and `stdout` redirected.
- Start asynchronous pumping for `command1` to `command2` and `command2` to `command3`.
  - For example, for `command1` and `command2`:
    - Copy stdout of `command1` asynchronously to stdin of `command2`.
    - Once the stdout of `command` finished, close stdin of `command2`.
- Wait for all three commands to exit.

If an internal command is encountered, the internal command SHOULD NOT be executed within the same
shell process. A new instance of the shell program must be started.

## References

- [`cmd` Remarks](https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/cmd#remarks)
