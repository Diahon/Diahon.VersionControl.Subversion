using Diahon.VersionControl.Subversion.Test;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using System.Text;

[assembly: AssemblyFixture(typeof(SvnContainer))]

namespace Diahon.VersionControl.Subversion.Test;

internal sealed class SvnContainer : IAsyncLifetime
{
    public static byte[] TestBinContent { get; } = RandomBuffer(100);

    public IContainer Container { get; }
    public SvnContainer()
    {
        const string SvnRoot = "/var/svn";
        const string EntryPointPath = "/app/entrypoint.sh";

        Container = new ContainerBuilder()
            .WithImage("obslib/subversion:svnserve-latest-0")
            .WithPortBinding(SvnClient.SvnPort)

            // Config
            .WithResourceMapping(
                Resource(
                    "[Users]",
                    $"{Env.Credentials.UserName} = {Env.Credentials.Password}"
                ),
                $"{SvnRoot}/passwd"
            )
            .WithResourceMapping(
                Resource(
                    "[general]",
                    "anon-access = none",
                    "auth-access = write",
                    "password-db = passwd",
                    "realm = My SVN Repository",
                    "log-file=/var/log/svn.log"
                ),
                $"{SvnRoot}/svnserve.conf"
            )

            // Files
            .WithResourceMapping(
                Resource("Hello World"),
                $"{SvnRoot}/file.txt"
            )
            .WithResourceMapping(
                TestBinContent,
                $"{SvnRoot}/file.bin"
            )

            // Entrypoint
            .WithResourceMapping(
                Resource(
                    "#!/usr/bin/env bash",
                    // Create repo
                    $"/usr/local/subversion/bin/svnadmin create {SvnRoot}/{Env.SvnRepoName}",
                    // Copy config into repo
                    $"cp {SvnRoot}/passwd {SvnRoot}/{Env.SvnRepoName}/conf/passwd",
                    $"cp {SvnRoot}/svnserve.conf {SvnRoot}/{Env.SvnRepoName}/conf/svnserve.conf",
                    // Add files to repo
                    $"/usr/local/subversion/bin/svn import -m \"file.txt\" {SvnRoot}/file.txt file:///{SvnRoot}/{Env.SvnRepoName}/file.txt",
                    $"/usr/local/subversion/bin/svn import -m \"file.bin\" {SvnRoot}/file.bin file:///{SvnRoot}/{Env.SvnRepoName}/file.bin",
                    // Start svn daemon
                    $"/usr/local/subversion/bin/svnserve --daemon --foreground --root={SvnRoot}"
                ),
                EntryPointPath,
                UnixFileModes.UserRead | UnixFileModes.UserExecute
            )
            .WithEntrypoint(EntryPointPath)

            // Build
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await Container.StartAsync();

        // Give svn some time to start
        // ToDo: Find a better solution
        await Task.Delay(TimeSpan.FromSeconds(2));
    }

    public async ValueTask DisposeAsync()
        => await Container.StopAsync();

    static byte[] Resource(params string[] lines)
        => Encoding.Default.GetBytes(string.Join('\n', lines));

    static byte[] RandomBuffer(int length)
    {
        var buffer = new byte[length];
        Random.Shared.NextBytes(buffer);
        return buffer;
    }
}
