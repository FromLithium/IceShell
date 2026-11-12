// Copyright (C) NexusKrop & contributors 2023
// See "COPYING.txt" for licence

using Cake.Common;
using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.GitVersion;
using Cake.Core;
using Cake.Frosting;

[TaskName("CIBuild")]
[IsDependentOn(typeof(UploadArtefactTask))]
public class CIBuildTask : FrostingTask<CIContext>
{
}

public class CIContext : FrostingContext
{
    public string MsBuildConfiguration { get; set; }
    public string GitHubKey { get; set; }

    public CIContext(ICakeContext context)
        : base(context)
    {
        MsBuildConfiguration = context.Argument("configuration", "Release");
    }
}

[TaskName("CIBuildApp")]
[IsDependentOn(typeof(RestoreTask))]
public class CIBuildAppTask : FrostingTask<CIContext>
{
    public override void Run(CIContext context)
    {
        var runId = context.GitHubActions().Environment.Workflow.RunId;
        var @ref = context.GitHubActions().Environment.Workflow.RefName;
        var targetPath = context.GitHubActions().Environment.Runner.Workspace.Combine("build-output");

        context.CreateDirectory(targetPath);

        var settings = new DotNetBuildSettings()
        {
            Configuration = context.MsBuildConfiguration,
            NoRestore = true,
            OutputDirectory = targetPath
        };

        if (context.GitHubActions().Environment.Workflow.RefType != Cake.Common.Build.GitHubActions.Data.GitHubActionsRefType.Tag)
        {
            settings.VersionSuffix = $"dev-artefact.{runId}.{@ref}";
        }

        context.DotNetBuild("../IceShell.slnx", settings);
    }
}

[TaskName("CITest")]
[IsDependentOn(typeof(CIBuildAppTask))]
public sealed class CITestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Information("Running tests");

        context.DotNetTest("../IceShell.Tests/IceShell.Tests.csproj", new()
        {
            NoBuild = true,
            NoRestore = true,
            OutputDirectory = context.GitHubActions().Environment.Runner.Workspace.Combine("build-output")
        });
    }
}

[TaskName("CIArtefact")]
[IsDependentOn(typeof(CITestTask))]
public class UploadArtefactTask : FrostingTask<CIContext>
{
    public override void Run(CIContext context)
    {
        if (!context.BuildSystem().IsRunningOnGitHubActions)
        {
            context.Information("Not GitHub actions.");
            return;
        }

        var github = context.GitHubActions();
        github.Commands.UploadArtifact(context.GitHubActions().Environment.Runner.Workspace.Combine("build-output"), "build-output");
    }
}