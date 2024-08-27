using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Nuke.Common.Tools.DotNet;
using Microsoft.Build.Construction;
using static Nuke.Common.IO.CompressionTasks;
using System.IO;
using Nuke.Common.CI.GitHubActions;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetClean(setting => setting
                .SetProject(RootDirectory / "listdepasm.sln")
                .SetConfiguration(Configuration));
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(setting => setting);
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(setting => setting.SetConfiguration(Configuration));
        });
    [Parameter]
    readonly string Runtime;
    Target Publish => _ => _
        .DependsOn(Restore)
        .Requires(() => !string.IsNullOrEmpty(Runtime))
        .Executes(() =>
        {
            var publishDir = RootDirectory / "dist" / "publish" / Configuration / Runtime;
            var project = RootDirectory / "listdepasm" / "listdepasm.csproj";
            DotNetPublish(setting => setting.SetConfiguration(Configuration)
                .SetProject(project)
                .SetProperty("EnableSingleFile", "true")
                .SetRuntime(Runtime)
                .SetProperty("PublishDir", publishDir));
        });
    AbsolutePath GetPublishDir() => RootDirectory / "dist" / "publish" / Configuration / Runtime;
    Target Archive => _ => _
        .Requires(() => !string.IsNullOrEmpty(Runtime))
        .After(Publish)
        .Executes(() =>
        {
            var publishDir = GetPublishDir();
            var tmpdir = RootDirectory / "dist" / "tmp" / Configuration;
            var destdir = RootDirectory / "dist" / "archive" / Configuration / Runtime;
            if (tmpdir.Exists())
            {
                tmpdir.DeleteDirectory();
            }
            foreach(var f in publishDir.GetFiles())
            {
                if(f.Extension != ".pdb")
                {
                    CopyFileToDirectory(f, tmpdir, FileExistsPolicy.Overwrite);
                }
            }
            tmpdir.ZipTo(destdir / $"listdepasm-{Runtime}.zip", fileMode: FileMode.Create);
            tmpdir.TarGZipTo(destdir / $"listdepasm-{Runtime}.tgz", fileMode: FileMode.Create);
        });
    Target Pack => _ => _
        .After(Restore)
        .Executes(() =>
        {
            var project = RootDirectory / "listdepasm" / "listdepasm.csproj";
            var destdir = RootDirectory / "dist" / "nupkg" / Configuration;
            destdir.CreateOrCleanDirectory();
            DotNetPack(setting => setting.SetProject(project)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(destdir));
        });
    [PathVariable]
    Tool Gh;
    [Parameter]
    readonly string TagName;
    Target UploadGithubRelease => _ => _
        .After(Archive)
        .Requires(() => !string.IsNullOrEmpty(Runtime))
        .Executes(() =>
        {
            Gh($"release upload {TagName} ");
        });
}
