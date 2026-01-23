using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Surveys;

public class AnswerOption : CommonEntity
{
    public int Id { get; private set; }
    public int AnswerId { get; private set; }
    public int QuestionOptionId { get; private set; }

    // Matrix question type properties
    public int? ScaleValue { get; private set; }
    public string? Explanation { get; private set; }

    public Answer Answer { get; private set; } = null!;
    public QuestionOption QuestionOption { get; private set; } = null!;

    private AnswerOption()
    {
    }

    public AnswerOption(int answerId, int questionOptionId)
    {
        AnswerId = answerId;
        QuestionOptionId = questionOptionId;
    }

    public AnswerOption(int answerId, int questionOptionId, int scaleValue, string? explanation)
    {
        AnswerId = answerId;
        QuestionOptionId = questionOptionId;
        ScaleValue = scaleValue;
        Explanation = explanation?.Trim();
    }

    public void UpdateMatrixAnswer(int scaleValue, string? explanation)
    {
        ScaleValue = scaleValue;
        Explanation = explanation?.Trim();
    }
}
