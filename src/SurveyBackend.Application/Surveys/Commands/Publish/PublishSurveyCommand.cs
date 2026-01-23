namespace SurveyBackend.Application.Surveys.Commands.Publish;

public sealed record PublishSurveyCommand(int SurveyId, DateTime? StartDate, DateTime? EndDate) : ICommand<bool>;
