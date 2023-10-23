using Diahon.VersionControl.Subversion.Primitives;
using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Protocol;

// response: ( version:number ( cap:word ... ) url:string ? ra-client:string ( ? client:string ) )
public sealed class GreetingResponse : ISvnClientPrototype<GreetingResponse>
{
    public required SvnNumber Version { get; init; }

    public required IReadOnlyList<SvnWord> Capabilities { get; init; }

    public required string Url { get; init; }

    public void WriteContent(ref SvnWriter writer)
    {
        writer.WriteNumber(Version);
        writer.WriteListStart();
        foreach (var item in Capabilities)
        {
            writer.WriteWord(item);
        }
        writer.WriteListEnd();
        writer.WriteString(Url);
    }
}
