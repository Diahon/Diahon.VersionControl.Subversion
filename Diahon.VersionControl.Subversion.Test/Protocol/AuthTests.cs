using Diahon.VersionControl.Subversion.Protocol;

namespace Diahon.VersionControl.Subversion.Test.Protocol;

public sealed class AuthTests
{
    [Theory]
    [InlineData("<17938105980017839451.1697575297746638@dh-svn>", "123", "9c97db98d39920e270f7d0bfbd926cc8")]
    public void CalculateCorrectDigest(string challenge, string password, string expected)
    {
        AuthChallengeResponse challengeResponse = new()
        {
            Challenge = challenge,
            Credential = new System.Net.NetworkCredential("USER", password)
        };

        var digest = challengeResponse.ComputeDigest().ToLower();

        Assert.Equal(expected, digest);
    }
}
