using System.Linq;
using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Surveys;

public class Answer : CommonEntity
{
    public int Id { get; private set; }
    public int ParticipationId { get; private set; }
    public int QuestionId { get; private set; }
    public string? TextValue { get; private set; }

    public Participation Participation { get; private set; } = null!;
    public Question Question { get; private set; } = null!;
    public AnswerAttachment? Attachment { get; private set; }
    public ICollection<AnswerOption> SelectedOptions { get; private set; } = new List<AnswerOption>();

    private Answer()
    {
    }

    internal Answer(int participationId, int questionId, string? textValue)
    {
        ParticipationId = participationId;
        QuestionId = questionId;
        TextValue = textValue?.Trim();
    }

    public AnswerOption AddSelectedOption(int questionOptionId)
    {
        var selection = new AnswerOption(Id, questionOptionId);
        SelectedOptions.Add(selection);
        return selection;
    }

    public void Update(string? textValue)
    {
        TextValue = textValue?.Trim();
    }

    public void ReplaceSelectedOptions(IEnumerable<int>? optionIds)
    {
        var newOptionIds = (optionIds ?? Enumerable.Empty<int>()).Distinct().ToList();

        var optionsToRemove = SelectedOptions
            .Where(ao => !newOptionIds.Contains(ao.QuestionOptionId))
            .ToList();

        foreach (var option in optionsToRemove)
        {
            SelectedOptions.Remove(option);
        }

        var existingOptionIds = SelectedOptions.Select(ao => ao.QuestionOptionId).ToHashSet();
        var optionIdsToAdd = newOptionIds.Where(oid => !existingOptionIds.Contains(oid));

        foreach (var optionId in optionIdsToAdd)
        {
            SelectedOptions.Add(new AnswerOption(Id, optionId));
        }
    }

    public void ReplaceMatrixAnswers(IEnumerable<(int OptionId, int ScaleValue, string? Explanation)> matrixAnswers)
    {
        var newAnswers = matrixAnswers.ToList();
        var newOptionIds = newAnswers.Select(a => a.OptionId).ToHashSet();

        // Remove options that are no longer in the new answers
        var optionsToRemove = SelectedOptions
            .Where(ao => !newOptionIds.Contains(ao.QuestionOptionId))
            .ToList();

        foreach (var option in optionsToRemove)
        {
            SelectedOptions.Remove(option);
        }

        // Update or add new answers
        foreach (var (optionId, scaleValue, explanation) in newAnswers)
        {
            var existing = SelectedOptions.FirstOrDefault(ao => ao.QuestionOptionId == optionId);
            if (existing != null)
            {
                existing.UpdateMatrixAnswer(scaleValue, explanation);
            }
            else
            {
                SelectedOptions.Add(new AnswerOption(Id, optionId, scaleValue, explanation));
            }
        }
    }
}
