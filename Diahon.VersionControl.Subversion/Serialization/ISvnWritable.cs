namespace Diahon.VersionControl.Subversion.Serialization;

public interface ISvnWritable<TSelf> where TSelf : ISvnWritable<TSelf>
{
    void WriteContent(ref SvnWriter writer);
    void Write(ref SvnWriter writer)
    {
        writer.WriteListStart();
        WriteContent(ref writer);
        writer.WriteListEnd();
    }
}
