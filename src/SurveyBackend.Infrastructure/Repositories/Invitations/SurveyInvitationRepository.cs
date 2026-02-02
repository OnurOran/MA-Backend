using Microsoft.EntityFrameworkCore;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Surveys;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Infrastructure.Repositories.Invitations;

public sealed class SurveyInvitationRepository : ISurveyInvitationRepository
{
    private readonly SurveyBackendDbContext _context;

    public SurveyInvitationRepository(SurveyBackendDbContext context)
    {
        _context = context;
    }

    public async Task<SurveyInvitation?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.SurveyInvitations
            .Include(i => i.Survey)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<SurveyInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken)
    {
        return await _context.SurveyInvitations
            .Include(i => i.Survey)
                .ThenInclude(s => s.Questions.OrderBy(q => q.Order))
                    .ThenInclude(q => q.Options.OrderBy(o => o.Order))
            .Include(i => i.Survey)
                .ThenInclude(s => s.Attachment)
            .FirstOrDefaultAsync(i => i.Token == token, cancellationToken);
    }

    public async Task<IReadOnlyList<SurveyInvitation>> GetBySurveyIdAsync(int surveyId, CancellationToken cancellationToken)
    {
        return await _context.SurveyInvitations
            .Where(i => i.SurveyId == surveyId)
            .OrderByDescending(i => i.CreateDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SurveyInvitation>> GetPendingBySurveyIdAsync(int surveyId, CancellationToken cancellationToken)
    {
        return await _context.SurveyInvitations
            .Where(i => i.SurveyId == surveyId && i.Status == InvitationStatus.Pending)
            .OrderBy(i => i.CreateDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SurveyInvitation>> GetSentBySurveyIdAsync(int surveyId, CancellationToken cancellationToken)
    {
        return await _context.SurveyInvitations
            .Where(i => i.SurveyId == surveyId && i.Status == InvitationStatus.Sent)
            .OrderBy(i => i.CreateDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> TokenExistsAsync(string token, CancellationToken cancellationToken)
    {
        return await _context.SurveyInvitations
            .AnyAsync(i => i.Token == token, cancellationToken);
    }

    public async Task<bool> EmailExistsForSurveyAsync(int surveyId, string email, CancellationToken cancellationToken)
    {
        return await _context.SurveyInvitations
            .AnyAsync(i => i.SurveyId == surveyId && i.Email == email && i.Status != InvitationStatus.Cancelled, cancellationToken);
    }

    public async Task<bool> PhoneExistsForSurveyAsync(int surveyId, string phone, CancellationToken cancellationToken)
    {
        return await _context.SurveyInvitations
            .AnyAsync(i => i.SurveyId == surveyId && i.Phone == phone && i.Status != InvitationStatus.Cancelled, cancellationToken);
    }

    public async Task<SurveyInvitation?> GetByParticipationIdAsync(int participationId, CancellationToken cancellationToken)
    {
        return await _context.SurveyInvitations
            .FirstOrDefaultAsync(i => i.ParticipationId == participationId, cancellationToken);
    }

    public async Task AddAsync(SurveyInvitation invitation, CancellationToken cancellationToken)
    {
        await _context.SurveyInvitations.AddAsync(invitation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<SurveyInvitation> invitations, CancellationToken cancellationToken)
    {
        await _context.SurveyInvitations.AddRangeAsync(invitations, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(SurveyInvitation invitation, CancellationToken cancellationToken)
    {
        _context.SurveyInvitations.Update(invitation);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
