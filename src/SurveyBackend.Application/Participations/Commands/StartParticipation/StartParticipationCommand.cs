namespace SurveyBackend.Application.Participations.Commands.StartParticipation;

public sealed record StartParticipationCommand(int SurveyNumber, Guid? ExternalId) : ICommand<int>
{
    public string? IpAddress { get; init; }
}
