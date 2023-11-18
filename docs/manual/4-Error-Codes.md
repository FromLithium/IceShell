# Error Codes

This document lists all error codes currently available in the IceShell environment. All built-in commands and commands in built-in modules uses these standard error codes.

Commands from external modules can have custom error codes; refer to their documentation for more information. External commands does not have error codes; you should interpret their return code.

The [Built-in commands](3-BuiltIn-Commands.md) page describes the exact meaning of error codes returned by built-in commands and commands from built-in modules.

| No. | Error Name                   | Description                                                      |
|-----|------------------------------|------------------------------------------------------------------|
| 0   | None                         | Error was not specified                                          |
| 1   | ParsingFailure               | Error occurred while parsing the command                         |
| 2   | ExternalStartFail            | Failed to start external program                                 |
| 3   | GenericCommandFail           | Command failed due to unknown error                              |
| 4   | GenericCommandException      | Exception thrown in command routine                              |
| 5   | ElevationRequired            | Elevated/root privileges required                                |
| 6   | BadDirectory                 | Directory does not exist (when command requires it to exist)     |
| 7   | SingleDestinationMultiSource | Source contains multiple file but destination is a single file   |
| 8   | SyntaxError                  | A syntax error was found in command                              |
| 9   | BadFile                      | File does not exist (when command requires it to exist)          |
| 10  | BadArgument                  | Invalid argument                                                 |
| 11  | OperatingSystemError         | Error occurred in, from, or due to operating system              |
| 12  | OperationNotSupported        | Not implemented or not supported                                 |
| 13  | FileExists                   | File already exists (when command requires it to be absent)      |
| 14  | DirectoryExists              | Directory already exists (when commands require it to be absent) |
| 15  | WriteUnauthorized            | Writing to destination is not authorized                         |
| 16  | RequiresPipelineInput        | Command requires pipeline input                                  |
| 17  | BadCommand                   | Command not found                                                |

---

Copyright (C) 2023 IceShell contributors.

Permission is granted to copy, distribute and/or modify this document under the terms of the GNU Free Documentation License, Version 1.3 or any later version published by the Free Software Foundation; with no Invariant Sections, no Front-Cover Texts, and no Back-Cover Texts.

A copy of the license is included in the file named "LICENSE-DOCS", in the `docs/manual` directory of the project repository.
