namespace SurveyBackend.Domain.Enums;

public enum DeliveryMethod
{
    Email,
    Sms
}

public enum InvitationStatus
{
    Pending,
    Sent,
    Viewed,
    Completed,
    Cancelled
}
