namespace SurveyBackend.Application.Interfaces.Security;

public interface ITokenGenerator
{
    string GenerateInvitationToken();
}
