using Nuke.Common.CI.GitHubActions;
using Xerris.Nuke.Components;

[GitHubActions(
    "continuous",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.MacOsLatest,
    FetchDepth = 0,
    OnPullRequestBranches = new[] { "main" },
    OnPushBranches = new[] { "main", "release/v*" },
    PublishArtifacts = true,
    InvokedTargets = new[] { nameof(ITest.Test), nameof(IPack.Pack) },
    CacheKeyFiles = new[] { "global.json", "src/**/*.csproj" })]
partial class Build
{
}
