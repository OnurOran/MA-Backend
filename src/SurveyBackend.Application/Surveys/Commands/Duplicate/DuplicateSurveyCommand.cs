namespace SurveyBackend.Application.Surveys.Commands.Duplicate;

public sealed record DuplicateSurveyCommand(int SurveyId) : ICommand<int>;
