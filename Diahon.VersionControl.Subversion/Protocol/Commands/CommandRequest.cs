using Diahon.VersionControl.Subversion.Serialization;

namespace Diahon.VersionControl.Subversion.Protocol.Commands;

public class CommandRequest<TCommand> where TCommand : ISvnCommand<TCommand>
{
    public void WriteContent(ref SvnWriter writer, TCommand command)
    {
        writer.WriteWord(TCommand.CommandName);

        command.Write(ref writer);
    }
}
