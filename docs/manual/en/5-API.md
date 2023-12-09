# 5. The IceShell API

The IceShell API provided via the IceShell core module, the `IceShell.Core` assembly. It has a namespace, `IceShell.Core.Api` which contains most formats and interfaces for public usage as well as internal implementation details and some utility APIs available for public usage in `IceShell.Core` and its other sub-namespaces.

IceShell relies on other bundled libraries, most notably `IceShell.Parsing` which supports command agnostic parsing services for the IceShell format.

As IceShell is still in rapid development the API may change at any time until the release. You should try not to target "nightly" versions as it can be dangerous to do so; instead, target a preview version with a formal number, that way, changes will be slower for you.

## Commands API

The most commonly used part of the IceShell API would be the commands API. In IceShell, you create internal commands by creating a `CommandEntry` and associate it with a method to register to an instance of `IRegistryCommandDispatcher`. All `IShell` implementations must provide a valid instance of a valid implementation of the `IRegistryCommandDispatcher` interface, and during module initialisation your plugin will be given an instance to an implementation of such interface.

You can use `CommandBuilder` to create command entries and register commands. It is a helper class that helps you to quickly create `CommandEntry` and add options and parameters to the command, assoicating with a `CommandDelegate` and register it to `IRegistryCommandDispatcher`.

For detailed information, see the section 5.1 [Custom Commands](5.1-Custom-Commands.md).

* * *

Copyright (C) 2023 IceShell contributors.

Permission is granted to copy, distribute and/or modify this document under the terms of the GNU Free Documentation License, Version 1.3 or any later version published by the Free Software Foundation; with no Invariant Sections, no Front-Cover Texts, and no Back-Cover Texts.

A copy of the license is included in the file named "LICENSE-DOCS", in the `docs/manual` directory of the project repository.
