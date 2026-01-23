using SurveyBackend.Domain.Common;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Domain.Surveys;

public class Attachment : CommonEntity
{
    public int Id { get; private set; }
    public AttachmentOwnerType OwnerType { get; private set; }
    public int DepartmentId { get; private set; }
    public int? SurveyId { get; private set; }
    public int? QuestionId { get; private set; }
    public int? QuestionOptionId { get; private set; }
    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long SizeBytes { get; private set; }
    public string StoragePath { get; private set; } = null!;

    public Survey? Survey { get; private set; }
    public Question? Question { get; private set; }
    public QuestionOption? QuestionOption { get; private set; }

    private Attachment()
    {
    }

    private Attachment(
        AttachmentOwnerType ownerType,
        int departmentId,
        int? surveyId,
        int? questionId,
        int? questionOptionId,
        string fileName,
        string contentType,
        long sizeBytes,
        string storagePath)
    {
        OwnerType = ownerType;
        DepartmentId = departmentId;
        SurveyId = surveyId;
        QuestionId = questionId;
        QuestionOptionId = questionOptionId;
        FileName = fileName;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        StoragePath = storagePath;
    }

    public static Attachment CreateForSurvey(
        int surveyId,
        int departmentId,
        string fileName,
        string contentType,
        long sizeBytes,
        string storagePath)
    {
        return new Attachment(AttachmentOwnerType.Survey, departmentId, surveyId, null, null, fileName, contentType, sizeBytes, storagePath);
    }

    public static Attachment CreateForQuestion(
        int questionId,
        int departmentId,
        string fileName,
        string contentType,
        long sizeBytes,
        string storagePath)
    {
        return new Attachment(AttachmentOwnerType.Question, departmentId, null, questionId, null, fileName, contentType, sizeBytes, storagePath);
    }

    public static Attachment CreateForOption(
        int questionOptionId,
        int departmentId,
        string fileName,
        string contentType,
        long sizeBytes,
        string storagePath)
    {
        return new Attachment(AttachmentOwnerType.Option, departmentId, null, null, questionOptionId, fileName, contentType, sizeBytes, storagePath);
    }
}
