using SurveyBackend.Application.Modules.Auth.Commands.Admin;

namespace SurveyBackend.Application.Modules.Auth.Validators;

public sealed class UpdateLocalAdminPasswordCommandValidator : AbstractValidator<UpdateLocalAdminPasswordCommand>
{
    public UpdateLocalAdminPasswordCommandValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Şifre boş olamaz.")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
            .Matches(@"[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
            .Matches(@"[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
            .Matches(@"[0-9]").WithMessage("Şifre en az bir rakam içermelidir.")
            .Matches(@"[!@#$%^&*()_+\-=\[\]{};:',.<>?]").WithMessage("Şifre en az bir özel karakter içermelidir.");
    }
}
