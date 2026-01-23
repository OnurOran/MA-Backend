namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record SurveyReportDto
{
    public int SurveyId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? IntroText { get; init; }
    public string? OutroText { get; init; }
    public string AccessType { get; init; } = string.Empty;
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool IsActive { get; init; }
    public int TotalParticipations { get; init; }
    public int CompletedParticipations { get; init; }
    public double CompletionRate { get; init; }
    public IReadOnlyList<ParticipantSummaryDto> Participants { get; init; } = Array.Empty<ParticipantSummaryDto>();
    public IReadOnlyList<QuestionReportDto> Questions { get; init; } = Array.Empty<QuestionReportDto>();
    public AttachmentDto? Attachment { get; init; }
}

public sealed record ParticipantSummaryDto
{
    public int ParticipationId { get; init; }
    public string? ParticipantName { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime StartedAt { get; init; }
}

public sealed record QuestionReportDto
{
    public int QuestionId { get; init; }
    public string Text { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public int Order { get; init; }
    public bool IsRequired { get; init; }
    public int TotalResponses { get; init; }
    public double ResponseRate { get; init; }
    public AttachmentDto? Attachment { get; init; }

    public IReadOnlyList<OptionResultDto>? OptionResults { get; init; }

    public IReadOnlyList<TextResponseDto>? TextResponses { get; init; }

    public IReadOnlyList<FileResponseDto>? FileResponses { get; init; }

    public IReadOnlyList<ConditionalBranchResultDto>? ConditionalResults { get; init; }

    // Matrix question type properties
    public IReadOnlyList<string>? MatrixScaleLabels { get; init; }
    public IReadOnlyList<MatrixRowResultDto>? MatrixResults { get; init; }
}

public sealed record OptionResultDto
{
    public int OptionId { get; init; }
    public string Text { get; init; } = string.Empty;
    public int Order { get; init; }
    public int SelectionCount { get; init; }
    public double Percentage { get; init; }
    public AttachmentDto? Attachment { get; init; }
}

public sealed record TextResponseDto
{
    public int ParticipationId { get; init; }
    public string? ParticipantName { get; init; }
    public string TextValue { get; init; } = string.Empty;
    public DateTime SubmittedAt { get; init; }
}

public sealed record FileResponseDto
{
    public int AnswerId { get; init; }
    public int AttachmentId { get; init; }
    public int ParticipationId { get; init; }
    public string? ParticipantName { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long SizeBytes { get; init; }
    public DateTime SubmittedAt { get; init; }
}

public sealed record ConditionalBranchResultDto
{
    public int ParentOptionId { get; init; }
    public string ParentOptionText { get; init; } = string.Empty;
    public int ParticipantCount { get; init; }
    public IReadOnlyList<QuestionReportDto> ChildQuestions { get; init; } = Array.Empty<QuestionReportDto>();
}

public sealed record MatrixRowResultDto
{
    public int OptionId { get; init; }
    public string Text { get; init; } = string.Empty;
    public int Order { get; init; }
    public int TotalResponses { get; init; }
    public double AverageScore { get; init; }
    public IReadOnlyList<int> ScaleDistribution { get; init; } = Array.Empty<int>();
    public IReadOnlyList<MatrixRowExplanationDto> Explanations { get; init; } = Array.Empty<MatrixRowExplanationDto>();
}

public sealed record MatrixRowExplanationDto
{
    public int ParticipationId { get; init; }
    public string? ParticipantName { get; init; }
    public int ScaleValue { get; init; }
    public string Explanation { get; init; } = string.Empty;
    public DateTime SubmittedAt { get; init; }
}

public sealed record ParticipantResponseDto
{
    public int ParticipationId { get; init; }
    public string? ParticipantName { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public IReadOnlyList<ParticipantAnswerDto> Answers { get; init; } = Array.Empty<ParticipantAnswerDto>();
}

public sealed record ParticipantAnswerDto
{
    public int QuestionId { get; init; }
    public string QuestionText { get; init; } = string.Empty;
    public string? TextValue { get; init; }
    public IReadOnlyList<string> SelectedOptions { get; init; } = Array.Empty<string>();
    public string? FileName { get; init; }
    public int? AnswerId { get; init; }
    public IReadOnlyList<MatrixAnswerDetailDto>? MatrixAnswers { get; init; }
}

public sealed record MatrixAnswerDetailDto
{
    public string RowText { get; init; } = string.Empty;
    public int ScaleValue { get; init; }
    public string? Explanation { get; init; }
}
