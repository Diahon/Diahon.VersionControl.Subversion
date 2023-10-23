using Diahon.VersionControl.Subversion.Primitives;
using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Protocol.Commands;

public sealed class ListCommand : ISvnCommand<ListCommand>
{
    public static string CommandName { get; } = "list";

    public required string Path { get; init; }
    public SvnNumber? Revision { get; init; }
    public required SvnWord Depth { get; init; }
    public IReadOnlyList<SvnWord> Fields { get; init; } = Array.Empty<SvnWord>();
    public IReadOnlyList<string>? Patterns { get; init; }

    public void WriteContent(ref SvnWriter writer)
    {
        writer.WriteString(Path);

        writer.WriteListStart();
        if (Revision != null)
            writer.WriteNumber(Revision.Value);
        writer.WriteListEnd();

        writer.WriteWord(Depth);

        writer.WriteListStart();
        foreach (var field in Fields)
        {
            writer.WriteWord(field);
        }
        writer.WriteListEnd();

        if (Patterns != null && Patterns.Count > 0)
        {
            writer.WriteListStart();
            foreach (var pattern in Patterns)
            {
                writer.WriteString(pattern);
            }
            writer.WriteListEnd();
        }
    }

    public static readonly SvnWord DepthInfinity = new("infinity");
    public static readonly SvnWord DepthImmediates = new("immediates");
    public static readonly SvnWord DepthFiles = new("files");

    public static readonly SvnWord FieldKind = new("kind");
}
