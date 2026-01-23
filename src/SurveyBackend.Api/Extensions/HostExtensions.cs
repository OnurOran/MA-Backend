using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SurveyBackend.Infrastructure.Persistence;

namespace SurveyBackend.Api.Extensions;

public static class HostExtensions
{

    public static async Task ApplyDatabaseMigrationsAsync(this IHost host, CancellationToken cancellationToken = default)
    {
        await using var scope = host.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SurveyBackendDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
