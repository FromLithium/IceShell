# 3. 内置命令

> **NOTE**
>
> 本节还在编写中。

本节包含了 IceShell 内置命令和默认附带的模块的详细帮助信息。

## CALL

_来自：批处理模块_

在当前上下文中执行批处理程序。如果批处理程序中任何命令失败，即立即视作批处理程序执行失败。

```bat
call <file>
```

### 参数

- **file**：欲执行的批处理文件。

### 返回代码

如果批处理程序执行成功，则返回零；否则，返回批处理程序中最后一个执行的命令的返回代码。

<!-- ======================================= -->

## CD

更改当前所在的工作目录。如果未指定欲更改为的目录，则显示当前工作目录。

```bat
cd [directory]
```

### 参数

- **directory**：_选填_。欲更改为的工作目录。

### 返回代码

任何情况下都返回零。

<!-- ======================================= -->

## CLS

清屏。

```cmd
cls
```

### 返回代码

任何情况下都返回零。

<!-- ======================================= -->

## COPY

复制一个或多个文件。

```bat
COPY [/F] <files...> <destination>
```

### 参数

- **files**：欲复制的文件。
- **destination**：目标。

### 选项

- `/F`：指定是否要覆盖目标目录中的文件同名。

### 返回代码

成功返回 `0`，失败则返回非 `0`。若要确认具体错误，请检查错误代码。

<!-- ======================================= -->

## DEL

删除一个或多个文件。

```cmd
DEL [/P] <files...>
```

### 参数

- **files**：欲删除的文件。

### 选项

- `/P`：指定是否在删除每个文件前提示用户是否确认删除。

### 返回代码

成功返回 `0`，失败则返回非 `0`。若要确认具体错误，请检查错误代码。

<!-- ======================================= -->

## DIR

列出当前目录（或指定的目录）中的文件和子目录。

```cmd
dir [directory] [/d:date-format] [/t:time-format] [/h] [/L|/U] [/B] [/S]
```

### 参数

- **directory**：_选填_。欲列出文件和子目录的目录。如不填则使用当前目录。

### 选项

- `/d`：指定日期格式。如不填则使用默认格式（`YYYY/mm/dd`）。 
- `/t`：指定时间格式。如不填则使用默认格式（`HH:mm:ss`）。
- `/h`：指定是否显示隐藏文件。Windows 上隐藏文件是被设为隐藏属性的文件，而在 *NIX 系统上则是名称以半角点号 `.` 开头的文件。
- `/L`：指定是否强制以全小写列出文件名。不能与 `/U` 同时指定。
- `/U`：指定是否强制以全大写列出文件名。不能与 `/L` 同时指定。
- `/B`：指定是否只输出文件名且不使用表格式显示。
- `/S`：指定是否在 Windows 平台上使用 DOS 兼容短文件名。

### 返回代码

成功返回 `0`，失败则返回非 `0`。若要确认具体错误，请检查错误代码。

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

Performs conditional execution, or checks for a conditions.

```bat
if [not] <condition expression>... [(then-command)] [ELSE (else-command)]
         <variable>=<value>
         FILE <file-path>
         DIR <directory-path>
         EXISTS <path>
         ERRORLEVEL <exit-code>
```

## Arguments

- **not**: If specified, this inverts the result of the condition expression.
- **condition expression**:
  - `<variable>=<value>`: Determines if the variable is equivalent to the specified value.
  - `FILE <file-path>`: Determines if the specified file exists.
    - **file-path**: The path to the file to check for existence.
  - `DIR <directory-path`: Determines if the specified directory exists.
    - **directory-path**: The path to the directory to check for existence.
  - `EXISTS <path>`: Determines if either a file or a folder exists at the specified path.
    - **path**: The path to check for existence.
- **(then-command)**: Must be wrapped in parentheses. The command to execute if the condition was met.
- **ELSE**: Specifies to execute a command if the condition did not met.
  - **(else-command)**: Required if **ELSE** was specified. Must be wrapped in parentheses. The command to execute if the condition did not met.

## Exit Code

- If a **then-command** was not specified, returns zero if the condition was met, and non-zero if the condition did not met, or on failure.
- If a **then-command** was specified, returns zero on success and non-zero on failure.

For detailed error information, check the error code. If the error code was `None`, the command succeeded.

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
|-------------|-------------------------------------------|
| `$Q`        | The equal sign (`=`)                      |
| `$$`        | The dollar sign (`$`)                     |
| `$T`        | The current time in short time format     |
| `$D`        | The current date in short date format     |
| `$P`        | The path to the current working directory |
| `$V`        | The version of IceShell application       |
| `$G`        | The greater than sign (`>`)               |
| `$L`        | The lesser than sign (`<`)                |
| `$B`        | The pipe sign (`                          |`) |
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
