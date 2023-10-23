namespace Diahon.VersionControl.Subversion.Primitives;

public interface ISvnPrimitive<TSelf> where TSelf : ISvnPrimitive<TSelf>
{
    static abstract TSelf Parse(SvnObject obj);
}
