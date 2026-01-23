using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SurveyBackend.Domain.Common;
using SurveyBackend.Domain.Departments;
using SurveyBackend.Domain.Enums;
using SurveyBackend.Domain.Parameters;
using SurveyBackend.Domain.Roles;
using SurveyBackend.Domain.Surveys;
using SurveyBackend.Domain.Users;
using SurveyBackend.Infrastructure.Persistence.Seeds;

namespace SurveyBackend.Infrastructure.Persistence;

public class SurveyBackendDbContext : DbContext
{
    public SurveyBackendDbContext(DbContextOptions<SurveyBackendDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Survey> Surveys => Set<Survey>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<DependentQuestion> DependentQuestions => Set<DependentQuestion>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Participation> Participations => Set<Participation>();
    public DbSet<Answer> Answers => Set<Answer>();
    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<AnswerAttachment> AnswerAttachments => Set<AnswerAttachment>();
    public DbSet<Parameter> Parameters => Set<Parameter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(CommonEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(CommonEntity.IsDelete));
                var filterExpression = Expression.Lambda(Expression.Not(property), parameter);

                entityType.SetQueryFilter(filterExpression);

                var rowVersionProperty = entityType.FindProperty(nameof(CommonEntity.RowVersion));
                if (rowVersionProperty != null)
                {
                    rowVersionProperty.IsConcurrencyToken = true;
                    rowVersionProperty.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAddOrUpdate;
                }
            }
        }

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Id)
                .ValueGeneratedOnAdd();
            entity.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(d => d.ExternalIdentifier)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasIndex(d => d.ExternalIdentifier)
                .IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id)
                .ValueGeneratedOnAdd();
            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(254);
            entity.Property(u => u.DepartmentId)
                .IsRequired();
            entity.Property(u => u.PasswordHash)
                .HasMaxLength(512);
            entity.Property(u => u.IsSuperAdmin)
                .IsRequired();
            entity.HasOne<Department>()
                .WithMany()
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId);
            entity.HasMany(u => u.Roles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("UserRefreshToken");
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.Id)
                .ValueGeneratedOnAdd();
            entity.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(512);
            entity.Property(rt => rt.ExpiresAt)
                .IsRequired();
            entity.Property(rt => rt.UserId)
                .IsRequired();
            entity.HasIndex(rt => rt.Token)
                .IsUnique();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id)
                .ValueGeneratedOnAdd();
            entity.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(r => r.Description)
                .HasMaxLength(200);
            entity.HasMany(r => r.Permissions)
                .WithOne(rp => rp.Role)
                .HasForeignKey(rp => rp.RoleId);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permission");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(p => p.Description)
                .HasMaxLength(200);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermission");
            entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });
            entity.HasOne(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRole");
            entity.HasKey(ur => new { ur.UserId, ur.RoleId, ur.DepartmentId });
            entity.Property(ur => ur.DepartmentId)
                .IsRequired();
            entity.HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId);
            entity.HasOne(ur => ur.Department)
                .WithMany()
                .HasForeignKey(ur => ur.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Survey>(entity =>
        {
            entity.ToTable("Survey");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            entity.Property(s => s.Slug)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(s => s.Slug)
                .IsUnique();

            entity.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(s => s.Description)
                .HasMaxLength(2000);
            entity.Property(s => s.IntroText)
                .HasMaxLength(256);
            entity.Property(s => s.ConsentText)
                .HasMaxLength(1000);
            entity.Property(s => s.OutroText)
                .HasMaxLength(150);
            entity.Property(s => s.DepartmentId)
                .IsRequired();
            entity.Property(s => s.AccessType)
                .IsRequired();
            entity.Property(s => s.IsPublished)
                .IsRequired()
                .HasDefaultValue(false);
            entity.Property(s => s.StartDate);
            entity.Property(s => s.EndDate);

            entity.Property<int?>("TypeId").IsRequired(false);
            entity.HasOne<Parameter>()
                .WithMany()
                .HasForeignKey("TypeId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasOne<Department>()
                .WithMany()
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(s => s.Questions)
                .WithOne(q => q.Survey)
                .HasForeignKey(q => q.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("Question");
            entity.HasKey(q => q.Id);
            entity.Property(q => q.Id)
                .ValueGeneratedOnAdd();
            entity.Property(q => q.Text)
                .IsRequired()
                .HasMaxLength(1000);
            entity.Property(q => q.Description)
                .HasMaxLength(500);
            entity.Property(q => q.Order)
                .IsRequired();
            entity.Property(q => q.Type)
                .IsRequired();
            entity.Property(q => q.IsRequired)
                .IsRequired();
            entity.Property(q => q.AllowedAttachmentContentTypes)
                .HasMaxLength(512);

            // Matrix question type properties
            entity.Property(q => q.MatrixScale1Label)
                .HasMaxLength(100);
            entity.Property(q => q.MatrixScale2Label)
                .HasMaxLength(100);
            entity.Property(q => q.MatrixScale3Label)
                .HasMaxLength(100);
            entity.Property(q => q.MatrixScale4Label)
                .HasMaxLength(100);
            entity.Property(q => q.MatrixScale5Label)
                .HasMaxLength(100);
            entity.Property(q => q.MatrixShowExplanation)
                .HasDefaultValue(false);
            entity.Property(q => q.MatrixExplanationLabel)
                .HasMaxLength(200);

            entity.Property<int?>("TypeId").IsRequired(false);
            entity.HasOne<Parameter>()
                .WithMany()
                .HasForeignKey("TypeId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasMany(q => q.Options)
                .WithOne(o => o.Question)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany<Answer>()
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.ToTable("QuestionOption");
            entity.HasKey(qo => qo.Id);
            entity.Property(qo => qo.Id)
                .ValueGeneratedOnAdd();
            entity.Property(qo => qo.Text)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(qo => qo.Order)
                .IsRequired();
            entity.Property(qo => qo.Value);

            entity.HasMany(qo => qo.DependentQuestions)
                .WithOne(dq => dq.ParentOption)
                .HasForeignKey(dq => dq.ParentQuestionOptionId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(qo => qo.Attachment)
                .WithOne(a => a.QuestionOption)
                .HasForeignKey<Attachment>(a => a.QuestionOptionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DependentQuestion>(entity =>
        {
            entity.ToTable("DependentQuestion");
            entity.HasKey(dq => dq.Id);
            entity.Property(dq => dq.Id)
                .ValueGeneratedOnAdd();
            entity.Property(dq => dq.ParentQuestionOptionId)
                .IsRequired();
            entity.Property(dq => dq.ChildQuestionId)
                .IsRequired();

            entity.HasOne(dq => dq.ChildQuestion)
                .WithMany()
                .HasForeignKey(dq => dq.ChildQuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Participant>(entity =>
        {
            entity.ToTable("Participant");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();
            entity.Property(p => p.ExternalId);
            entity.Property(p => p.LdapUsername)
                .HasMaxLength(100);

            entity.HasIndex(p => p.ExternalId)
                .IsUnique()
                .HasFilter("[ExternalId] IS NOT NULL");

            entity.HasIndex(p => p.LdapUsername)
                .IsUnique()
                .HasFilter("[LdapUsername] IS NOT NULL");
        });

        modelBuilder.Entity<Participation>(entity =>
        {
            entity.ToTable("Participation");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();
            entity.Property(p => p.SurveyId)
                .IsRequired();
            entity.Property(p => p.ParticipantId)
                .IsRequired();
            entity.Property(p => p.StartedAt)
                .IsRequired();
            entity.Property(p => p.CompletedAt);
            entity.Property(p => p.IpAddress)
                .HasMaxLength(50);

            entity.HasOne(p => p.Participant)
                .WithMany()
                .HasForeignKey(p => p.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Survey>()
                .WithMany()
                .HasForeignKey(p => p.SurveyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.Answers)
                .WithOne(a => a.Participation)
                .HasForeignKey(a => a.ParticipationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.ToTable("Answer");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id)
                .ValueGeneratedOnAdd();
            entity.Property(a => a.ParticipationId)
                .IsRequired();
            entity.Property(a => a.QuestionId)
                .IsRequired();
            entity.Property(a => a.TextValue)
                .HasMaxLength(2000);

            entity.HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(a => a.SelectedOptions)
                .WithOne(ao => ao.Answer)
                .HasForeignKey(ao => ao.AnswerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AnswerOption>(entity =>
        {
            entity.ToTable("AnswerOption");
            entity.HasKey(ao => ao.Id);
            entity.Property(ao => ao.Id)
                .ValueGeneratedOnAdd();
            entity.Property(ao => ao.AnswerId)
                .IsRequired();
            entity.Property(ao => ao.QuestionOptionId)
                .IsRequired();

            // Matrix question type properties
            entity.Property(ao => ao.ScaleValue);
            entity.Property(ao => ao.Explanation)
                .HasMaxLength(500);

            entity.HasOne(ao => ao.QuestionOption)
                .WithMany()
                .HasForeignKey(ao => ao.QuestionOptionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AnswerAttachment>(entity =>
        {
            entity.ToTable("AnswerAttachment");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id)
                .ValueGeneratedOnAdd();
            entity.Property(a => a.FileName)
                .IsRequired()
                .HasMaxLength(128);
            entity.Property(a => a.ContentType)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(a => a.StoragePath)
                .IsRequired()
                .HasMaxLength(1024);
            entity.Property(a => a.SizeBytes)
                .IsRequired();
            entity.Property(a => a.SurveyId)
                .IsRequired();
            entity.Property(a => a.DepartmentId)
                .IsRequired();

            entity.HasIndex(a => a.AnswerId)
                .IsUnique();

            entity.HasOne(a => a.Answer)
                .WithOne(ans => ans.Attachment)
                .HasForeignKey<AnswerAttachment>(a => a.AnswerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.ToTable("Attachment", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_Attachment_SingleOwner", @"
                (
                    (CASE WHEN SurveyId IS NOT NULL THEN 1 ELSE 0 END) +
                    (CASE WHEN QuestionId IS NOT NULL THEN 1 ELSE 0 END) +
                    (CASE WHEN QuestionOptionId IS NOT NULL THEN 1 ELSE 0 END)
                ) = 1");
            });

            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id)
                .ValueGeneratedOnAdd();
            entity.Property(a => a.OwnerType)
                .IsRequired();
            entity.Property(a => a.DepartmentId)
                .IsRequired();
            entity.Property(a => a.FileName)
                .IsRequired()
                .HasMaxLength(128);
            entity.Property(a => a.ContentType)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(a => a.StoragePath)
                .IsRequired()
                .HasMaxLength(1024);
            entity.Property(a => a.SizeBytes)
                .IsRequired();

            entity.HasIndex(a => a.SurveyId)
                .IsUnique()
                .HasFilter("[SurveyId] IS NOT NULL");
            entity.HasIndex(a => a.QuestionId)
                .IsUnique()
                .HasFilter("[QuestionId] IS NOT NULL");
            entity.HasIndex(a => a.QuestionOptionId)
                .IsUnique()
                .HasFilter("[QuestionOptionId] IS NOT NULL");

            entity.HasOne(a => a.Survey)
                .WithOne(s => s.Attachment)
                .HasForeignKey<Attachment>(a => a.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.Question)
                .WithOne(q => q.Attachment)
                .HasForeignKey<Attachment>(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.QuestionOption)
                .WithOne(qo => qo.Attachment)
                .HasForeignKey<Attachment>(a => a.QuestionOptionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Parameter>(entity =>
        {
            entity.ToTable("Parameter");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id)
                .ValueGeneratedNever();

            entity.Property(p => p.Code)
                .HasMaxLength(20);

            entity.Property(p => p.GroupName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.DisplayName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.Description)
                .HasMaxLength(500);

            entity.Property(p => p.ParentId)
                .IsRequired();

            entity.Property(p => p.LevelNo)
                .IsRequired();

            entity.Property(p => p.Symbol)
                .HasMaxLength(10);

            entity.Property(p => p.OrderNo)
                .IsRequired();

        });

        modelBuilder.SeedParameters();
    }
}
