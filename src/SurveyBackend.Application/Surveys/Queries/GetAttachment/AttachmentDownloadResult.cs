namespace SurveyBackend.Application.Surveys.Queries.GetAttachment;

public sealed record AttachmentDownloadResult(string FileName, string ContentType, string StoragePath, long SizeBytes);
