using System.Linq;
using SurveyBackend.Domain.Common;

namespace SurveyBackend.Domain.Surveys;

public class Participation : CommonEntity
{
    public int Id { get; private set; }
    public int SurveyId { get; private set; }
    public int ParticipantId { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? IpAddress { get; private set; }

    public Participant Participant { get; private set; } = null!;
    public ICollection<Answer> Answers { get; private set; } = new List<Answer>();

    private Participation()
    {
    }

    private Participation(int surveyId, int participantId, DateTime startedAt, string? ipAddress)
    {
        SurveyId = surveyId;
        ParticipantId = participantId;
        StartedAt = startedAt;
        IpAddress = ipAddress;
    }

    public static Participation Start(int surveyId, int participantId, DateTime startedAt, string? ipAddress = null)
    {
        return new Participation(surveyId, participantId, startedAt, ipAddress);
    }

    public void Complete(DateTime completedAt)
    {
        CompletedAt = completedAt;
    }

    public Answer AddOrUpdateAnswer(int questionId, string? textValue, IEnumerable<int>? optionIds = null)
    {
        var answer = Answers.FirstOrDefault(a => a.QuestionId == questionId);
        if (answer is null)
        {
            answer = new Answer(Id, questionId, textValue);
            Answers.Add(answer);
        }
        else
        {
            answer.Update(textValue);
        }

        answer.ReplaceSelectedOptions(optionIds);
        return answer;
    }
}
