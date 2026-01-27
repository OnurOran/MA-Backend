namespace SurveyBackend.Application.Surveys.Commands.Delete;

public sealed record DeleteSurveyCommand(int SurveyId) : ICommand<bool>;
