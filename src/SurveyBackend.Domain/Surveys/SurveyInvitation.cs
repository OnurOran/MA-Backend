using SurveyBackend.Domain.Common;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Domain.Surveys;

public class SurveyInvitation : CommonEntity
{
    public int Id { get; private set; }
    public int SurveyId { get; private set; }
    public string Token { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public DeliveryMethod DeliveryMethod { get; private set; }
    public InvitationStatus Status { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? ViewedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public int? ParticipationId { get; private set; }

    public Survey Survey { get; private set; } = null!;
    public Participation? Participation { get; private set; }

    private SurveyInvitation()
    {
    }

    private SurveyInvitation(
        int surveyId,
        string token,
        string firstName,
        string lastName,
        string? email,
        string? phone,
        DeliveryMethod deliveryMethod)
    {
        SurveyId = surveyId;
        Token = token;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email?.Trim();
        Phone = phone?.Trim();
        DeliveryMethod = deliveryMethod;
        Status = InvitationStatus.Pending;
    }

    public static SurveyInvitation CreateForEmail(
        int surveyId,
        string token,
        string firstName,
        string lastName,
        string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required for email delivery method.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name is required.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name is required.", nameof(lastName));
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token is required.", nameof(token));
        }

        return new SurveyInvitation(surveyId, token, firstName, lastName, email, null, DeliveryMethod.Email);
    }

    public static SurveyInvitation CreateForSms(
        int surveyId,
        string token,
        string firstName,
        string lastName,
        string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("Phone is required for SMS delivery method.", nameof(phone));
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name is required.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name is required.", nameof(lastName));
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token is required.", nameof(token));
        }

        return new SurveyInvitation(surveyId, token, firstName, lastName, null, phone, DeliveryMethod.Sms);
    }

    public void MarkAsSent()
    {
        if (Status == InvitationStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot send a cancelled invitation.");
        }

        Status = InvitationStatus.Sent;
        SentAt = DateTime.UtcNow;
    }

    public void MarkAsViewed()
    {
        if (Status == InvitationStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot view a cancelled invitation.");
        }

        if (Status == InvitationStatus.Pending)
        {
            Status = InvitationStatus.Sent;
            SentAt = DateTime.UtcNow;
        }

        if (ViewedAt is null)
        {
            ViewedAt = DateTime.UtcNow;
        }

        if (Status == InvitationStatus.Sent)
        {
            Status = InvitationStatus.Viewed;
        }
    }

    public void MarkAsCompleted(int participationId)
    {
        if (Status == InvitationStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot complete a cancelled invitation.");
        }

        Status = InvitationStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        ParticipationId = participationId;
    }

    public void Cancel()
    {
        if (Status == InvitationStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel a completed invitation.");
        }

        Status = InvitationStatus.Cancelled;
    }

    public void ResetToPending()
    {
        if (Status == InvitationStatus.Completed || Status == InvitationStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot reset a completed or cancelled invitation.");
        }

        Status = InvitationStatus.Pending;
        SentAt = null;
    }

    public void LinkParticipation(int participationId)
    {
        ParticipationId = participationId;
    }

    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }
}
