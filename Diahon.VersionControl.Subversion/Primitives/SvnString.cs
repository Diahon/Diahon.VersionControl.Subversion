namespace Diahon.VersionControl.Subversion.Primitives;

// ToDo: Validation
public record struct SvnString(string Value) : ISvnPrimitive<SvnString>
{
    public static SvnString Parse(SvnObject obj)
        => obj.GetString();

    public static implicit operator ReadOnlySpan<char>(SvnString word)
        => word.Value;
}
