using Diahon.VersionControl.Subversion.Test;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using System.Text;

[assembly: AssemblyFixture(typeof(SvnContainer))]

namespace Diahon.VersionControl.Subversion.Test;

internal sealed class SvnContainer : IAsyncLifetime
{
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

            // Entrypoint
            .WithResourceMapping(
                Resource(
                    "#!/usr/bin/env bash",
                    $"/usr/local/subversion/bin/svnadmin create {SvnRoot}/{Env.SvnRepoName}",
                    $"cp {SvnRoot}/passwd {SvnRoot}/{Env.SvnRepoName}/conf/passwd",
                    $"cp {SvnRoot}/svnserve.conf {SvnRoot}/{Env.SvnRepoName}/conf/svnserve.conf",
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
        => await Container.StartAsync();

    public async ValueTask DisposeAsync()
        => await Container.StopAsync();

    static byte[] Resource(params string[] lines)
        => Encoding.Default.GetBytes(string.Join('\n', lines));
}
