namespace SurveyBackend.Domain.Common;

public abstract class CommonEntity
{

    public bool IsActive { get; protected set; } = true;

    public bool IsDelete { get; protected set; }

    public DateTime CreateDate { get; protected set; }

    public int? CreateEmployeeId { get; protected set; }

    public DateTime? UpdateDate { get; protected set; }

    public int? UpdateEmployeeId { get; protected set; }

    public byte[]? RowVersion { get; protected set; }

    public void Delete()
    {
        IsDelete = true;
    }

    public void Restore()
    {
        IsDelete = false;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void SetCreatedAudit(int? employeeId, DateTime timestamp)
    {
        CreateEmployeeId = employeeId;
        CreateDate = timestamp;
    }

    public void SetUpdatedAudit(int? employeeId, DateTime timestamp)
    {
        UpdateEmployeeId = employeeId;
        UpdateDate = timestamp;
    }
}
