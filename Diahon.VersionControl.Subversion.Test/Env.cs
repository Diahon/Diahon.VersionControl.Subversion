using System.Net;

namespace Diahon.VersionControl.Subversion.Test;

static class Env
{
    public const string SvnRepoName = "repo2";
    public static Uri RepoUrl { get; } = new($"svn://localhost/{SvnRepoName}");
    public static NetworkCredential Credentials { get; } = new("some_user", "ps123");
}
