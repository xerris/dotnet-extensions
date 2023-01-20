using System.IO;
using System.Text.Json;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Serilog;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

// ReSharper disable InconsistentNaming

[GitHubActions(
    "continuous",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.MacOsLatest,
    FetchDepth = 0,
    OnPushBranches = new []{ "main", "release/*" },
    OnPushTags = new[] { "v*" },
    OnPullRequestBranches = new[] { "main" },
    PublishArtifacts = true,
    InvokedTargets = new[] { nameof(Compile), nameof(Test) },
    CacheKeyFiles = new[] { "global.json", "source/**/*.csproj" })]
partial class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    protected override void OnBuildInitialized()
    {
        Log.Information("{VersionInfo}", JsonSerializer.Serialize(GitVersion, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Required] [GitVersion(Framework = "net6.0", NoFetch = true)] readonly GitVersion GitVersion;

    [CI] readonly GitHubActions GitHubActions;

    [Solution] readonly Solution Solution;

    AbsolutePath OutputDirectory => RootDirectory / "output";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetClean(s => s
                .SetProject(Solution));

            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    AbsolutePath VersionFile => OutputDirectory / "VERSION";

    Target Compile => _ => _
        .DependsOn(Clean, Restore)
        .Produces(VersionFile)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());

            File.WriteAllText(VersionFile, GitVersion.NuGetVersionV2);
        });

}
