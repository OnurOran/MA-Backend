using SurveyBackend.Application.Interfaces.Files;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;

namespace SurveyBackend.Application.Surveys.Services;

public sealed class AnswerAttachmentService
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

    private readonly IAnswerAttachmentRepository _repository;
    private readonly IFileStorage _fileStorage;

    public AnswerAttachmentService(IAnswerAttachmentRepository repository, IFileStorage fileStorage)
    {
        _repository = repository;
        _fileStorage = fileStorage;
    }

    public async Task<AnswerAttachment> SaveAsync(
        Survey survey,
        Question question,
        Participation participation,
        Answer answer,
        AttachmentUploadDto upload,
        CancellationToken cancellationToken)
    {
        var bytes = Decode(upload.Base64Content);
        ValidateUpload(upload, bytes.Length, question);

        using var stream = new MemoryStream(bytes);
        var saveResult = await _fileStorage.SaveAsync(upload.FileName, upload.ContentType, stream, cancellationToken);

        var existing = await _repository.GetByAnswerIdAsync(answer.Id, cancellationToken);
        if (existing is not null)
        {
            await TryDeleteFileAsync(existing.StoragePath, cancellationToken);
            await _repository.RemoveAsync(existing, cancellationToken);
        }

        var attachment = AnswerAttachment.Create(
            answer.Id,
            survey.Id,
            survey.DepartmentId,
            saveResult.FileName,
            saveResult.ContentType,
            saveResult.SizeBytes,
            saveResult.StoragePath);

        await _repository.AddAsync(attachment, cancellationToken);
        return attachment;
    }

    private static void ValidateUpload(AttachmentUploadDto upload, long sizeBytes, Question question)
    {
        if (sizeBytes <= 0)
        {
            throw new InvalidOperationException("Dosya içeriği boş.");
        }

        if (sizeBytes > MaxAttachmentSizeBytes)
        {
            throw new InvalidOperationException("Dosya boyutu 5MB sınırını aşıyor.");
        }

        if (string.IsNullOrWhiteSpace(upload.ContentType))
        {
            throw new InvalidOperationException("Dosya türü belirtilmelidir.");
        }

        var allowedFromQuestion = question.GetAllowedAttachmentContentTypes();
        if (allowedFromQuestion.Any())
        {
            if (!allowedFromQuestion.Contains(upload.ContentType, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Bu soru için yüklenebilecek dosya türü geçerli değil.");
            }
        }
        else if (!AllowedContentTypes.Contains(upload.ContentType))
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
