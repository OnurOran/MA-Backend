namespace SurveyBackend.Application.Participations.Commands.SubmitAnswer;

public sealed class SubmitAnswerCommandValidator : AbstractValidator<SubmitAnswerCommand>
{
    private const int MaxTextAnswerLength = 2000;

    public SubmitAnswerCommandValidator()
    {
        RuleFor(x => x.ParticipationId)
            .NotEmpty();

        RuleFor(x => x.QuestionId)
            .NotEmpty();

        // Note: We don't validate "at least one answer" here because:
        // - Required question validation is handled in the handler (which has access to question.IsRequired)
        // - Non-required questions can have empty answers
        // - The frontend now skips submission entirely for non-required questions with no answer

        RuleFor(x => x.TextValue)
            .MaximumLength(MaxTextAnswerLength)
            .WithMessage($"Text answer cannot exceed {MaxTextAnswerLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.TextValue));
    }
}
