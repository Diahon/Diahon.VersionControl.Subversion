using Diahon.VersionControl.Subversion.Primitives;
using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Protocol;

// greeting: ( minver:number maxver:number mechs:list ( cap:word ... ) )
public sealed partial class Greeting : ISvnServerPrototype<Greeting>
{
    public required SvnNumber MinVersion { get; init; }
    public required SvnNumber MaxVersion { get; init; }

    public required SvnList Mechs { get; init; }

    public required IReadOnlyList<SvnWord> Capabilities { get; init; }

    public static Greeting ReadContent(ref SvnReader reader)
    {
        if (!reader.TryReadPrimitiveOrEnd<SvnNumber>(out var MinVersion))
            throw SvnFormatException.UnexpectedEndOfList();

        if (!reader.TryReadPrimitiveOrEnd<SvnNumber>(out var MaxVersion))
            throw SvnFormatException.UnexpectedEndOfList();

        var Mechs = reader.ReadList();

        if (!reader.TryReadPrimitiveArrayOrEnd<SvnWord>(out var Capabilities))
            throw SvnFormatException.UnexpectedEndOfList();

        return new()
        {
            MinVersion = MinVersion,
            MaxVersion = MaxVersion,
            Mechs = Mechs,
            Capabilities = Capabilities
        };
    }
}
