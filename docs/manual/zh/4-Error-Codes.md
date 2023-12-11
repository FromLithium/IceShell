# 4. 错误代码参考

本节列出所有 IceShell 内置错误代码以供参考。所有内置命令和模块定义的内部命令都使用这些错误代码；但是，模块有可能定义自定义错误代码，请查阅模块的文档确认。

外部命令不适用错误代码；请检查外部命令的退出代码来检查是否出错。

[内置命令](3-BuiltIn-Commands.md)一节还有各个命令返回值以及返回错误代码的详细解释。

| No. | 代码名称                         | 代码详细信息             |
|-----|------------------------------|--------------------|
| 0   | None                         | 未指定命令错误代码          |
| 1   | ParsingFailure               | 解析命令时出错            |
| 2   | ExternalStartFail            | 启动外部程序失败           |
| 3   | GenericCommandFail           | 命令因未知错误执行失败        |
| 4   | GenericCommandException      | 命令执行程序出现异常         |
| 5   | ElevationRequired            | 需要管理员权限            |
| 6   | BadDirectory                 | 目录不存在              |
| 7   | SingleDestinationMultiSource | 源文件是多个文件，但目标只有单个文件 |
| 8   | SyntaxError                  | 命令中有语法错误           |
| 9   | BadFile                      | 文件不存在              |
| 10  | BadArgument                  | 参数无效               |
| 11  | OperatingSystemError         | 操作系统错误             |
| 12  | OperationNotSupported        | 未实现或不支持            |
| 13  | FileExists                   | 文件已经存在             |
| 14  | DirectoryExists              | 目录已经存在             |
| 15  | WriteUnauthorized            | 没有向目标写的权限          |
| 16  | RequiresPipelineInput        | 命令需要管道输入           |
| 17  | BadCommand                   | 找不到命令              |

---

Copyright (C) 2023 IceShell contributors.

Permission is granted to copy, distribute and/or modify this document under the terms of the GNU Free Documentation License, Version 1.3 or any later version published by the Free Software Foundation; with no Invariant Sections, no Front-Cover Texts, and no Back-Cover Texts.

A copy of the license is included in the file named "LICENSE-DOCS", in the `docs/manual` directory of the project repository.
