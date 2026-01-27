using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.TextTemplates.DTOs;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.TextTemplates.Queries.GetTextTemplatesByType;

public sealed record GetTextTemplatesByTypeQuery(int DepartmentId, TextTemplateType Type)
    : ICommand<IReadOnlyCollection<TextTemplateDto>>, IDepartmentScopedCommand;
