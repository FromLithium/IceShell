# 0. 前言

IceShell 是一个用 C# 编写的实验性命令解释器，其命令语法由 Windows 命令提示符和 MS-DOS 启发。

> [!NOTE]
> 本参考手册于 2023 年 10 月编写，最后一次更新在 2023 年 12 月。如果程序在本文档编写之后修改，本文档可能与程序不一致。请仔细检查[修改日志](../../../CHANGELOG.md)。

本命令解释器是一个业余项目，并非设计用于高级系统管理或远程使用。详见[此处](Appendix-A-Security.md)。

加入我们的 Discord 服务器：[![Discord](https://img.shields.io/discord/1178887806286823424?style=flat-square&logo=discord&logoColor=white&label=%20&color=blue)](https://discord.gg/UFfWb9Rj)

## 入门

### 安装

您可以从“版本发布”页面下载最新版本。

#### 版本发布

您可以从 [Codeberg 版本发布页面](https://codeberg.org/WithLithum/IceShell/releases)下载最新的发布版。通常发布版会提供 Windows 和 GNU/Linux 使用的二进制文件。

macOS 用户可以下载任一版本，并使用`dotnet iceshell.dll`来替代`iceshell.exe`或`iceshell`来启动 IceShell。

### 使用方法

#### Hello World 实例

在 IceShell 命令提示下输入：

```bat
echo Hello World!
```

随后程序会反馈：

```plain
Hello World!
```

请查阅以下页面了解程序的详细用法：

- [Syntax](2-Syntax.md)
- [Built-in commands](3-BuiltIn-Commands.md)

---

Copyright (C) 2023 IceShell contributors.

Permission is granted to copy, distribute and/or modify this document under the terms of the GNU Free Documentation License, Version 1.3 or any later version published by the Free Software Foundation; with no Invariant Sections, no Front-Cover Texts, and no Back-Cover Texts.

A copy of the license is included in the file named "LICENSE-DOCS", in the `docs/manual` directory of the project repository.
