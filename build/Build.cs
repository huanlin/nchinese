using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.NuGet;
using Nuke.Core;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;
using static Nuke.Core.IO.FileSystemTasks;
using static Nuke.Core.IO.PathConstruction;

class Build : NukeBuild
{
    // Console application entry. Also defines the default target.
    public static int Main () => Execute<Build>(x => x.Compile);

    // Auto-injection fields:

    [Nuke.Common.Tools.GitVersion.GitVersion] readonly GitVersion GitVersion;
    // Semantic versioning. Must have 'GitVersion.CommandLine' referenced.

    // [GitRepository] readonly GitRepository GitRepository;
    // Parses origin, branch name and head from git config.

    // [Parameter] readonly string MyGetApiKey;
    // Returns command-line arguments and environment variables.

    Target Clean => _ => _
            .OnlyWhen(() => false) // Disabled for safety.
            .Executes(() =>
            {
                DeleteDirectories(GlobDirectories(SourceDirectory, "**/bin", "**/obj"));
                EnsureCleanDirectory(OutputDirectory);
            });

    Target Restore => _ => _
            .DependsOn(Clean)
            .Executes(() =>
            {
                MSBuild(s => DefaultMSBuildRestore);
            });

    Target Compile => _ => _
            .DependsOn(Restore)
            .Executes(() =>
            {
                MSBuild(s => DefaultMSBuildCompile);
            });

    Target Pack => _ => _
            .DependsOn(Compile)
            .Executes(() =>
            {
                Logger.Info($"Nuget packages will be created in '{OutputDirectory}'");

                var nugetSettings = DefaultNuGetPack.SetBasePath(RootDirectory);
                string nuspecFileName = RootDirectory / $"nuspec/NChinese.nuspec";
                Logger.Info($"Creating Nuget package with {nuspecFileName}");
                NuGetPack(nuspecFileName, s => nugetSettings);

                nuspecFileName = RootDirectory / $"nuspec/NChinese.Imm.nuspec";
                Logger.Info($"Creating Nuget package with {nuspecFileName}");
                NuGetPack(nuspecFileName, s => nugetSettings);
            });
}
