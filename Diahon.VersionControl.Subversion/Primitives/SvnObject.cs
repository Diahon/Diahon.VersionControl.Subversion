using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Primitives;

public readonly struct SvnObject : ISvnPrimitive<SvnObject>
{
    public required SvnObjectKind Kind { get; init; }
    internal string? StringValue { get; init; }
    internal SvnNumber? NumberValue { get; init; }

    public bool IsPrimitive => Kind is SvnObjectKind.Word or SvnObjectKind.Number or SvnObjectKind.String;

    public SvnNumber GetNumber()
    {
        AssertKind(SvnObjectKind.Number);
        return NumberValue ?? throw new NullReferenceException("Value was null");
    }

    public SvnWord GetWord()
    {
        AssertKind(SvnObjectKind.Word);
        return new(StringValue ?? throw new NullReferenceException("Value was null"));
    }

    public SvnString GetString()
    {
        AssertKind(SvnObjectKind.String);
        return new(StringValue ?? throw new NullReferenceException("Value was null"));
    }

    internal void AssertKind(SvnObjectKind expected)
    {
        if (Kind != expected)
            throw SvnFormatException.ExpectedToken(expected);
    }

    static SvnObject ISvnPrimitive<SvnObject>.Parse(SvnObject obj)
        => obj;
}
