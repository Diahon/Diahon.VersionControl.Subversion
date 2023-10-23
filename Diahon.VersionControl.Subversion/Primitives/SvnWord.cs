namespace Diahon.VersionControl.Subversion.Primitives;

// ToDo: Validation
public record struct SvnWord(string Value) : ISvnPrimitive<SvnWord>
{
    public static SvnWord Parse(SvnObject obj)
        => obj.GetWord();

    public static implicit operator ReadOnlySpan<char>(SvnWord word)
        => word.Value;
}
