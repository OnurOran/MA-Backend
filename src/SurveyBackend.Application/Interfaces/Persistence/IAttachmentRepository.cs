using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IAttachmentRepository
{
    Task<Attachment?> GetByOwnerAsync(AttachmentOwnerType ownerType, int ownerId, CancellationToken cancellationToken);
    Task<Attachment?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Attachment attachment, CancellationToken cancellationToken);
    Task RemoveAsync(Attachment attachment, CancellationToken cancellationToken);
    Task<AttachmentAccessInfo?> GetAccessInfoAsync(int attachmentId, CancellationToken cancellationToken);
}
