using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Participations.Commands.CompleteParticipation;

public sealed class CompleteParticipationCommandHandler : ICommandHandler<CompleteParticipationCommand, bool>
{
    private readonly IParticipationRepository _participationRepository;
    private readonly ISurveyRepository _surveyRepository;

    public CompleteParticipationCommandHandler(
        IParticipationRepository participationRepository,
        ISurveyRepository surveyRepository)
    {
        _participationRepository = participationRepository;
        _surveyRepository = surveyRepository;
    }

    public async Task<bool> HandleAsync(CompleteParticipationCommand request, CancellationToken cancellationToken)
    {
        var participation = await _participationRepository.GetByIdAsync(request.ParticipationId, cancellationToken)
                          ?? throw new InvalidOperationException("Katılım bulunamadı.");

        var survey = await _surveyRepository.GetByIdAsync(participation.SurveyId, cancellationToken)
                   ?? throw new InvalidOperationException("Anket bulunamadı.");

        ValidateRequiredQuestions(survey, participation);

        participation.Complete(DateTime.Now);

        await _participationRepository.UpdateAsync(participation, cancellationToken);
        return true;
    }

    private static void ValidateRequiredQuestions(Domain.Surveys.Survey survey, Domain.Surveys.Participation participation)
    {
        var answeredQuestionIds = participation.Answers.Select(a => a.QuestionId).ToHashSet();
        var errors = new List<string>();

        var childQuestionIds = survey.Questions
            .SelectMany(q => q.Options)
            .SelectMany(o => o.DependentQuestions)
            .Select(dq => dq.ChildQuestionId)
            .ToHashSet();

        foreach (var question in survey.Questions.Where(q => !childQuestionIds.Contains(q.Id)))
        {
            if (!question.IsRequired) continue;

            if (!answeredQuestionIds.Contains(question.Id))
            {
                errors.Add($"Zorunlu soru cevaplanmadı: {question.Text}");
                continue;
            }

            if (question.Type == QuestionType.Conditional)
            {
                var answer = participation.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
                if (answer != null && answer.SelectedOptions.Any())
                {
                    var selectedOptionId = answer.SelectedOptions.First().QuestionOptionId;
                    var selectedOption = question.Options.FirstOrDefault(o => o.Id == selectedOptionId);

                    if (selectedOption != null)
                    {
                        foreach (var dependentQuestion in selectedOption.DependentQuestions)
                        {
                            var childQuestion = dependentQuestion.ChildQuestion;
                            if (childQuestion.IsRequired && !answeredQuestionIds.Contains(childQuestion.Id))
                            {
                                errors.Add($"Zorunlu soru cevaplanmadı: {childQuestion.Text}");
                            }
                        }
                    }
                }
            }
        }

        if (errors.Any())
        {
            throw new InvalidOperationException($"Anket tamamlanamadı:\n{string.Join("\n", errors)}");
        }
    }
}
