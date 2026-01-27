using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Surveys;

public class DependentQuestion : CommonEntity
{
    public int Id { get; private set; }
    public int ParentQuestionOptionId { get; private set; }
    public int ChildQuestionId { get; private set; }

    public QuestionOption ParentOption { get; private set; } = null!;
    public Question ChildQuestion { get; private set; } = null!;

    private DependentQuestion()
    {
    }

    public DependentQuestion(QuestionOption parentOption, Question childQuestion)
    {
        ParentOption = parentOption;
        ChildQuestion = childQuestion;
    }
}
