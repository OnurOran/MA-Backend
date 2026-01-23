using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Commands.AddQuestion;

public sealed record AddQuestionCommand(int SurveyId, CreateQuestionDto Question) : ICommand<int>;
