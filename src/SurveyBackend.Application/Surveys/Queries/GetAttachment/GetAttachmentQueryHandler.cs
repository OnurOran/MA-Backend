using SurveyBackend.Application.Interfaces.Identity;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Application.Surveys.Queries.GetAttachment;

public sealed class GetAttachmentQueryHandler : ICommandHandler<GetAttachmentQuery, AttachmentDownloadResult>
{
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetAttachmentQueryHandler(IAttachmentRepository attachmentRepository, ICurrentUserService currentUserService)
    {
        _attachmentRepository = attachmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<AttachmentDownloadResult> HandleAsync(GetAttachmentQuery request, CancellationToken cancellationToken)
    {
        var accessInfo = await _attachmentRepository.GetAccessInfoAsync(request.AttachmentId, cancellationToken)
                         ?? throw new FileNotFoundException("Dosya bulunamadı.");

        EnsureAccess(accessInfo);

        return new AttachmentDownloadResult(accessInfo.FileName, accessInfo.ContentType, accessInfo.StoragePath, accessInfo.SizeBytes);
    }

    private void EnsureAccess(DTOs.AttachmentAccessInfo info)
    {

        if (_currentUserService.IsSuperAdmin || _currentUserService.HasPermission("ManageUsers"))
        {
            return;
        }

        if (_currentUserService.HasPermission("ManageDepartment") &&
            _currentUserService.DepartmentId.HasValue &&
            _currentUserService.DepartmentId.Value == info.DepartmentId)
        {
            return;
        }

        if (!IsSurveyAvailable(info))
        {
            throw new UnauthorizedAccessException("Anket şu anda erişilebilir değil.");
        }

        if (info.AccessType == AccessType.Public)
        {
            return;
        }

        if (info.AccessType == AccessType.Internal)
        {
            if (_currentUserService.IsAuthenticated)
            {
                return;
            }
            throw new UnauthorizedAccessException("Bu ankete erişim için giriş yapmanız gerekiyor.");
        }

        throw new UnauthorizedAccessException("Dosyaya erişim için yetkili değilsiniz.");
    }

    private static bool IsSurveyAvailable(DTOs.AttachmentAccessInfo info)
    {
        if (!info.IsActive)
        {
            return false;
        }

        var now = DateTime.Now;
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
