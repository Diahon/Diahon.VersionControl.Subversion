using Diahon.VersionControl.Subversion.Primitives;
using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Protocol.Commands;

// params:   ( path:string [ rev:number ] want-props:bool want-contents:bool ? want-iprops:bool )
public sealed class GetFileCommand : ISvnCommand<GetFileCommand>
{
    public static string CommandName { get; } = "get-file";

    public required string Path { get; init; }
    public SvnNumber? Revision { get; init; }
    public required bool WantProps { get; init; }
    public required bool WantContents { get; init; }
    public bool WantIProps { get; init; } = false;

    public void WriteContent(ref SvnWriter writer)
    {
        writer.WriteString(Path);

        writer.WriteListStart();
        if (Revision != null)
            writer.WriteNumber(Revision.Value);
        writer.WriteListEnd();

        writer.WriteBool(WantProps);
        writer.WriteBool(WantContents);
        writer.WriteBool(WantIProps);
    }
}
