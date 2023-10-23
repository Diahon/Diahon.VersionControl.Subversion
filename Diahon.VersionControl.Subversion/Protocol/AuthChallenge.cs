using Diahon.VersionControl.Subversion.Primitives;
using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Protocol;

public sealed class AuthChallenge : ISvnServerPrototype<AuthChallenge>
{
    public required SvnWord AuthStatus { get; init; }
    public SvnString? TokenOrMessage { get; init; }

    public static AuthChallenge ReadContent(ref SvnReader reader)
    {
        if (!reader.TryReadPrimitiveOrEnd<SvnWord>(out var status))
            throw SvnFormatException.UnexpectedEndOfList();

        reader.ReadListStart();
        if (reader.TryReadPrimitiveOrEnd<SvnString>(out var tokenOrMessage))
            reader.ReadListEnd();

        return new()
        {
            AuthStatus = status,
            TokenOrMessage = tokenOrMessage
        };
    }
}
