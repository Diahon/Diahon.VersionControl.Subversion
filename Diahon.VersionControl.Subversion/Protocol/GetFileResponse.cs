using Diahon.VersionControl.Subversion.Primitives;
using Diahon.VersionControl.Subversion.Serialization;
using System.Text;

namespace Diahon.VersionControl.Subversion.Protocol;

// response: ( [ checksum:string ] rev:number props:proplist [ inherited-props:iproplist ] )
public sealed class GetFileResponse : ISvnServerPrototype<GetFileResponse>
{
    public SvnString? Checksum { get; init; }
    public required SvnNumber Revision { get; init; }

    public static GetFileResponse ReadContent(ref SvnReader reader)
    {
        reader.ReadListStart();
        if (reader.TryReadPrimitiveOrEnd<SvnString>(out var checksum))
            reader.ReadListEnd();

        if (!reader.TryReadPrimitiveOrEnd<SvnNumber>(out var revision))
            throw SvnFormatException.UnexpectedEndOfList();

        // propList
        _ = reader.ReadList();

        return new()
        {
            Checksum = checksum,
            Revision = revision,
        };
    }

    public static string ReadFileContents(ref SvnReader reader)
    {
        StringBuilder builder = new();

        SvnString str;
        do
        {
            if (!reader.TryReadPrimitiveOrEnd(out str))
                throw SvnFormatException.UnexpectedEndOfList();

            builder.Append(str.Value);
        } while (str.Value.Length > 0);

        reader.Read<CommandResponse<EmptyResponse>>();

        return builder.ToString();
    }
}
