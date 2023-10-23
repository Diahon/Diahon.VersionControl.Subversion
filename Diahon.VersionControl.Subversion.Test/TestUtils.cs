using Diahon.VersionControl.Subversion.Primitives;

namespace Diahon.VersionControl.Subversion.Test;

internal static class TestUtils
{
    public static void AssertContent(this Stream stream, string expected)
    {
        stream.Position = 0;
        using StreamReader reader = new(stream);
        Assert.Equal(
            expected,
            actual: reader.ReadToEnd()
        );
    }

    public static void SetContent(this Stream stream, string content)
    {
        StreamWriter writer = new(stream) { AutoFlush = true };
        writer.Write(content);
        stream.Position = 0;
    }

    public static void AssertToken(this SvnObject token, SvnObjectKind kind)
    {
        Assert.Equal(kind, token.Kind);
    }

    public static void AssertToken(this SvnObject token, SvnObjectKind kind, string expected)
    {
        token.AssertToken(kind);
        Assert.Equal(expected, token.StringValue);
    }

    public static void AssertToken(this SvnObject token, SvnObjectKind kind, long expected)
    {
        token.AssertToken(kind);
        Assert.Equal(expected, token.NumberValue);
    }
}
