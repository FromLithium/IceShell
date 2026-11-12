// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNet;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

public class BuildContext : FrostingContext
{
    public string MsBuildConfiguration { get; set; }

    public BuildContext(ICakeContext context)
        : base(context)
    {
        MsBuildConfiguration = context.Argument("configuration", "Release");
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(TestTask))]
public class DefaultTask : FrostingTask<BuildContext>
{
}

[TaskName("Restore")]
public sealed class RestoreTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Information("Restoring");

        context.DotNetRestore("../");
    }
}

[TaskName("BuildAll")]
[IsDependentOn(typeof(RestoreTask))]
public sealed class BuildAllTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Information("Building");

        context.DotNetBuild("../IceShell.slnx", new()
        {
            Configuration = context.MsBuildConfiguration,
            NoRestore = true
        });
    }
}

[TaskName("Test")]
[IsDependentOn(typeof(BuildAllTask))]
public sealed class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Information("Running tests");

        context.DotNetTest("../IceShell.Tests/IceShell.Tests.csproj", new()
        {
            NoBuild = true,
            NoRestore = true
        });
    }
}