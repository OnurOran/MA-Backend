using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Surveys.Services;

public interface IAttachmentService
{
    Task<Attachment> SaveSurveyAttachmentAsync(Survey survey, AttachmentUploadDto upload, CancellationToken cancellationToken);
    Task<Attachment> SaveQuestionAttachmentAsync(Survey survey, Question question, AttachmentUploadDto upload, CancellationToken cancellationToken);
    Task<Attachment> SaveOptionAttachmentAsync(Survey survey, QuestionOption option, AttachmentUploadDto upload, CancellationToken cancellationToken);
    Task RemoveAttachmentsAsync(IEnumerable<Attachment> attachments, CancellationToken cancellationToken);
}
