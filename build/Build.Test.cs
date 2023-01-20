using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.ReportGenerator;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;

partial class Build
{
    AbsolutePath TestResultDirectory => OutputDirectory / "test-results";

    Target Test => _ => _
        .DependsOn(Restore)
        .Triggers(GenerateCoverageReport)
        .Produces(TestResultDirectory / "*.trx")
        .Produces(TestResultDirectory / "*.xml")
        .Executes(() =>
        {
            var unitTestProjects = Solution.GetProjects("*.Tests");

            DotNetTest(_ => _
                    .Apply(TestSettingsBase)
                    .CombineWith(unitTestProjects, (_, v) => _
                        .Apply(TestProjectSettingsBase, v)),
                completeOnFailure: true);
        });

    Configure<DotNetTestSettings> TestSettingsBase => _ => _
        .SetConfiguration(Configuration)
        .SetNoRestore(SucceededTargets.Contains(Restore))
        .SetNoBuild(SucceededTargets.Contains(Compile))
        .ResetVerbosity()
        .SetResultsDirectory(TestResultDirectory)
        .EnableCollectCoverage()
        .SetCoverletOutputFormat(CoverletOutputFormat.cobertura)
        .SetExcludeByFile("**/*.Generated.cs")
        .When(IsServerBuild, _ => _
            .EnableUseSourceLink());

    Configure<DotNetTestSettings, Project> TestProjectSettingsBase => (_, v) => _
        .SetProjectFile(v)
        .SetLoggers($"trx;LogFileName={v.Name}.trx")
        .SetCoverletOutput(TestResultDirectory / $"{v.Name}.xml");

    AbsolutePath CoverageReportDirectory => OutputDirectory / "coverage";

    Target GenerateCoverageReport => _ => _
        .After(Test)
        .Produces(CoverageReportDirectory)
        .Executes(() =>
        {
            EnsureCleanDirectory(CoverageReportDirectory);

            ReportGenerator(s => s
                .SetReports(TestResultDirectory / "*.xml")
                .SetReportTypes(ReportTypes.HtmlInline)
                .SetFramework("net7.0")
                .SetTargetDirectory(CoverageReportDirectory));
        });
}
