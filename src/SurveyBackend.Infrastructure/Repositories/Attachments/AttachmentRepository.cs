using Microsoft.EntityFrameworkCore;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Surveys.DTOs;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Infrastructure.Repositories.Attachments;

public sealed class AttachmentRepository : IAttachmentRepository
{
    private readonly SurveyBackendDbContext _dbContext;

    public AttachmentRepository(SurveyBackendDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Attachment?> GetByOwnerAsync(AttachmentOwnerType ownerType, int ownerId, CancellationToken cancellationToken)
    {
        return ownerType switch
        {
            AttachmentOwnerType.Survey => await _dbContext.Attachments.FirstOrDefaultAsync(a => a.SurveyId == ownerId, cancellationToken),
            AttachmentOwnerType.Question => await _dbContext.Attachments.FirstOrDefaultAsync(a => a.QuestionId == ownerId, cancellationToken),
            AttachmentOwnerType.Option => await _dbContext.Attachments.FirstOrDefaultAsync(a => a.QuestionOptionId == ownerId, cancellationToken),
            _ => null
        };
    }

    public async Task<Attachment?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.Attachments
            .AsNoTracking()
            .Include(a => a.Survey)
            .Include(a => a.Question)
            .Include(a => a.QuestionOption)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task AddAsync(Attachment attachment, CancellationToken cancellationToken)
    {
        _dbContext.Attachments.Add(attachment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Attachment attachment, CancellationToken cancellationToken)
    {
        _dbContext.Attachments.Remove(attachment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<AttachmentAccessInfo?> GetAccessInfoAsync(int attachmentId, CancellationToken cancellationToken)
    {
        var surveyAttachment = await (
            from a in _dbContext.Attachments
            join s in _dbContext.Surveys on a.SurveyId equals s.Id
            where a.Id == attachmentId
            select new AttachmentAccessInfo(
                a.Id,
                s.Id,
                s.DepartmentId,
                s.AccessType,
                s.IsActive,
                s.StartDate,
                s.EndDate,
                a.FileName,
                a.ContentType,
                a.SizeBytes,
                a.StoragePath))
            .FirstOrDefaultAsync(cancellationToken);

        if (surveyAttachment is not null)
        {
            return surveyAttachment;
        }

        var questionAttachment = await (
            from a in _dbContext.Attachments
            join q in _dbContext.Questions on a.QuestionId equals q.Id
            join s in _dbContext.Surveys on q.SurveyId equals s.Id
            where a.Id == attachmentId
            select new AttachmentAccessInfo(
                a.Id,
                s.Id,
                s.DepartmentId,
                s.AccessType,
                s.IsActive,
                s.StartDate,
                s.EndDate,
                a.FileName,
                a.ContentType,
                a.SizeBytes,
                a.StoragePath))
            .FirstOrDefaultAsync(cancellationToken);

        if (questionAttachment is not null)
        {
            return questionAttachment;
        }

        var optionAttachment = await (
            from a in _dbContext.Attachments
            join qo in _dbContext.QuestionOptions on a.QuestionOptionId equals qo.Id
            join q in _dbContext.Questions on qo.QuestionId equals q.Id
            join s in _dbContext.Surveys on q.SurveyId equals s.Id
            where a.Id == attachmentId
            select new AttachmentAccessInfo(
                a.Id,
                s.Id,
                s.DepartmentId,
                s.AccessType,
                s.IsActive,
                s.StartDate,
                s.EndDate,
                a.FileName,
                a.ContentType,
                a.SizeBytes,
                a.StoragePath))
            .FirstOrDefaultAsync(cancellationToken);

        return optionAttachment;
    }
}
