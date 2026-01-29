using System.Security.Cryptography;
using SurveyBackend.Application.Interfaces.Security;

namespace SurveyBackend.Infrastructure.Security;

public sealed class InvitationTokenGenerator : ITokenGenerator
{
    private const int TokenLength = 8;
    private const string AllowedChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public string GenerateInvitationToken()
    {
        var tokenChars = new char[TokenLength];
        var randomBytes = new byte[TokenLength];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        for (var i = 0; i < TokenLength; i++)
        {
            tokenChars[i] = AllowedChars[randomBytes[i] % AllowedChars.Length];
        }

        return new string(tokenChars);
    }
}
