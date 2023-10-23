namespace Diahon.VersionControl.Subversion.Serialization;

public interface ISvnReadable<TSelf> where TSelf : ISvnReadable<TSelf>
{
    static abstract TSelf ReadContent(ref SvnReader reader);
    static virtual TSelf Read(ref SvnReader reader)
    {
        reader.ReadListStart();
        var result = TSelf.ReadContent(ref reader);
        reader.ReadListEnd();

        return result;
    }
}
