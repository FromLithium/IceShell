# Building Guidelines

## With .NET CLI

Building IceShell with CLI requires .NET 8.0 SDK or later.

- Navigate to the solution folder.
- Run `dotnet build`.
- Binaries should be available in `IceShell/src/IceShell/bin`.

## With Visual Studio

Building IceShell with Visual Studio requires Visual Studio 2022 or later,
with .NET Desktop Development workload.

- Open solution in Visual Studio.
- (Optional) Click `Debug` on the launch bar and change it to `Release`.
- Click Build -> Build Solution.
- Binaries should be available in `IceShell/src/IceShell/bin`.
