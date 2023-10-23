namespace Diahon.VersionControl.Subversion.Protocol.Commands;

public interface ISvnCommand<TSelf> : ISvnClientPrototype<TSelf> where TSelf : ISvnCommand<TSelf>
{
    static abstract string CommandName { get; }
}
