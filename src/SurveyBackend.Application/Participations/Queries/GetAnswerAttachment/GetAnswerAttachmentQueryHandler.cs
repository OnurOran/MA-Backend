using SurveyBackend.Application.Common;
using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.Queries.GetAttachment;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Participations.Queries.GetAnswerAttachment;

public sealed class GetAnswerAttachmentQueryHandler : ICommandHandler<GetAnswerAttachmentQuery, AttachmentDownloadResult>
{
    private readonly IAnswerAttachmentRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetAnswerAttachmentQueryHandler(IAnswerAttachmentRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<AttachmentDownloadResult> HandleAsync(GetAnswerAttachmentQuery request, CancellationToken cancellationToken)
    {
        var accessInfo = await _repository.GetAccessInfoAsync(request.AttachmentId, cancellationToken)
                         ?? throw new FileNotFoundException("Dosya bulunamadı.");

        EnsureAccess(accessInfo);

        return new AttachmentDownloadResult(accessInfo.FileName, accessInfo.ContentType, accessInfo.StoragePath, accessInfo.SizeBytes);
    }

    private void EnsureAccess(Surveys.DTOs.AnswerAttachmentAccessInfo info)
    {
        if (_currentUserService.IsSuperAdmin || _currentUserService.HasPermission("ManageUsers"))
        {
            return;
        }

        if (_currentUserService.HasPermission("ManageDepartment"))
        {
            return;
        }

        if (info.AccessType == AccessType.Public)
        {
            return;
        }

        if (info.AccessType == AccessType.Internal)
        {

            return;
        }

        throw new UnauthorizedAccessException("Dosyaya erişim için yetkili değilsiniz.");
    }

    private static bool IsSurveyAvailable(Surveys.DTOs.AnswerAttachmentAccessInfo info)
    {
        if (!info.IsActive)
        {
            return false;
        }

        var now = TimeHelper.NowInTurkey;
        if (info.StartDate.HasValue && info.StartDate.Value > now)
        {
            return false;
        }

        if (info.EndDate.HasValue && info.EndDate.Value <= now)
        {
            return false;
        }

        return true;
    }
}
