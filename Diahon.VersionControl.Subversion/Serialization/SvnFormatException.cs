namespace Diahon.VersionControl.Subversion.Serialization;

public sealed class SvnFormatException : Exception
{
    private SvnFormatException(string message) : base(message) { }

    public static Exception UnexpectedEndOfList()
        => new SvnFormatException("Unexpected end of list");

    public static Exception UnexpectedToken(object actual)
        => new SvnFormatException($"Unexpected token {actual}");

    public static Exception UnexpectedToken(object actual, object expected)
        => new SvnFormatException($"Unexpected token {actual}, expected {expected}");

    public static Exception ExpectedToken(object expected)
        => new SvnFormatException($"Expected token {expected}");
}
