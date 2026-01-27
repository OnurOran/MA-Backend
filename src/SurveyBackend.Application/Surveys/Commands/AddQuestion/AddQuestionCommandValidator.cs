using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.Commands.AddQuestion;

public sealed class AddQuestionCommandValidator : AbstractValidator<AddQuestionCommand>
{
    public AddQuestionCommandValidator()
    {
        RuleFor(x => x.SurveyId)
            .NotEmpty();

        RuleFor(x => x.Question)
            .NotNull();

        When(x => x.Question is not null, () =>
        {
            RuleFor(x => x.Question!.Text)
                .NotEmpty()
                .WithMessage("Soru metni gereklidir.");

            When(x => x.Question!.Options is not null, () =>
            {
                RuleForEach(x => x.Question!.Options!)
                    .ChildRules(option =>
                    {
                        option.RuleFor(o => o.Text)
                            .NotEmpty()
                            .WithMessage("Seçenek metni gereklidir.");
                    });
            });

            // Conditional question validation
            When(x => x.Question!.Type == QuestionType.Conditional, () =>
            {
                RuleFor(x => x.Question!.Options)
                    .NotNull()
                    .WithMessage("Koşullu sorular için seçenekler gereklidir.")
                    .Must(options => options is not null && options.Count >= 2 && options.Count <= 5)
                    .WithMessage("Koşullu sorular 2 ile 5 arasında seçenek içermelidir.");

                // Validate child questions if present
                When(x => x.Question!.ChildQuestions is not null && x.Question!.ChildQuestions.Count > 0, () =>
                {
                    RuleForEach(x => x.Question!.ChildQuestions!)
                        .ChildRules(child =>
                        {
                            child.RuleFor(c => c.Text)
                                .NotEmpty()
                                .WithMessage("Alt soru metni gereklidir.");

                            child.RuleFor(c => c.Type)
                                .NotEqual(QuestionType.Conditional)
                                .WithMessage("Alt sorular Koşullu tip olamaz.");

                            child.RuleFor(c => c.Type)
                                .NotEqual(QuestionType.Matrix)
                                .WithMessage("Alt sorular Matris tip olamaz.");

                            // Validate child question options for SingleSelect/MultiSelect
                            child.When(c => c.Type == QuestionType.SingleSelect || c.Type == QuestionType.MultiSelect, () =>
                            {
                                child.RuleFor(c => c.Options)
                                    .NotNull()
                                    .WithMessage("Seçimli alt sorular için seçenekler gereklidir.")
                                    .Must(options => options is not null && options.Count >= 2)
                                    .WithMessage("Seçimli alt sorular en az 2 seçenek içermelidir.");
                            });
                        });
                });
            });

            // SingleSelect/MultiSelect validation
            When(x => x.Question!.Type == QuestionType.SingleSelect || x.Question!.Type == QuestionType.MultiSelect, () =>
            {
                RuleFor(x => x.Question!.Options)
                    .NotNull()
                    .WithMessage("Seçimli sorular için seçenekler gereklidir.")
                    .Must(options => options is not null && options.Count >= 2 && options.Count <= 10)
                    .WithMessage("Seçimli sorular 2 ile 10 arasında seçenek içermelidir.");
            });

            // Matrix question validation
            When(x => x.Question!.Type == QuestionType.Matrix, () =>
            {
                RuleFor(x => x.Question!.MatrixScaleLabels)
                    .NotNull()
                    .WithMessage("Matrix soruları için ölçek etiketleri gereklidir.")
                    .Must(labels => labels is not null && labels.Count == 5)
                    .WithMessage("Matrix soruları için 5 adet ölçek etiketi gereklidir.");

                RuleFor(x => x.Question!.Options)
                    .NotNull()
                    .WithMessage("Matrix soruları için satırlar gereklidir.")
                    .Must(options => options is not null && options.Count >= 2 && options.Count <= 20)
                    .WithMessage("Matrix soruları için 2-20 arasında satır gereklidir.");
            });
        });
    }
}
