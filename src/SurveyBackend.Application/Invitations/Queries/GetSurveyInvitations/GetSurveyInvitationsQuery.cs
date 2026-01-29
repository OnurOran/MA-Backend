using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Invitations.DTOs;

namespace SurveyBackend.Application.Invitations.Queries.GetSurveyInvitations;

public sealed record GetSurveyInvitationsQuery(int SurveyId) : ICommand<IReadOnlyList<InvitationDto>>;
