using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Interfaces.Persistence;

public interface IAnswerAttachmentRepository
{
    Task<AnswerAttachment?> GetByAnswerIdAsync(int answerId, CancellationToken cancellationToken);
    Task AddAsync(AnswerAttachment attachment, CancellationToken cancellationToken);
    Task RemoveAsync(AnswerAttachment attachment, CancellationToken cancellationToken);
    Task<AnswerAttachmentAccessInfo?> GetAccessInfoAsync(int attachmentId, CancellationToken cancellationToken);
}
