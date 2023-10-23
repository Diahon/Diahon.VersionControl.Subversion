using Diahon.VersionControl.Subversion.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Diahon.VersionControl.Subversion.Serialization;

public unsafe readonly ref struct SvnReader(Stream stream)
{
    readonly SvnTokenizer _tokenizer = new(new SvnStreamReader(stream));
    static readonly Encoding Encoding = Encoding.UTF8;

    public SvnList ReadList()
    {
        int nestingLevel = 1;
        if (_tokenizer.Read().Kind != SvnObjectKind.ListStart)
            throw new InvalidDataException("Expected list start");

        while (nestingLevel > 0)
        {
            var token = _tokenizer.Read();
            if (token.Kind == SvnObjectKind.ListStart)
                nestingLevel++;
            else if (token.Kind == SvnObjectKind.ListEnd)
                nestingLevel--;
        }

        return default;
    }

    public void ReadListStart()
        => _tokenizer.Read().AssertKind(SvnObjectKind.ListStart);

    public void ReadListEnd()
        => _tokenizer.Read().AssertKind(SvnObjectKind.ListEnd);

    public bool TryReadDone()
    {
        var token = _tokenizer.Read();
        if (token.Kind != SvnObjectKind.Word)
            return false;

        return token.GetWord().Value == "done";
    }

    public bool TryReadPrimitiveOrEnd<TPrimitive>([MaybeNullWhen(false)] out TPrimitive value) where TPrimitive : struct, ISvnPrimitive<TPrimitive>
    {
        value = default;

        var token = _tokenizer.Read();
        if (token.Kind == SvnObjectKind.ListEnd)
            return false;

        value = TPrimitive.Parse(token);
        return true;
    }

    public delegate T ReadDelegate<T>(ref SvnReader reader, SvnObject currentToken);
    bool TryReadArrayOrEndInternal<T>([MaybeNullWhen(false)] out IReadOnlyList<T> value, ReadDelegate<T> read)
    {
        value = default;

        var token = _tokenizer.Read();
        if (token.Kind == SvnObjectKind.ListEnd)
            return false;

        token.AssertKind(SvnObjectKind.ListStart);

        List<T> list = new();
        while (true)
        {
            token = _tokenizer.Read();
            if (token.Kind == SvnObjectKind.ListEnd)
                break;

            var me = this;
            list.Add(read(ref me, token));
        }
        value = list;
        return true;
    }

    public bool TryReadPrimitiveArrayOrEnd<TPrimitive>([MaybeNullWhen(false)] out IReadOnlyList<TPrimitive> value) where TPrimitive : struct, ISvnPrimitive<TPrimitive>
        => TryReadArrayOrEndInternal(out value, (ref SvnReader reader, SvnObject currentToken) => TPrimitive.Parse(currentToken));

    public bool TryReadArrayOrEnd<T>([MaybeNullWhen(false)] out IReadOnlyList<T> value, bool readAsTuple = false) where T : class, ISvnReadable<T>
        => TryReadArrayOrEndInternal(out value, (ref SvnReader reader, SvnObject currentToken) => readAsTuple ? T.Read(ref reader) : T.ReadContent(ref reader));

    public T Read<T>() where T : ISvnReadable<T>
    {
        var me = this;
        return T.Read(ref me);
    }

    internal SvnObject ReadToken()
        => _tokenizer.Read();

    readonly ref struct SvnTokenizer(SvnStreamReader reader)
    {
        readonly StringBuilder _stringBuilder = new();
        readonly SvnStreamReader _reader = reader;

        internal SvnObject Read()
        {
            var currentChar = _reader.ReadChar();
            if (currentChar == SvnTokens.ListStart)
            {
                ExpectSeparator();
                return new() { Kind = SvnObjectKind.ListStart };
            }

            if (currentChar == SvnTokens.ListEnd)
            {
                ExpectSeparator();
                return new() { Kind = SvnObjectKind.ListEnd };
            }

            if (char.IsAsciiLetter(currentChar))
                return new()
                {
                    Kind = SvnObjectKind.Word,
                    StringValue = ReadRemainingWord(currentChar)
                };

            if (!char.IsAsciiDigit(currentChar))
                throw SvnFormatException.UnexpectedToken(currentChar, "digit");

            var number = ReadRemainingNumber(currentChar, out var isStringPrefix);
            if (!isStringPrefix)
                return new()
                {
                    Kind = SvnObjectKind.Number,
                    NumberValue = number
                };

            return new()
            {
                Kind = SvnObjectKind.String,
                StringValue = ReadStringValue((int)number.Value)
            };
        }

        private string ReadRemainingWord(char firstChar)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(firstChar);
            while (true)
            {
                var currentChar = _reader.ReadChar();
                if (SvnTokens.IsTokenSeparator(currentChar))
                    break;

                if (!char.IsAsciiLetterOrDigit(currentChar) && currentChar != '-')
                    throw new InvalidDataException($"Unexpected char '{currentChar}'");

                _stringBuilder.Append(currentChar);
            }
            return _stringBuilder.ToString();
        }

        private SvnNumber ReadRemainingNumber(char firstChar, out bool isStringPrefix)
        {
            isStringPrefix = false;

            _stringBuilder.Clear();
            _stringBuilder.Append(firstChar);
            while (true)
            {
                var currentChar = _reader.ReadChar();
                if (SvnTokens.IsTokenSeparator(currentChar))
                    break;

                if (currentChar == SvnTokens.StringSeparator)
                {
                    isStringPrefix = true;
                    break;
                }

                if (!char.IsAsciiDigit(currentChar))
                    throw new InvalidDataException($"Unexpected char '{currentChar}'");

                _stringBuilder.Append(currentChar);
            }
            return new(long.Parse(_stringBuilder.ToString())); // ToDo: Less allocations
        }

        [SkipLocalsInit]
        private string ReadStringValue(int length)
        {
            Span<byte> buffer = stackalloc byte[length]; // ToDo: StackOverflow?!
            _reader.Read(buffer);

            ExpectSeparator();
            return Encoding.GetString(buffer);
        }

        private void ExpectSeparator()
        {
            var actual = _reader.ReadChar();
            if (!SvnTokens.IsTokenSeparator(actual))
                throw new InvalidDataException($"Expected white space");
        }
    }

    readonly ref struct SvnStreamReader(Stream stream)
    {
        readonly Stream _stream = stream;

        public void Read(Span<byte> buffer)
        {
            int readBytes = 0;
            do
            {
                readBytes += _stream.Read(buffer[readBytes..]);
            } while (readBytes < buffer.Length);
        }

        public char ReadChar()
        {
            Span<byte> buffer = stackalloc byte[1];
            Read(buffer);
            return (char)buffer[0];
        }
    }
}
