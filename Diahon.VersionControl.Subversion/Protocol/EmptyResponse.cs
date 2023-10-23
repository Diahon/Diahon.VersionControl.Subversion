using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Protocol;

public sealed class EmptyResponse : ISvnServerPrototype<EmptyResponse>
{
    static readonly EmptyResponse Empty = new();

    private EmptyResponse() { }

    public static EmptyResponse ReadContent(ref SvnReader reader)
        => Empty;
}
