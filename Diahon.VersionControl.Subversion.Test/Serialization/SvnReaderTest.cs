using Diahon.VersionControl.Subversion.Primitives;
using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Test.Serialization;

public sealed class SvnReaderTest
{
    [Fact]
    public void ShouldReadCorrectExample()
    {
        using MemoryStream stream = new();
        stream.SetContent("( word 22 6:string ( sublist ) ) ");
        SvnReader reader = new(stream);

        reader.ReadToken().AssertToken(SvnObjectKind.ListStart);
        reader.ReadToken().AssertToken(SvnObjectKind.Word, "word");
        reader.ReadToken().AssertToken(SvnObjectKind.Number, 22);
        reader.ReadToken().AssertToken(SvnObjectKind.String, "string");
        reader.ReadToken().AssertToken(SvnObjectKind.ListStart);
        reader.ReadToken().AssertToken(SvnObjectKind.Word, "sublist");
        reader.ReadToken().AssertToken(SvnObjectKind.ListEnd);
        reader.ReadToken().AssertToken(SvnObjectKind.ListEnd);
    }
}
