using Diahon.VersionControl.Subversion.Primitives;
using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Protocol;

// repos-info: ( uuid:string repos-url:string ( cap:word ... ) )
public sealed class RepoInfos : ISvnServerPrototype<RepoInfos>
{
    public required SvnString UUID { get; init; }
    public required Uri Url { get; init; }
    public required IReadOnlyList<SvnWord> Capabilities { get; init; }

    public static RepoInfos ReadContent(ref SvnReader reader)
    {
        if (!reader.TryReadPrimitiveOrEnd<SvnString>(out var uuid))
            throw SvnFormatException.UnexpectedEndOfList();

        if (!reader.TryReadPrimitiveOrEnd<SvnString>(out var url))
            throw SvnFormatException.UnexpectedEndOfList();

        if (!reader.TryReadPrimitiveArrayOrEnd<SvnWord>(out var capabilities))
            throw SvnFormatException.UnexpectedEndOfList();

        return new()
        {
            UUID = uuid,
            Url = new(url.Value),
            Capabilities = capabilities
        };
    }
}
