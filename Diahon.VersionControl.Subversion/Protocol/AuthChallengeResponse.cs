using Diahon.VersionControl.Subversion.Serialization;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Diahon.VersionControl.Subversion.Protocol;

// challenge: ( step ( token:string ) ) | ( failure ( message:string ) ) | ( success [ token:string ] )
public sealed class AuthChallengeResponse : ISvnClientPrototype<AuthChallengeResponse>
{
    public required string Challenge { get; init; }
    public required NetworkCredential Credential { get; init; }

    public void WriteContent(ref SvnWriter writer)
    {
        writer.WriteString($"{Credential.UserName} {ComputeDigest()}");
    }

    // https://github.com/apache/subversion/blob/260d47d5813f86ed02272a73e1d84b035252c31c/subversion/libsvn_ra_svn/cram.c#L82-L110
    internal unsafe string ComputeDigest()
    {
        var encoding = Encoding.ASCII;

        Span<byte> digest = stackalloc byte[16];

        Span<byte> secret = stackalloc byte[64];
        if (encoding.GetByteCount(Credential.Password) > secret.Length)
            throw new InvalidOperationException("Password too long!"); // ToDo: Hash via md5

        encoding.GetBytes(Credential.Password, secret);

        for (int i = 0; i < secret.Length; i++)
            secret[i] ^= 0x36;

        var hash = IncrementalHash.CreateHash(HashAlgorithmName.MD5);
        hash.AppendData(secret);
        hash.AppendData(encoding.GetBytes(Challenge)); // ToDo: Less allocations / ArrayBuffer
        var hashSize = hash.GetHashAndReset(digest);
        Debug.Assert(hashSize == digest.Length);

        for (int i = 0; i < secret.Length; i++)
            secret[i] ^= (0x36 ^ 0x5c);

        hash.AppendData(secret);
        hash.AppendData(digest);
        hashSize = hash.GetHashAndReset(digest);
        Debug.Assert(hashSize == digest.Length);

        return Convert.ToHexString(digest).ToLower(); // ToDo: allocation-free ToLower
    }
}
