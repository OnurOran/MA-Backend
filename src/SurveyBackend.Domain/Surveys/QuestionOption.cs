using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Surveys;

public class QuestionOption : CommonEntity
{
    public int Id { get; private set; }
    public int QuestionId { get; private set; }
    public string Text { get; private set; } = null!;
    public int Order { get; private set; }
    public int? Value { get; private set; }

    public Question Question { get; private set; } = null!;
    public Attachment? Attachment { get; private set; }
    public ICollection<DependentQuestion> DependentQuestions { get; private set; } = new List<DependentQuestion>();

    private QuestionOption()
    {
    }

    private QuestionOption(int questionId, string text, int order, int? value)
    {
        QuestionId = questionId;
        Text = text;
        Order = order;
        Value = value;
    }

    internal static QuestionOption Create(int questionId, string text, int order, int? value)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Question option text cannot be empty.", nameof(text));
        }

        return new QuestionOption(questionId, text.Trim(), order, value);
    }

    public DependentQuestion AddDependentQuestion(int childQuestionId)
    {
        var dependentQuestion = new DependentQuestion(Id, childQuestionId);
        DependentQuestions.Add(dependentQuestion);
        return dependentQuestion;
    }
}
