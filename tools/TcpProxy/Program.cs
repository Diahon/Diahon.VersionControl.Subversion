// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;

// https://svn.apache.org/repos/asf/subversion/trunk/subversion/libsvn_ra_svn/protocol

const int SvnPort = 3690;
TcpListener listener = new(System.Net.IPAddress.Loopback, SvnPort);
listener.Start();

long _id = 0;

while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    _ = Task.Run(async () =>
    {
        TcpClient remoteClient = new();
        await remoteClient.ConnectAsync("10.10.1.33", SvnPort);

        PipeStream(remoteClient.GetStream(), client.GetStream(), writeToConsole: true);
        PipeStream(client.GetStream(), remoteClient.GetStream());
    });
}

void PipeStream(Stream src, Stream target, bool writeToConsole = false)
{
    long id = Interlocked.Increment(ref _id);
    List<byte> data = new();
    Task.Run(() =>
    {
        try
        {
            while (true)
            {
                var currentData = (byte)src.ReadByte();
                data.Add(currentData);
                target.WriteByte(currentData);
                if (writeToConsole)
                    Console.Write((char)currentData);
            }
        }
        catch (Exception) { }

        File.WriteAllBytes($"{id}.txt", data.ToArray());
    });
}