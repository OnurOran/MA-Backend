using SurveyBackend.Application.Common.Models;

namespace SurveyBackend.Application.Interfaces.External;

public interface ILdapService
{
    Task<LdapUserInfo?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken);
}
