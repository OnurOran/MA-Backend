using SurveyBackend.Application.Interfaces.Files;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Surveys.Services;

public sealed class AttachmentService : IAttachmentService
{
    private static readonly HashSet<string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "image/png",
            "image/jpeg",
            "image/jpg",
            "image/webp",
            "application/pdf",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            "application/vnd.ms-powerpoint"
        };

    private const long MaxAttachmentSizeBytes = 5 * 1024 * 1024;

    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IFileStorage _fileStorage;

    public AttachmentService(IAttachmentRepository attachmentRepository, IFileStorage fileStorage)
    {
        _attachmentRepository = attachmentRepository;
        _fileStorage = fileStorage;
    }

    public async Task<Attachment> SaveSurveyAttachmentAsync(Survey survey, AttachmentUploadDto upload, CancellationToken cancellationToken)
    {
        return await SaveAsync(
            AttachmentOwnerType.Survey,
            survey.DepartmentId,
            upload,
            surveyId: survey.Id,
            questionId: null,
            optionId: null,
            cancellationToken);
    }

    public async Task<Attachment> SaveQuestionAttachmentAsync(Survey survey, Question question, AttachmentUploadDto upload, CancellationToken cancellationToken)
    {
        return await SaveAsync(
            AttachmentOwnerType.Question,
            survey.DepartmentId,
            upload,
            surveyId: null,
            questionId: question.Id,
            optionId: null,
            cancellationToken);
    }

    public async Task<Attachment> SaveOptionAttachmentAsync(Survey survey, QuestionOption option, AttachmentUploadDto upload, CancellationToken cancellationToken)
    {
        return await SaveAsync(
            AttachmentOwnerType.Option,
            survey.DepartmentId,
            upload,
            surveyId: null,
            questionId: null,
            optionId: option.Id,
            cancellationToken);
    }

    public async Task RemoveAttachmentsAsync(IEnumerable<Attachment> attachments, CancellationToken cancellationToken)
    {
        foreach (var attachment in attachments)
        {
            await TryDeleteFileAsync(attachment.StoragePath, cancellationToken);
            await _attachmentRepository.RemoveAsync(attachment, cancellationToken);
        }
    }

    private async Task<Attachment> SaveAsync(
        AttachmentOwnerType ownerType,
        int departmentId,
        AttachmentUploadDto upload,
        int? surveyId,
        int? questionId,
        int? optionId,
        CancellationToken cancellationToken)
    {
        var bytes = Decode(upload.Base64Content);
        ValidateUpload(upload, bytes.Length);

        using var stream = new MemoryStream(bytes);
        var saveResult = await _fileStorage.SaveAsync(upload.FileName, upload.ContentType, stream, cancellationToken);

        var existing = await _attachmentRepository.GetByOwnerAsync(ownerType, surveyId ?? questionId ?? optionId ?? 0, cancellationToken);
        if (existing is not null)
        {
            await TryDeleteFileAsync(existing.StoragePath, cancellationToken);
            await _attachmentRepository.RemoveAsync(existing, cancellationToken);
        }

        var attachment = CreateAttachment(ownerType, departmentId, surveyId, questionId, optionId, saveResult);
        await _attachmentRepository.AddAsync(attachment, cancellationToken);
        return attachment;
    }

    private static Attachment CreateAttachment(
        AttachmentOwnerType ownerType,
        int departmentId,
        int? surveyId,
        int? questionId,
        int? optionId,
        FileSaveResult saveResult)
    {
        return ownerType switch
        {
            AttachmentOwnerType.Survey => Attachment.CreateForSurvey(surveyId ?? throw new ArgumentNullException(nameof(surveyId)), departmentId, saveResult.FileName, saveResult.ContentType, saveResult.SizeBytes, saveResult.StoragePath),
            AttachmentOwnerType.Question => Attachment.CreateForQuestion(questionId ?? throw new ArgumentNullException(nameof(questionId)), departmentId, saveResult.FileName, saveResult.ContentType, saveResult.SizeBytes, saveResult.StoragePath),
            AttachmentOwnerType.Option => Attachment.CreateForOption(optionId ?? throw new ArgumentNullException(nameof(optionId)), departmentId, saveResult.FileName, saveResult.ContentType, saveResult.SizeBytes, saveResult.StoragePath),
            _ => throw new ArgumentOutOfRangeException(nameof(ownerType), ownerType, "Unsupported attachment owner.")
        };
    }

    private static void ValidateUpload(AttachmentUploadDto upload, long sizeBytes)
    {
        if (sizeBytes <= 0)
        {
            throw new InvalidOperationException("Dosya içeriği boş.");
        }

        if (sizeBytes > MaxAttachmentSizeBytes)
        {
            throw new InvalidOperationException($"Dosya boyutu 5MB sınırını aşıyor. Boyut: {sizeBytes} bayt.");
        }

        if (string.IsNullOrWhiteSpace(upload.ContentType) || !AllowedContentTypes.Contains(upload.ContentType))
        {
            throw new InvalidOperationException("Desteklenmeyen dosya türü.");
        }

        if (string.IsNullOrWhiteSpace(upload.FileName))
        {
            throw new InvalidOperationException("Dosya adı boş olamaz.");
        }
    }

    private static byte[] Decode(string base64Content)
    {
        try
        {
            return Convert.FromBase64String(base64Content);
        }
        catch (FormatException)
        {
            throw new InvalidOperationException("Dosya içeriği geçerli bir Base64 formatında değil.");
        }
    }

    private async Task TryDeleteFileAsync(string storagePath, CancellationToken cancellationToken)
    {
        try
        {
            await _fileStorage.DeleteAsync(storagePath, cancellationToken);
        }
        catch
        {

        }
    }
}
