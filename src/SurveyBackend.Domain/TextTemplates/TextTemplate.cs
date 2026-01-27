using SurveyBackend.Domain.Common;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Domain.TextTemplates;

public class TextTemplate : CommonEntity
{
    public int Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public TextTemplateType Type { get; private set; }
    public int DepartmentId { get; private set; }

    private TextTemplate() { }

    public static TextTemplate Create(string title, string content, TextTemplateType type, int departmentId)
    {
        return new TextTemplate
        {
            Title = title,
            Content = content,
            Type = type,
            DepartmentId = departmentId
        };
    }

    public void Update(string title, string content, TextTemplateType type)
    {
        Title = title;
        Content = content;
        Type = type;
    }
}
