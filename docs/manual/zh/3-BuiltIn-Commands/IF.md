# IF

| 来自 | 信息           |
|----|--------------|
| 模块 | IceShell 核心  |
| 版本 | v0.2.1-alpha |

进行条件判断执行，或者单纯的检查某个条件。

```batch
if [not] <condition expression>... [(then-command)] [ELSE (else-command)]
         <variable>=<value>
         FILE <file-path>
         DIR <directory-path>
         EXISTS <path>
         ERRORLEVEL <exit-code>
```

## 参数

* **not**：如填写则反转条件。
* **condition expression**：
  * `<variable>=<value>`：判断变量是否与指定的值一致。
  * `FILE <file-path>`：判断指定文件是否存在。
    * **file-path**：欲判断是否存在的文件路径。
  * `DIR <directory-path`：判断指定目录是否存在。
    * **directory-path**：欲判断是否存在的目录。
  * `EXISTS <path>`：判断文件或目录是否存在于指定路径。
    * **path**：欲判断的路径。
* **(then-command)**：欲在条件满足时执行的命令。必须以半角圆括号包围。
* **ELSE**: 指定如条件不满足时执行的命令。
  * **(else-command)**: 如 **ELSE** 被指定则必填。必须以半角圆括号包围。欲在条件不满足时执行的命令。

## 错误代码

本命令本身仅会在参数语法不正确时返回错误代码 `8`（`BadArgument`），除此以外不会产生任何其它错误代码。

如错误代码为 `None` 但命令返回代码不为零，则表示命令执行成功但指定的条件不满足。

## 返回代码

* 如果 **then-command** 未指定，则在条件满足时返回零；如果条件不满足或命令执行失败，则返回非零。
* 如果 **then-command** 已指定，则在执行命令成功时返回零，否则返回非零。

请检查错误代码来了解详细错误信息。如错误代码为 `None` 但命令返回代码不为零，则表示命令执行成功但指定的条件不满足。

---

Copyright (C) 2023 IceShell contributors.

Permission is granted to copy, distribute and/or modify this document under the terms of the GNU Free Documentation License, Version 1.3 or any later version published by the Free Software Foundation; with no Invariant Sections, no Front-Cover Texts, and no Back-Cover Texts.

A copy of the license is included in the file named "LICENSE-DOCS", in the `docs/manual` directory of the project repository.
