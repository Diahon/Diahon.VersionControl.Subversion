namespace Diahon.VersionControl.Subversion.Serialization;

internal static class SvnTokens
{
    public const byte ListStart = (byte)'(';
    public const byte ListEnd = (byte)')';

    public const byte TokenSpaceSeparator = (byte)' ';

    public const byte StringSeparator = (byte)':';

    public static bool IsTokenSeparator(char c)
        => c == ' ' || c == '\n';
}
