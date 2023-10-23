using Diahon.VersionControl.Subversion.Primitives;
using Diahon.VersionControl.Subversion.Protocol.Commands;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Diahon.VersionControl.Subversion.Serialization;

public unsafe readonly ref struct SvnWriter(Stream stream)
{
    readonly Stream _stream = stream;

    public void WriteCommand<TCommand>(TCommand command) where TCommand : ISvnCommand<TCommand>
    {
        WriteListStart();

        WriteWord(TCommand.CommandName);

        var me = this;
        command.Write(ref me);

        WriteListEnd();
    }

    public void Write<T>(T value) where T : ISvnWritable<T>
    {
        var me = this;
        value.Write(ref me);
    }

    public void WriteListStart()
    {
        Span<byte> data = stackalloc[] { SvnTokens.ListStart, SvnTokens.TokenSpaceSeparator };
        _stream.Write(data);
    }

    public void WriteListEnd()
    {
        Span<byte> data = stackalloc[] { SvnTokens.ListEnd, SvnTokens.TokenSpaceSeparator };
        _stream.Write(data);
    }

    public void WriteWord(ReadOnlySpan<char> word)
    {
        if (word.Length == 0)
            throw new ArgumentException("Invalid length", nameof(word));

        if (!char.IsAsciiLetter(word[0]))
            throw new ArgumentException("Invalid word", nameof(word));

        WriteWordLessChecksNoSeparator(word);
        _stream.WriteByte(SvnTokens.TokenSpaceSeparator);
    }

    public void WriteNumber(SvnNumber number)
    {
        WriteNumberNoSeparator((ulong)number.Value);
        _stream.WriteByte(SvnTokens.TokenSpaceSeparator);
    }

    [SkipLocalsInit]
    public void WriteString(ReadOnlySpan<char> str, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;

        var bufferSize = encoding.GetByteCount(str);
        Span<byte> buffer = stackalloc byte[bufferSize];
        var encodedByteCount = encoding.GetBytes(str, buffer);

        Debug.Assert(encodedByteCount == bufferSize);

        WriteNumberNoSeparator((ulong)bufferSize);
        _stream.WriteByte(SvnTokens.StringSeparator);
        _stream.Write(buffer);
        _stream.WriteByte(SvnTokens.TokenSpaceSeparator);
    }

    public void WriteBool(bool value)
    {
        WriteWord(value ? "true" : "false");
    }

    [SkipLocalsInit]
    void WriteNumberNoSeparator(ulong digit)
    {
        Span<char> buffer = stackalloc char[20];
        if (!digit.TryFormat(buffer, out var charsWritten))
            throw new InvalidOperationException("Could not format digit");

        WriteWordLessChecksNoSeparator(buffer[0..charsWritten]);
    }

    void WriteWordLessChecksNoSeparator(ReadOnlySpan<char> word)
    {
        Span<byte> asciiWord = stackalloc byte[word.Length]; // ToDo: StackOverflow?!
        for (int i = 0; i < asciiWord.Length; i++)
        {
            char currentChar = word[i];

            if (!char.IsAsciiLetterOrDigit(currentChar) && currentChar != '-')
                throw new ArgumentException("Invalid word", nameof(word));

            asciiWord[i] = (byte)currentChar;
        }

        _stream.Write(asciiWord);
    }
}
