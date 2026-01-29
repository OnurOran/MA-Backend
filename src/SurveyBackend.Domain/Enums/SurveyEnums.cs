namespace SurveyBackend.Domain.Enums;

public enum SurveyStatus
{
    Draft,
    Published
}

public enum AccessType
{
    Internal,
    Public,
    InvitationOnly
}

public enum QuestionType
{
    SingleSelect,
    MultiSelect,
    OpenText,
    FileUpload,
    Conditional,
    Matrix
}
