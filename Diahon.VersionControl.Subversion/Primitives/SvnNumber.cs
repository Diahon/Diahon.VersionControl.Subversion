namespace Diahon.VersionControl.Subversion.Primitives;

public record struct SvnNumber(long Value) : ISvnPrimitive<SvnNumber>
{
    public static SvnNumber Parse(SvnObject obj)
        => obj.GetNumber();

    public static implicit operator SvnNumber(long value)
        => new(value);
}
