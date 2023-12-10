# Contributing Guidelines

Thank you for taking time to improve this very project, whether by
reporting bugs, suggesting features, or contributing code.

In this guide you will know about:

- Ways to contribute
- How do you contribute effectively

## Issue Labels

An issue is usually assigned up to three scoped labels, and when your issue is assigned any of the scoped label it means that
your issue have been acknowledged.

You can also work on issues that have no assignees set and have `status/open` scoped label attached.

### Issue Status

The Issue Status labels indicates the current state of an issue.

- `open`: Open for contributions.
- `question`: More information are needed (the issue is incomplete).
- `in-progress`: Is being actively worked on by at least one contributor.
- `done`: Work has been completed and is either being tested or is pending closure.
- `duplicate`: The issue is a duplicate of another issue.
- `wontfix`: The issue is either impossible to fix or implement, or it is not wise to fix or implement the issue.

### Effort/Expertise Category

The Effort/Expertise Category are set of labels that roughly describes how much effort
and expertise (relative to this project) is required to complete this issue.

- `⏫advanced`: Requires probably around hours to implement and finish, and requires somewhat advanced expertise
- `➖intermediate`: Requires probably around 30m-2h to implement and finish, and intermediate-to-beginner expertise
- `⏬beginner`: Requires not more than 30m to complete (usually low-priority tasks)

## Ways to contribute

There are plenty of ways to contribute, either by writing code to improve this
project, or by other ways (some of them did not even need you to write a single
line of code!).

### Issues

Anyone can report bugs, make suggestions or request new features via the Issues
section of the repository. [Here's an link to it.](https://codeberg.org/WithLithum/IceShell/issues)

#### Bugs

If you are reporting bugs or other sorts of issues of the project, the report
should include the following information:

- The description of the bug
- Detailed information how to reproduce the bug
- What platform are you experiencing the issue with (OS version, .NET version,
  etc.)
- Which version of IceShell are you using (obtain via running `ver`, 
  or on the CI or release page you obtained the download from), make sure to
  include the section after `+` if version was obtained through `ver` command

### Contributing Code

Contributing code is the most direct form of contribution to this project.

#### Building the Project

This project does not need specific steps to build. However, if you need a how
to build information:

- Clone the repository.
- Navigate to the repository directory and open your terminal.
- Make sure .NET SDK is installed.
- Run `dotnet build`.

#### How to Contribute Code

If you need to make major changes, please do open an issue to discuss it first.

For code contributions, you should use Pull Request.

- First, _fork_ this repository, and name it whatever you like.
- Make your changes.
- Add/update tests as appropriate.
- Run unit tests.
- Create Pull Request from your repository. This is usually done by clicking
  `Contribute` in your repository's homepage or via the Pull Requests section.

Anyone can contributing code to this project via the Pull Requests feature of
GitHub. But, please do stick to the following guidelines if you are
contributing code to keep this project tidy:

- Make sure all existing unit tests pass, and new features are tested promptly.
- Follow the code styles of the project.

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
