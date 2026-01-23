using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Surveys;

public class AnswerAttachment : CommonEntity
{
    public int Id { get; private set; }
    public int AnswerId { get; private set; }
    public int SurveyId { get; private set; }
    public int DepartmentId { get; private set; }
    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long SizeBytes { get; private set; }
    public string StoragePath { get; private set; } = null!;

    public Answer Answer { get; private set; } = null!;

    private AnswerAttachment()
    {
    }

    private AnswerAttachment(int answerId, int surveyId, int departmentId, string fileName, string contentType, long sizeBytes, string storagePath)
    {
        AnswerId = answerId;
        SurveyId = surveyId;
        DepartmentId = departmentId;
        FileName = fileName;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        StoragePath = storagePath;
    }

    public static AnswerAttachment Create(int answerId, int surveyId, int departmentId, string fileName, string contentType, long sizeBytes, string storagePath)
    {
        return new AnswerAttachment(answerId, surveyId, departmentId, fileName, contentType, sizeBytes, storagePath);
    }
}
