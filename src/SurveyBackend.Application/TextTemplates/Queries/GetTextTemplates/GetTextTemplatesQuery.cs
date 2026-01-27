using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.TextTemplates.DTOs;

namespace SurveyBackend.Application.TextTemplates.Queries.GetTextTemplates;

public sealed record GetTextTemplatesQuery(int DepartmentId)
    : ICommand<IReadOnlyCollection<TextTemplateDto>>, IDepartmentScopedCommand;
