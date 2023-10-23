using Diahon.VersionControl.Subversion.Primitives;
using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Protocol;

// auth-response: ( mech:word [ token:string ] )
public sealed class AuthResponse : ISvnClientPrototype<AuthResponse>
{
    public required SvnWord Mech { get; init; }
    public string? Token { get; init; }

    public void WriteContent(ref SvnWriter writer)
    {
        writer.WriteWord(Mech);

        writer.WriteListStart();
        if (Token != null)
            writer.WriteString(Token);
        writer.WriteListEnd();
    }
}
