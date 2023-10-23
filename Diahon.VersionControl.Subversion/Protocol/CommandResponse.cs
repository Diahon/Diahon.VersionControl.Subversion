using Diahon.VersionControl.Subversion.Primitives;
using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Protocol;

// command-response: ( success params:list ) | ( failure ( err:error ... ) )
public sealed class CommandResponse<T> : ISvnServerPrototype<CommandResponse<T>> where T : ISvnServerPrototype<T>
{
    public required SvnWord Status { get; init; }
    public required T Content { get; init; }

    public static CommandResponse<T> ReadContent(ref SvnReader reader)
    {
        if (!reader.TryReadPrimitiveOrEnd<SvnWord>(out var status))
            throw SvnFormatException.UnexpectedEndOfList();

        if (status.Value == "failure")
        {
            if (!reader.TryReadArrayOrEnd<SvnError>(out var errors))
                throw SvnFormatException.UnexpectedEndOfList();

            reader.ReadListEnd();

            if (errors.Count == 1)
                throw errors[0];

            throw new AggregateException(errors);
        }

        return new() { Status = status, Content = T.Read(ref reader) };
    }
}
