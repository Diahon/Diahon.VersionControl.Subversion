using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Protocol;

public interface ISvnClientPrototype<TSelf> : ISvnWritable<TSelf> where TSelf : ISvnClientPrototype<TSelf> { }
