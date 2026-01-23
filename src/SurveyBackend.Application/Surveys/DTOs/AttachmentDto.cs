namespace SurveyBackend.Application.Surveys.DTOs;

public sealed record AttachmentDto(int Id, string FileName, string ContentType, long SizeBytes);
