namespace SurveyBackend.Application.Abstractions.Messaging;

public interface IDepartmentScopedCommand
{
    int DepartmentId { get; }
}
