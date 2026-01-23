using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using SurveyBackend.Application.Common.Models;
using SurveyBackend.Application.Interfaces.External;
using SurveyBackend.Infrastructure.Configurations;

namespace SurveyBackend.Infrastructure.Directory;

public sealed class LdapService : ILdapService
{
    private readonly LdapSettings _settings;
    private readonly ILogger<LdapService> _logger;

    public LdapService(IOptions<LdapSettings> settings, ILogger<LdapService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public Task<LdapUserInfo?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("LDAP authentication failed: username or password is empty");
            return Task.FromResult<LdapUserInfo?>(null);
        }

        return Task.Run(() => AuthenticateInternal(username, password), cancellationToken);
    }

    private LdapUserInfo? AuthenticateInternal(string username, string password)
    {
        using var cn = new LdapConnection();

        try
        {
            _logger.LogInformation("Attempting LDAP connection to {LdapPath}:389 for user {Username}", _settings.Path, username);

            cn.Connect(_settings.Path, 389);
            var bindDn = $"{username}@{_settings.Domain}";

            _logger.LogDebug("Binding to LDAP with DN: {BindDn}", bindDn);
            cn.Bind(bindDn, password);

            _logger.LogInformation("LDAP bind successful for user {Username}", username);

            var searchFilter = $"(&({_settings.UsernameAttribute}={username})(objectClass=user))";
            _logger.LogDebug("Searching LDAP with filter: {SearchFilter} in base: {SearchBase}", searchFilter, _settings.SearchBase);

            var searchResults = cn.Search(
                _settings.SearchBase,
                2,
                searchFilter,
                new[] { _settings.EmailAttribute, _settings.DepartmentAttribute, "objectGuid" },
                false);

            if (searchResults.HasMore())
            {
                var entry = searchResults.Next();
                var email = entry.GetAttribute(_settings.EmailAttribute)?.StringValue ?? string.Empty;
                var department = entry.GetAttribute(_settings.DepartmentAttribute)?.StringValue ?? string.Empty;

                var userId = Guid.NewGuid();
                try
                {
                    var guidAttr = entry.GetAttribute("objectGuid");
                    if (guidAttr is not null)
                    {
                        userId = new Guid((byte[])(object)guidAttr.ByteValue);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse objectGuid for user {Username}, using generated GUID", username);
                }

                _logger.LogInformation("LDAP authentication successful for user {Username}, Department: {Department}", username, department);
                return new LdapUserInfo(userId, username, email, department);
            }

            _logger.LogWarning("LDAP search returned no results for user {Username}", username);
            return null;
        }
        catch (LdapException ex)
        {
            _logger.LogError(ex, "LDAP authentication failed for user {Username}. Error: {ErrorMessage}, ResultCode: {ResultCode}",
                username, ex.Message, ex.ResultCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during LDAP authentication for user {Username}: {ErrorMessage}",
                username, ex.Message);
            return null;
        }
    }
}
