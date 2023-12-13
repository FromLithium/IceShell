# IF

| Name   | Description   |
|--------|---------------|
| Module | IceShell Core |
| Since  | v0.2.1-alpha  |

Performs conditional execution, or checks for a condition.

```batch
if [not] <condition expression>... [(then-command)] [ELSE (else-command)]
         <variable>=<value>
         FILE <file-path>
         DIR <directory-path>
         EXISTS <path>
         ERRORLEVEL <exit-code>
```

## Arguments

* **not**: If specified, this inverts the result of the condition expression.
* **condition expression**:
  * `<variable>=<value>`: Determines if the variable is equivalent to the specified value.
  * `FILE <file-path>`: Determines if the specified file exists.
    * **file-path**: The path to the file to check for existence.
  * `DIR <directory-path`: Determines if the specified directory exists.
    * **directory-path**: The path to the directory to check for existence.
  * `EXISTS <path>`: Determines if either a file or a folder exists at the specified path.
    * **path**: The path to check for existence.
* **(then-command)**: Must be wrapped in parentheses. The command to execute if the condition was met.
* **ELSE**: Specifies to execute a command if the condition did not met.
  * **(else-command)**: Required if **ELSE** was specified. Must be wrapped in parentheses. The command to execute if the condition did not met.

## Error Codes

This command may produce error code `8` (`BadArgument`) on invalid argument syntax. This command itself does not produce any other error.

If the error code is `None` but the command returned with non-zero, the command succeeded but the condition specified was not met.

## Exit Code

* If a **then-command** was not specified, returns zero if the condition was met, and non-zero if the condition did not met, or on failure.
* If a **then-command** was specified, returns zero on success and non-zero on failure.

For detailed error information, check the error code. If the error code was `None`, the command succeeded with the condition not met.
