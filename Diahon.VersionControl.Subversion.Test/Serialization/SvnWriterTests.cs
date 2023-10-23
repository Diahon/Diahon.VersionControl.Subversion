using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Test.Serialization;

public sealed class SvnWriterTests
{
    [Fact]
    public void ShouldProduceCorrectExample()
    {
        using MemoryStream stream = new();
        SvnWriter writer = new(stream);

        writer.WriteListStart();
        writer.WriteWord("word");
        writer.WriteNumber(22);
        writer.WriteString("string");

        writer.WriteListStart();
        writer.WriteWord("sublist");
        writer.WriteListEnd();

        writer.WriteListEnd();

        stream.AssertContent("( word 22 6:string ( sublist ) ) ");
    }
}