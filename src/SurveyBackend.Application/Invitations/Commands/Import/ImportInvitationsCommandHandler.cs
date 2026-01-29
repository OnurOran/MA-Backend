using SurveyBackend.Application.Abstractions.Messaging;
using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Import;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Interfaces.Security;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Invitations.Commands.Import;

public sealed class ImportInvitationsCommandHandler : ICommandHandler<ImportInvitationsCommand, int>
{
    private readonly ISurveyInvitationRepository _invitationRepository;
    private readonly ISurveyRepository _surveyRepository;
    private readonly IExcelImportService _excelImportService;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;

    public ImportInvitationsCommandHandler(
        ISurveyInvitationRepository invitationRepository,
        ISurveyRepository surveyRepository,
        IExcelImportService excelImportService,
        ITokenGenerator tokenGenerator,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService)
    {
        _invitationRepository = invitationRepository;
        _surveyRepository = surveyRepository;
        _excelImportService = excelImportService;
        _tokenGenerator = tokenGenerator;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
    }

    public async Task<int> HandleAsync(ImportInvitationsCommand command, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
        {
            throw new UnauthorizedAccessException("Kullanıcı oturumu bulunamadı.");
        }

        var survey = await _surveyRepository.GetByIdAsync(command.SurveyId, cancellationToken)
            ?? throw new InvalidOperationException("Anket bulunamadı.");

        await _authorizationService.EnsureDepartmentScopeAsync(survey.DepartmentId, cancellationToken);

        if (survey.AccessType != AccessType.InvitationOnly)
        {
            throw new InvalidOperationException("Bu anket davetiye bazlı değil.");
        }

        var rows = await _excelImportService.ParseInvitationsAsync(command.ExcelStream, cancellationToken);

        if (rows.Count == 0)
        {
            throw new InvalidOperationException("Excel dosyasında geçerli kayıt bulunamadı.");
        }

        var invitations = new List<SurveyInvitation>();
        var usedTokens = new HashSet<string>();

        foreach (var row in rows)
        {
            var token = await GenerateUniqueTokenAsync(usedTokens, cancellationToken);
            usedTokens.Add(token);

            SurveyInvitation invitation;
            if (row.DeliveryMethod == DeliveryMethod.Email)
            {
                invitation = SurveyInvitation.CreateForEmail(
                    command.SurveyId,
                    token,
                    row.FirstName,
                    row.LastName,
                    row.Email!);
            }
            else
            {
                invitation = SurveyInvitation.CreateForSms(
                    command.SurveyId,
                    token,
                    row.FirstName,
                    row.LastName,
                    row.Phone!);
            }

            invitations.Add(invitation);
        }

        await _invitationRepository.AddRangeAsync(invitations, cancellationToken);

        return invitations.Count;
    }

    private async Task<string> GenerateUniqueTokenAsync(HashSet<string> usedTokens, CancellationToken cancellationToken)
    {
        const int maxAttempts = 10;
        for (var i = 0; i < maxAttempts; i++)
        {
            var token = _tokenGenerator.GenerateInvitationToken();
            if (!usedTokens.Contains(token) && !await _invitationRepository.TokenExistsAsync(token, cancellationToken))
            {
                return token;
            }
        }

        throw new InvalidOperationException("Benzersiz token oluşturulamadı. Lütfen tekrar deneyin.");
    }
}
