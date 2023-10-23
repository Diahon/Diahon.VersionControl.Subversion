using Diahon.VersionControl.Subversion.Primitives;
using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Protocol;

public sealed class Dirent : ISvnServerPrototype<Dirent>
{
    public required SvnString RelativePath { get; init; }
    public required SvnWord Kind { get; init; }

    public static Dirent ReadContent(ref SvnReader reader)
    {
        if (!reader.TryReadPrimitiveOrEnd<SvnString>(out var path))
            throw SvnFormatException.UnexpectedEndOfList();

        if (!reader.TryReadPrimitiveOrEnd<SvnWord>(out var kind))
            throw SvnFormatException.UnexpectedEndOfList();

        return new() { RelativePath = path, Kind = kind };
    }

    public const string KindDirectory = "dir";
    public const string KindFile = "file";
}
