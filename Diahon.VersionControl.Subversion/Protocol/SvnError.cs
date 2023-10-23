using Diahon.VersionControl.Subversion.Primitives;
using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Protocol;

// error: ( apr-err:number message:string file:string line:number )
public sealed class SvnError : Exception, ISvnServerPrototype<SvnError>
{
    public override string? StackTrace { get; }

    private SvnError(int aprError, string message, string file, int line) : base(message)
    {
        HResult = aprError;
        StackTrace = $"{file}:{line}";
    }

    public static SvnError ReadContent(ref SvnReader reader)
    {
        if (!reader.TryReadPrimitiveOrEnd<SvnNumber>(out var aprError))
            throw SvnFormatException.UnexpectedEndOfList();

        if (!reader.TryReadPrimitiveOrEnd<SvnString>(out var message))
            throw SvnFormatException.UnexpectedEndOfList();

        if (!reader.TryReadPrimitiveOrEnd<SvnString>(out var file))
            throw SvnFormatException.UnexpectedEndOfList();

        if (!reader.TryReadPrimitiveOrEnd<SvnNumber>(out var line))
            throw SvnFormatException.UnexpectedEndOfList();

        return new((int)aprError.Value, message.Value, file.Value, (int)line.Value);
    }
}
