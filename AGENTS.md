# AGENTS instructions

## Repository structure

- `assets/`: Assets like logos.
- `lang/`: JSON language files.
- `src/`: Source code.

## Prerequisites

- .NET 10 SDK
- If on Windows, developer mode enabled (for symlinks to work) during checkout.
- [AI Policy](AI_POLICY.md) followed.

## Commands

From repository root:

- **Restore**: `dotnet restore src/IceShell.sln`
- **Build**: `dotnet build src/IceShell/IceShell.csproj`
- **Test**: `dotnet test --project src/IceShell.Tests/IceShell.Tests.csproj`