namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record AttachmentUploadDto(string FileName, string ContentType, string Base64Content);
