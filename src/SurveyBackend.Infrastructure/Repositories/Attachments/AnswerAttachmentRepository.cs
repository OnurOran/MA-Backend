using Microsoft.EntityFrameworkCore;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Surveys;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Infrastructure.Repositories.Attachments;

public sealed class AnswerAttachmentRepository : IAnswerAttachmentRepository
{
    private readonly SurveyBackendDbContext _dbContext;

    public AnswerAttachmentRepository(SurveyBackendDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AnswerAttachment?> GetByAnswerIdAsync(int answerId, CancellationToken cancellationToken)
    {
        return await _dbContext.AnswerAttachments.FirstOrDefaultAsync(a => a.AnswerId == answerId, cancellationToken);
    }

    public async Task AddAsync(AnswerAttachment attachment, CancellationToken cancellationToken)
    {
        _dbContext.AnswerAttachments.Add(attachment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(AnswerAttachment attachment, CancellationToken cancellationToken)
    {
        _dbContext.AnswerAttachments.Remove(attachment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<AnswerAttachmentAccessInfo?> GetAccessInfoAsync(int attachmentId, CancellationToken cancellationToken)
    {
        return await (
            from a in _dbContext.AnswerAttachments
            join s in _dbContext.Surveys on a.SurveyId equals s.Id
            join ans in _dbContext.Answers on a.AnswerId equals ans.Id
            join p in _dbContext.Participations on ans.ParticipationId equals p.Id
            where a.Id == attachmentId
            select new AnswerAttachmentAccessInfo(
                a.Id,
                s.Id,
                s.DepartmentId,
                s.AccessType,
                s.IsActive,
                s.StartDate,
                s.EndDate,
                p.Id,
                ans.Id,
                a.FileName,
                a.ContentType,
                a.SizeBytes,
                a.StoragePath))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
