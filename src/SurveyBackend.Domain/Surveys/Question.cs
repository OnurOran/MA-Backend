using SurveyBackend.Domain.Common;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Domain.Surveys;

public class Question : CommonEntity
{
    public int Id { get; private set; }
    public int SurveyId { get; private set; }
    public string Text { get; private set; } = null!;
    public string? Description { get; private set; }
    public int Order { get; private set; }
    public QuestionType Type { get; private set; }
    public bool IsRequired { get; private set; }
    public string? AllowedAttachmentContentTypes { get; private set; }

    // Matrix question type properties
    public string? MatrixScale1Label { get; private set; }
    public string? MatrixScale2Label { get; private set; }
    public string? MatrixScale3Label { get; private set; }
    public string? MatrixScale4Label { get; private set; }
    public string? MatrixScale5Label { get; private set; }
    public bool MatrixShowExplanation { get; private set; }
    public string? MatrixExplanationLabel { get; private set; }

    public Survey Survey { get; private set; } = null!;
    public Attachment? Attachment { get; private set; }
    public ICollection<QuestionOption> Options { get; private set; } = new List<QuestionOption>();

    private Question()
    {
    }

    private Question(int surveyId, string text, int order, QuestionType type, bool isRequired, string? description)
    {
        SurveyId = surveyId;
        Text = text;
        Description = description;
        Order = order;
        Type = type;
        IsRequired = isRequired;
    }

    internal static Question Create(int surveyId, string text, int order, QuestionType type, bool isRequired, string? description)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Question text cannot be empty.", nameof(text));
        }

        return new Question(surveyId, text.Trim(), order, type, isRequired, description?.Trim());
    }

    public void SetAllowedAttachmentContentTypes(IEnumerable<string>? contentTypes)
    {
        var list = contentTypes?
            .Where(ct => !string.IsNullOrWhiteSpace(ct))
            .Select(ct => ct.Trim())
            .Where(ct => ct.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        AllowedAttachmentContentTypes = list is { Count: > 0 }
            ? string.Join(',', list)
            : null;
    }

    public IReadOnlyCollection<string> GetAllowedAttachmentContentTypes()
    {
        if (string.IsNullOrWhiteSpace(AllowedAttachmentContentTypes))
        {
            return Array.Empty<string>();
        }

        return AllowedAttachmentContentTypes
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToArray();
    }

    public QuestionOption AddOption(string text, int order, int? value = null)
    {
        var option = QuestionOption.Create(Id, text, order, value);
        Options.Add(option);
        return option;
    }

    public void SetMatrixScaleLabels(string scale1, string scale2, string scale3, string scale4, string scale5)
    {
        MatrixScale1Label = scale1?.Trim();
        MatrixScale2Label = scale2?.Trim();
        MatrixScale3Label = scale3?.Trim();
        MatrixScale4Label = scale4?.Trim();
        MatrixScale5Label = scale5?.Trim();
    }

    public void SetMatrixExplanationSettings(bool showExplanation, string? explanationLabel)
    {
        MatrixShowExplanation = showExplanation;
        MatrixExplanationLabel = explanationLabel?.Trim();
    }

    public string[] GetMatrixScaleLabels()
    {
        return new[]
        {
            MatrixScale1Label ?? string.Empty,
            MatrixScale2Label ?? string.Empty,
            MatrixScale3Label ?? string.Empty,
            MatrixScale4Label ?? string.Empty,
            MatrixScale5Label ?? string.Empty
        };
    }
}
