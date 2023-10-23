using Diahon.VersionControl.Subversion.Primitives;
using Diahon.VersionControl.Subversion.Serialization;
using System.Buffers;

namespace Diahon.VersionControl.Subversion.Protocol.Commands;

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

    public static ReadOnlyMemory<byte> ReadFileContents(ref SvnReader reader)
    {
        ArrayBufferWriter<byte> bufferWriter = new();

        int writtenBytes;
        do
        {
            reader.ReadStringBuffer(bufferWriter, out writtenBytes);
        } while (writtenBytes > 0);

        reader.Read<CommandResponse<EmptyResponse>>();

        return bufferWriter.WrittenMemory;
    }
}
