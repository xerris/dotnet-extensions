using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Xerris.Nuke.Components;

// ReSharper disable RedundantExtendsListEntry
// ReSharper disable InconsistentNaming

[DotNetVerbosityMapping]
[ShutdownDotNetAfterServerBuild]
partial class Build : NukeBuild,
    IHasGitRepository,
    IHasVersioning,
    IRestore,
    IFormat,
    ICompile,
    ITest,
    IReportCoverage,
    IPack,
    IPush
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => ((ICompile) x).Compile);

    [Solution]
    readonly Solution Solution;
    Solution IHasSolution.Solution => Solution;

    public IEnumerable<AbsolutePath> ExcludedFormatPaths => [];

    public bool RunFormatAnalyzers => IsLocalBuild;

    Target ICompile.Compile => t => t
        .Inherit<ICompile>()
        .DependsOn<IFormat>(x => x.VerifyFormat);

    bool IReportCoverage.CreateCoverageHtmlReport => true;

    IEnumerable<Project> ITest.TestProjects => Partition.GetCurrent(Solution.GetAllProjects("*.Tests"));

    Configure<DotNetPublishSettings> ICompile.PublishSettings => t => t
        .When(!ScheduledTargets.Contains(((IPush) this).Push), _ => _
            .ClearProperties());

    Target IPush.Push => t => t
        .Inherit<IPush>()
        .Consumes(this.FromComponent<IPush>().Pack)
        .Requires(() => this.FromComponent<IHasGitRepository>().GitRepository.Tags.Count != 0)
        .WhenSkipped(DependencyBehavior.Execute);
}
