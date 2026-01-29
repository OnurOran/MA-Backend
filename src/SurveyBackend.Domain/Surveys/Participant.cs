using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Surveys;

public class Participant : CommonEntity
{
    public int Id { get; private set; }
    public Guid? ExternalId { get; private set; }
    public string? LdapUsername { get; private set; }
    public int? InvitationId { get; private set; }

    public SurveyInvitation? Invitation { get; private set; }

    private Participant()
    {
    }

    private Participant(Guid? externalId, string? ldapUsername, int? invitationId = null)
    {
        ExternalId = externalId;
        LdapUsername = ldapUsername;
        InvitationId = invitationId;
    }

    public static Participant Create(Guid? externalId = null, string? ldapUsername = null)
    {
        if (externalId is null && string.IsNullOrWhiteSpace(ldapUsername))
        {
            throw new ArgumentException("Participant must have either an external id or an LDAP username.");
        }

        return new Participant(externalId, ldapUsername?.Trim());
    }

    public static Participant CreateAnonymous(Guid externalId)
    {
        return Create(externalId, null);
    }

    public static Participant CreateInternal(string ldapUsername)
    {
        return Create(null, ldapUsername);
    }

    public static Participant CreateFromInvitation(int invitationId)
    {
        return new Participant(Guid.NewGuid(), null, invitationId);
    }
}
