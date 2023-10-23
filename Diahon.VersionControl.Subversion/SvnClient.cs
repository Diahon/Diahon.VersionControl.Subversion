using Diahon.VersionControl.Subversion.Primitives;
using Diahon.VersionControl.Subversion.Protocol;
using Diahon.VersionControl.Subversion.Protocol.Commands;
using Diahon.VersionControl.Subversion.Serialization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Diahon.VersionControl.Subversion;

public sealed class SvnClient : IDisposable
{
    const int SvnPort = 3690;

    readonly TcpClient _client;
    public Uri Uri { get; }
    public RepoInfos RepoInfos { get; }
    private SvnClient(TcpClient client, Uri uri, RepoInfos repoInfos)
    {
        _client = client;
        Uri = uri;
        RepoInfos = repoInfos;
    }

    private static void InitializeIO(TcpClient client, out SvnWriter writer, out SvnReader reader)
    {
        var stream = client.GetStream();
        reader = new(stream);
        writer = new(stream);
    }

    public static async ValueTask<SvnClient> ConnectAsync(Uri connectionUri, NetworkCredential credential)
    {
        TcpClient client = new();
        await client.ConnectAsync(connectionUri.Host, SvnPort);

        var repoInfos = await Task.Run(() =>
        {
            InitializeIO(client, out var writer, out var reader);

            _ = reader.Read<CommandResponse<Greeting>>();
            writer.Write<GreetingResponse>(new()
            {
                Version = 2,
                Capabilities = new List<SvnWord>() { new("edit-pipeline"), new("svndiff1"), new("accepts-svndiff2"), new("absent-entries"), new("depth"), new("mergeinfo"), new("log-revprops") },
                Url = connectionUri.OriginalString
            });

            HandleAuth(ref writer, ref reader, credential);

            return reader.Read<CommandResponse<RepoInfos>>().Content;
        });

        return new(client, connectionUri, repoInfos);
    }

    static void HandleAuth(ref SvnWriter writer, ref SvnReader reader, NetworkCredential? credential = null)
    {
        var authRequest = reader.Read<CommandResponse<AuthRequest>>();
        if (authRequest.Content.Mech.Count == 0)
            return;

        ArgumentNullException.ThrowIfNull(credential);

        writer.Write<AuthResponse>(new()
        {
            Mech = new("CRAM-MD5")
        });
        var challenge = reader.Read<AuthChallenge>();

        new AuthChallengeResponse()
        {
            Challenge = challenge.TokenOrMessage?.Value ?? throw new NullReferenceException("Empty auth challenge"),
            Credential = credential
        }.WriteContent(ref writer);

        challenge = reader.Read<AuthChallenge>();
        if (challenge.AuthStatus.Value != "success")
            throw new ArgumentException(challenge.TokenOrMessage?.Value ?? "Invalid credentials", nameof(credential));
    }

    public IReadOnlyList<Dirent> List(string? path = null, params string[]? patterns)
    {
        InitializeIO(_client, out var writer, out var reader);

        writer.WriteCommand<ListCommand>(new()
        {
            Path = path ?? "",
            Depth = ListCommand.DepthInfinity,
            Fields = new List<SvnWord> { ListCommand.FieldKind },
            Patterns = patterns
        });

        HandleAuth(ref writer, ref reader);

        List<Dirent> result = new();
        while (!reader.TryReadDone())
        {
            result.Add(Dirent.ReadContent(ref reader));
            reader.ReadListEnd();
        }

        reader.Read<CommandResponse<EmptyResponse>>();

        return result;
    }

    public ReadOnlyMemory<byte> GetFile(string path)
    {
        InitializeIO(_client, out var writer, out var reader);

        writer.WriteCommand<GetFileCommand>(new()
        {
            Path = path ?? "",
            WantProps = false,
            WantContents = true
        });

        HandleAuth(ref writer, ref reader);

        reader.Read<CommandResponse<GetFileResponse>>();
        return GetFileResponse.ReadFileContents(ref reader);
    }

    public string GetFileString(string path, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;

        var buffer = GetFile(path);
        return encoding.GetString(buffer.Span);
    }

    public void Dispose()
    {
        _client.Close();
        _client.Dispose();
    }
}
