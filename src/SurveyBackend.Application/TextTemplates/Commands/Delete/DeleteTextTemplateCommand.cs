using SurveyBackend.Application.Abstractions.Messaging;

namespace SurveyBackend.Application.TextTemplates.Commands.Delete;

public sealed record DeleteTextTemplateCommand(int Id) : ICommand<bool>;
