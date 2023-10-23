using Diahon.VersionControl.Subversion.Primitives;
using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Protocol;

// auth-request: ( ( mech:word ... ) realm:string )
public sealed class AuthRequest : ISvnServerPrototype<AuthRequest>
{
    public required IReadOnlyList<SvnWord> Mech { get; init; }
    public required SvnString Realm { get; init; }

    public static AuthRequest ReadContent(ref SvnReader reader)
    {
        if (!reader.TryReadPrimitiveArrayOrEnd<SvnWord>(out var mech))
            throw SvnFormatException.UnexpectedEndOfList();

        if (!reader.TryReadPrimitiveOrEnd<SvnString>(out var realm))
            throw SvnFormatException.UnexpectedEndOfList();

        return new()
        {
            Mech = mech,
            Realm = realm
        };
    }
}
