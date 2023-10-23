using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Protocol;

public interface ISvnServerPrototype<TSelf> : ISvnReadable<TSelf> where TSelf : ISvnServerPrototype<TSelf>
{

}
