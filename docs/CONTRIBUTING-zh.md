# 贡献者指南

首先感谢您为改进本项目付出时间。本文档皆在指导您：

- 如何帮助改进本项目
- 如何提高贡献效率

## 工单标签

注：本处“工单”一词指 Issues，此处为 Gitea / Forgejo 翻译。

### 耗费分类标签

精力/能力分类标签主要分类解决该工单在技术上需要耗费的时间、精力以及需要的大概熟练水平。

- `⏫advanced`：需要数小时的时间来实现，或者需要较为熟练的编程经验
- `➖intermediate`：需要半小时至两小时的时间来实现，或者需要一定的编程经验
- `⏬beginner`：最多需要半小时实现，或者只需要初级编程经验

## 如何帮助改进本项目

无论是改进本项目的代码，或者以其它方式（甚至可能不需要您会任何编程）都能帮助改进本项目的质量。

### 工单

您可以在“工单管理”一栏中报告程序的问题、提出您宝贵的建议，甚至是建议添加新的功能！[点击前往](https://codeberg.org/WithLithum/IceShell/issues)。

#### 报告程序的问题

如果您要报告程序中出现的漏洞或是其它问题，请明确提供以下信息，以便于测试和解决问题：

- 问题的简短描述
- 遇到此问题时的平台信息（如 .NET 运行时版本、操作系统版本等）
  etc.)
- 遇到此问题时运行的 IceShell 版本号（使用 `ver` 命令即可取得，如果使用 `ver` 命令请勿省略或删去显示的版本号的加号`+`后面的内容。 

### 贡献代码

改进本项目的最直接方式就是贡献代码。

#### 构建项目

本项目无需有别于编译其它 .NET 程序的编译步骤，您可以使用您的 IDE 编译。但是如果您需要一个步骤指引的话：

- 克隆本仓库。
- 打开克隆本仓库的目录，然后在该仓库打开您的终端（通常可以通过右键菜单就能在此打开终端，Windows 需要 `Shift` 右键）。
- 确保已安装 .NET 8.0 SDK 及以上版本。如果您没有，请去[官方网站](https://dot.net)下载 .NET 8 SDK。
- 用终端运行`dotnet build` 命令。

#### 代码贡献指南

如果您愿作出较大改动，请先创建一个工单来讨论该改动方案。

请使用“合并请求”功能来提交您的代码贡献。不接受除此以外的任何代码贡献方案。

- 首先使用*派生*功能创建您的本仓库的副本，您可以随意命名。
- 创建一个新的分支（我们建议以 `contrib/feature/<feature>` 来命名，其中 `<feature>` 应为您要作出的修改的简短英文名称。
- 签出这个分支，作出您的改动。
- 按情况添加或变更现有的单元测试。对于重大新功能来说，基础功能最好被单元测试覆盖；这样，只要您运行单元测试，您就能知道您的功能是否按预期运行，而不用去调试整个 IceShell 程序并实验。
- 运行单元测试并检查结果。
- 回到这个仓库，打开合并请求一栏，点击“创建合并请求”，并选择 `trunk` 分支作为目标分支，然后选择您在第二部创建的分支作为源分支，然后就可以点击创建了。

还建议您遵循以下原则以帮助确保本仓库代码整洁有序：

- 确认所有现有的单元测试测试都成功，并且新的改动测试也没有问题。
- 遵循本项目的代码规范。

#### Commit styles

This repository adopts the [Gitmoji](https://gitmoji.dev) commit message standard, with some minor variations.

The commit message format is as belows:

```text
<type-emoji> (<scope>) <message> [(issue)]
```

* `type-emoji`: A Gitmoji emoji depicting the type of the changes made by that commit.
* `scope`: The commit scope. Described below.
* `message`: A brief description of the changes made by the commit.
* `issue`: Optional. Reference to the issue or PR related to.

#### Commit scope

* `general`: Project configurations, `README`, `CHANGELOG` etc. as well as general refactors, etc. overall changes.
* `docs`: `CONTRIBUTING`, the manual, etc.
* `command`: Commands and command execution and parsing engine (the dispatcher, registry, etc.). `IceShell.Core.Commands` namespace.
* `shell`: The shell interactive frontend, and the executive.
* `syntax`: `IceShell.Core.Syntax` namespace, and `IceShell.Parsing` namespace.
* `locale`: Localisation engine, and localisation and internationalisation.
