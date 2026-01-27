using SurveyBackend.Application.Interfaces.Export;
using SurveyBackend.Application.Interfaces.External;
using SurveyBackend.Application.Interfaces.Files;
using SurveyBackend.Application.Interfaces.Persistence;
using SurveyBackend.Application.Interfaces.Security;
using SurveyBackend.Application.Surveys.Services;
using SurveyBackend.Infrastructure.Configurations;
using SurveyBackend.Infrastructure.Directory;
using SurveyBackend.Infrastructure.Export;
using SurveyBackend.Infrastructure.Persistence;
using SurveyBackend.Infrastructure.Repositories;
using SurveyBackend.Infrastructure.Repositories.Authorization;
using SurveyBackend.Infrastructure.Repositories.Departments;
using SurveyBackend.Infrastructure.Repositories.Participations;
using SurveyBackend.Infrastructure.Repositories.Attachments;
using SurveyBackend.Infrastructure.Repositories.Surveys;
using SurveyBackend.Infrastructure.Repositories.TextTemplates;
using SurveyBackend.Infrastructure.Seeding;
using SurveyBackend.Infrastructure.Security;
using SurveyBackend.Infrastructure.Storage;

namespace SurveyBackend.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' bulunamadı.");

        services.AddScoped<AuditInterceptor>();

        services.AddDbContext<SurveyBackendDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString);
            options.AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>());
        });

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<LdapSettings>(configuration.GetSection("Ldap"));
        services.Configure<FileStorageOptions>(configuration.GetSection("FileStorage"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<ISurveyRepository, SurveyRepository>();
        services.AddScoped<IAttachmentRepository, AttachmentRepository>();
        services.AddScoped<IParticipantRepository, ParticipantRepository>();
        services.AddScoped<IParticipationRepository, ParticipationRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<ITextTemplateRepository, TextTemplateRepository>();
        services.AddScoped<IAnswerAttachmentRepository, AnswerAttachmentRepository>();
        services.AddScoped<ILdapService, LdapService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<DatabaseSeeder>();
        services.AddScoped<IFileStorage, LocalFileStorage>();
        services.AddScoped<IAttachmentService, AttachmentService>();
        services.AddScoped<AnswerAttachmentService>();
        services.AddScoped<IExcelExportService, ExcelExportService>();

        return services;
    }
}
