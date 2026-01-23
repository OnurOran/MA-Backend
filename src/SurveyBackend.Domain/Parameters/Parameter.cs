using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Parameters;

public sealed class Parameter : CommonEntity
{
    public int Id { get; private set; }

    public string? Code { get; private set; }

    public string GroupName { get; private set; } = null!;

    public string DisplayName { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public int ParentId { get; private set; }

    public int LevelNo { get; private set; }

    public string? Symbol { get; private set; }

    public int OrderNo { get; private set; }

    private Parameter() { }

    public static Parameter Create(
        int id,
        string? code,
        string groupName,
        string displayName,
        string name,
        string? description,
        int parentId,
        int levelNo,
        string? symbol,
        int orderNo)
    {
        return new Parameter
        {
            Id = id,
            Code = code,
            GroupName = groupName,
            DisplayName = displayName,
            Name = name,
            Description = description,
            ParentId = parentId,
            LevelNo = levelNo,
            Symbol = symbol,
            OrderNo = orderNo
        };
    }
}
