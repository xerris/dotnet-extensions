using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
    "publish",
    GitHubActionsImage.UbuntuLatest,
    FetchDepth = 0,
    OnPushBranches = new[] { MainBranch, $"{ReleaseBranchPrefix}/*" },
    OnPushTags = new[] { "v*" },
    InvokedTargets = new[] { nameof(Publish) },
    ImportSecrets = new[] { nameof(NuGetApiKey) })]
partial class Build
{
    Target Pack => _ => _
        .DependsOn(Compile)
        .Produces(OutputDirectory / "*.nupkg")
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .SetNoBuild(SucceededTargets.Contains(Compile))
                .SetOutputDirectory(OutputDirectory)
                .SetRepositoryUrl(GitRepository.HttpsUrl)
                .SetVersion(GitVersion.NuGetVersionV2));
        });

    [Parameter][Secret] readonly string NuGetApiKey;

    IEnumerable<AbsolutePath> PackageFiles => OutputDirectory.GlobFiles("*.nupkg");

    Target Publish => _ => _
        .DependsOn(Pack)
        .Consumes(Pack)
        .OnlyWhenStatic(() => false) // Temporarily disabled
        //.Requires(() => NuGetApiKey) // Temporarily disabled
        .Executes(() =>
        {
            DotNetNuGetPush(_ => _
                .SetSource("https://api.nuget.org/v3/index.json")
                .SetApiKey(NuGetApiKey)
                .CombineWith(PackageFiles, (_, v) => _
                    .SetTargetPath(v)));
        });
}