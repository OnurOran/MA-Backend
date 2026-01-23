using System.Text.RegularExpressions;
using SurveyBackend.Domain.Common;
using SurveyBackend.Domain.Enums;

namespace SurveyBackend.Domain.Surveys;

public class Survey : CommonEntity
{
    private static readonly Regex NonAlphanumericRegex = new(@"[^a-z0-9]+", RegexOptions.Compiled);
    private static readonly Regex MultipleHyphensRegex = new(@"-+", RegexOptions.Compiled);

    public int Id { get; private set; }
    public string Slug { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? IntroText { get; private set; }
    public string? ConsentText { get; private set; }
    public string? OutroText { get; private set; }
    public int DepartmentId { get; private set; }
    public bool IsPublished { get; private set; }
    public AccessType AccessType { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public Attachment? Attachment { get; private set; }
    public ICollection<Question> Questions { get; private set; } = new List<Question>();

    private Survey()
    {
    }

    private Survey(string slug, string title, string? description, string? introText, string? consentText, string? outroText, int departmentId, AccessType accessType, DateTime? startDate, DateTime? endDate)
    {
        Slug = slug;
        Title = title;
        Description = description;
        IntroText = introText;
        ConsentText = consentText;
        OutroText = outroText;
        DepartmentId = departmentId;
        AccessType = accessType;
        StartDate = startDate;
        EndDate = endDate;
        IsPublished = false;
    }

    public static string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return "anket";
        }

        var slug = title
            .Replace('İ', 'I')
            .Replace('Ğ', 'G')
            .Replace('Ü', 'U')
            .Replace('Ş', 'S')
            .Replace('Ö', 'O')
            .Replace('Ç', 'C')
            .ToLowerInvariant()
            .Replace('ı', 'i')
            .Replace('ğ', 'g')
            .Replace('ü', 'u')
            .Replace('ş', 's')
            .Replace('ö', 'o')
            .Replace('ç', 'c');

        slug = NonAlphanumericRegex.Replace(slug, "-");

        slug = MultipleHyphensRegex.Replace(slug, "-").Trim('-');

        if (slug.Length > 100)
        {
            slug = slug[..100].TrimEnd('-');
        }

        return string.IsNullOrWhiteSpace(slug) ? "anket" : slug;
    }

    public static Survey Create(string slug, string title, string? description, string? introText, string? consentText, string? outroText, int departmentId, AccessType accessType, DateTime? startDate = null, DateTime? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Survey title cannot be empty.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("Survey slug cannot be empty.", nameof(slug));
        }

        if (departmentId <= 0)
        {
            throw new ArgumentException("DepartmentId must be greater than 0.", nameof(departmentId));
        }

        if (startDate.HasValue && endDate.HasValue && endDate <= startDate)
        {
            throw new ArgumentException("EndDate must be later than StartDate when both are provided.", nameof(endDate));
        }

        return new Survey(slug.Trim(), title.Trim(), description?.Trim(), introText?.Trim(), consentText?.Trim(), outroText?.Trim(), departmentId, accessType, startDate, endDate);
    }

    public void Publish(DateTime? startDate = null, DateTime? endDate = null)
    {
        var effectiveStart = startDate ?? StartDate;
        var effectiveEnd = endDate ?? EndDate;

        if (effectiveStart.HasValue && effectiveEnd.HasValue && effectiveEnd <= effectiveStart)
        {
            throw new ArgumentException("EndDate must be later than StartDate when both are provided.", nameof(endDate));
        }

        StartDate = effectiveStart;
        EndDate = effectiveEnd;
        IsPublished = true;
    }

    public Question AddQuestion(string text, QuestionType type, int order, bool isRequired, string? description = null)
    {
        var question = Question.Create(Id, text, order, type, isRequired, description);
        Questions.Add(question);
        return question;
    }

    public void Update(string title, string? description, string? introText, string? consentText, string? outroText, AccessType accessType)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Survey title cannot be empty.", nameof(title));
        }

        Title = title.Trim();

        Description = description?.Trim();
        IntroText = introText?.Trim();
        ConsentText = consentText?.Trim();
        OutroText = outroText?.Trim();
        AccessType = accessType;
    }

    public void UpdateSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("Survey slug cannot be empty.", nameof(slug));
        }

        Slug = slug.Trim();
    }
}
