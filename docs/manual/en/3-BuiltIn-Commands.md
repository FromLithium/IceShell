# Built-In Commands

> **NOTE**
> 
> This section is a work in progress.

This section describes all built-in commands that are available in IceShell and its default modules.

## CALL

_Part of the Batching Module_

Executes a batch program in the current execution context. If any command in the batch program fails,
then the batch program itself fails.

```bat
call <file>
```

### Arguments

- **file**: The batch program to execute.

### Exit Code

Returns zero if the batch program succeeds; otherwise returns the exit code of
the failing (last executed) command in the batch program.

<!-- ======================================= -->

## CD

Changes the current working directory. If a directory was not specified, displays the
current working directory.

```bat
cd [directory]
```

### Arguments

- **directory**: _Optional_. The directory to change to.

### Exit Code

Always succeed.

<!-- ======================================= -->

## CLS

Clears the terminal screen.

```cmd
cls
```

### Exit Code

Always succeeds.

<!-- ======================================= -->

## COPY

Copies one or more files.

```bat
COPY [/F] <files...>
```

### Arguments

- **files**: The files to move.

### Options

- `/F`: If specified, overwrites existing files at destination.

### Exit Code

Returns `0` if succeeds, non-zero otherwise. Check the message of the error for details.

<!-- ======================================= -->

## DEL

Deletes one or more files.

```cmd
DEL [/P] <files...>
```

### Arguments

- **files**: A list of files to delete.

### Options

- `/P`: If specified, prompts user to confirm before each deletion.

### Exit Code

Returns zero on success and non-zero on failure. For detailed error information, check the exception message.

<!-- ======================================= -->

## DIR

Lists the contents of the current or specified directory.

```cmd
dir [directory] [/d:date-format] [/t:time-format] [/h] [/L|/U] [/B] [/S]
```

### Arguments

- **directory**: _Optional_. The directory to list. If unspecified, lists the current directory.

### Options

- `/d`: Specifies the date format. If unspecified, uses the default format (`YYYY/mm/dd`). 
- `/t`: Specifies the time format. If unspecified, uses the default format (`HH:mm:ss`).
- `/h`: If specified, displays hidden files and directories. On Windows, hidden means being attributed with "hidden" attribute, while on *NIX systems, are ones with name starting with a dot `.`.
- `/L`: If specified, all file names will be presented in lower case. Cannot be specified together with `/U`.
- `/U`: If specified, all file names will be presented in upper case. Cannot be specified together with `/L`.
- `/B`: If specified, only file names will be displayed and the grid formatting will be disabled.
- `/S`: If specified on Windows, the DOS-compatibility short name format will be used.

### Exit Code

Returns zero on success and non-zero on failure. For detailed error information, check the exception message.

<!-- ======================================= -->

## ECHO

Prints the specified message or redirected output from last command to standard output.

```cmd
echo [message]
```

### Arguments

- **message**: _Optional_. The message to display.

### Exit Code

Always succeeds.

<!-- ======================================= -->

## EXIT

Exits the IceShell. After this command succeeds, IceShell will terminate.

```cmd
exit
```

### Exit Code

Always succeeds.

<!-- ======================================= -->

## GOTO

_Part of the Batching Module_

If supported, skip to the specified label.

```bat
GOTO <label>
```

### Arguments

- **label**: The label to skip to.

### Errors

- **12 - OperationNotSupported**: The current context does not support skipping.

### Exit Code

Returns `0` if succeeds, non-zero otherwise.

<!-- ======================================= -->

## HELP

Displays help information.

```cmd
help [command]
```

### Arguments

- **command**: _Optional._ The command to show help message. If unspecified, displays a list of commands and their description.

### Exit Code

Returns zero on success and non-zero on failure. For detailed error information, check the exception message.

<!-- ======================================= -->

## IF

See [IF](3-BuiltIn-Commands/IF.md).

<!-- ======================================= -->

## MKDIR

Creates a directory or a tree of directories. The user must be authorised to create the file,
and the directories shall not exist.

```bat
mkdir <directory>
```

### Arguments

- **directory**: The directory or tree of directories to create.

### Errors

- 
  
  ### Exit Code

Returns `0` if the command succeeds, non-zero otherwise. Check the error code for detailed error.

<!-- ======================================= -->

## MKFILE

Creates a file. The user must be authorised to create the file, and the file specified should not be exist.

```bat
mkfile <file>
```

### Arguments

- **file**: _Required_. The file to create.

### Exit Code

Returns `0` if the command succeeds, non-zero otherwise. Check the error code for detailed error.

<!-- ======================================= -->

## MORE

Displays a long chunk of text across multiple pages. The dimensions of each page is determined based on the text buffer size.

This command requires pipeline input.

### Errors

- **16 - RequiresPipelineInput**: Pipeline input is required but not supplied.

### Exit Code

Returns `0` if the command succeeds, non-zero otherwise. Check the error code for detailed error.

<!-- ======================================= -->

## MOVE

Move or rename one or more files.

```cmd
move [/F] <files...> <destination>
```

### Arguments

- **files**: The files to move.

### Options

- `/F`: If specified, overwrites existing files at destination.

### Errors

- **6 - BadDirectory**: The destination directory does not exist.
- **7 - SingleDestinationMultiSource**: There are multiple (matches of) source files (pattern), but the destination is not a folder.
- **8 - SyntaxError**: No file or destination is provided.

### Exit Code

Returns `0` if the command succeeds, non-zero otherwise. Check the error code for detailed error.

<!-- ======================================= -->

## PROMPT

Displays or modified the current prompt.

```bat
PROMPT [prompt]
```

### Arguments

- **prompt**: _Optional._ The prompt template to change to.

### Remarks

The prompt template is a string with placeholders:

| Placeholder | Value                                     |
| ----------- | ----------------------------------------- |
| `$Q`        | The equal sign (`=`)                      |
| `$$`        | The dollar sign (`$`)                     |
| `$T`        | The current time in short time format     |
| `$D`        | The current date in short date format     |
| `$P`        | The path to the current working directory |
| `$V`        | The version of IceShell application       |
| `$G`        | The greater than sign (`>`)               |
| `$L`        | The lesser than sign (`<`)                |
| `$B`        | The pipe sign (`                          |
| `$_`        | System new-line sequence                  |

### Errors

- **10 - BadArgument**: The prompt template format is invalid.

### Exit Code

Returns `0` if the command succeeds, non-zero otherwise. Check the error code for detailed error.

<!-- ======================================= -->

## REM

Does nothing.

```bat
rem [any argument...]
```

### Exit Code

Always succeed.

<!-- ======================================= -->

## SET

Gets or sets an environment variable.

```bat
SET <name> [value]
```

### Arguments

- **name**: The name of the environment variable.
- **value**: _Optional_. The value to set to. If not specified, outputs the value of the variable.

### Errors

- **8 - SyntaxError**: The _name_ argument is empty.

### Exit Code

Returns `0` if the command succeeds, non-zero otherwise. Check the error code for detailed error.

<!-- ======================================= -->

## START

Starts the specified file via the default application associated for the type of the file, or if the file is a program, starts the program and immediately returns without waiting for its exit or handing the console to it.

```bat
START <file>
```

### Exit Code

Returns `0` if the command succeeds, non-zero otherwise. Check the message of the error for details.

<!-- ======================================= -->

## TYPE

Reads the contents of a text file to the console or other commands.

```bat
TYPE <file>
```

### Arguments

- **file**: The name of the file to read.

### Errors

This command may return any of the following error codes:

- **8 - SyntaxError**: The _file_ argument is empty.
- **9 - BadFile**: The specified _file_ does not exist.

### Exit Code

Returns `0` if the command succeeds, non-zero otherwise. Check the error code for detailed error.

<!-- ======================================= -->

## VER

Displays the IceShell version along with some Operating System information.

```bat
VER
```

### Options

- `/C`: Shows the copyright notice.
- `/S`: Shows basic system information.

### Exit Code

Always succeed.

---

Copyright (C) 2023 IceShell contributors.

Permission is granted to copy, distribute and/or modify this document under the terms of the GNU Free Documentation License, Version 1.3 or any later version published by the Free Software Foundation; with no Invariant Sections, no Front-Cover Texts, and no Back-Cover Texts.

A copy of the license is included in the file named "LICENSE-DOCS", in the `docs/manual` directory of the project repository.
