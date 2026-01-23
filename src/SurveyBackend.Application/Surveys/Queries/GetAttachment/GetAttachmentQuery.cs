using SurveyBackend.Application.Surveys.DTOs;

namespace SurveyBackend.Application.Surveys.Queries.GetAttachment;

public sealed record GetAttachmentQuery(int AttachmentId) : ICommand<AttachmentDownloadResult>;
