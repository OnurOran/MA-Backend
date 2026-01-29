using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Invitations.DTOs;

namespace SurveyBackend.Application.Invitations.Queries.GetSurveyByToken;

public sealed record GetSurveyByTokenQuery(string Token) : ICommand<TokenSurveyDto>;
