using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SurveyBackend.Application.Interfaces.Files;
using SurveyBackend.Infrastructure.Configurations;

namespace SurveyBackend.Infrastructure.Storage;

public sealed class LocalFileStorage : IFileStorage
{
    private readonly string _contentRoot;
    private readonly string _rootOption;
    private readonly string _rootFullPath;

    public LocalFileStorage(IOptions<FileStorageOptions> options, IHostEnvironment hostEnvironment)
    {
        _contentRoot = hostEnvironment.ContentRootPath;
        _rootOption = string.IsNullOrWhiteSpace(options.Value.RootPath) ? "wwwroot/uploads" : options.Value.RootPath;
        _rootFullPath = Path.IsPathRooted(_rootOption)
            ? _rootOption
            : Path.GetFullPath(Path.Combine(_contentRoot, _rootOption));
        System.IO.Directory.CreateDirectory(_rootFullPath);
    }

    public async Task<FileSaveResult> SaveAsync(string fileName, string contentType, Stream content, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(fileName);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var relativePath = Path.Combine(_rootOption, storedFileName);
        var fullPath = Path.Combine(_rootFullPath, storedFileName);

        System.IO.Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await content.CopyToAsync(fileStream, cancellationToken);
        }

        var fileInfo = new FileInfo(fullPath);
        return new FileSaveResult(relativePath.Replace('\\', '/'), fileInfo.Length, fileName, contentType);
    }

    public Task<Stream> OpenReadAsync(string storagePath, CancellationToken cancellationToken)
    {
        var fullPath = ResolvePath(storagePath);
        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken)
    {
        var fullPath = ResolvePath(storagePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    private string ResolvePath(string storagePath)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
        {
            throw new FileNotFoundException("Dosya yolu bulunamadı.");
        }

        if (Path.IsPathRooted(storagePath))
        {
            return storagePath;
        }

        return Path.GetFullPath(Path.Combine(_contentRoot, storagePath));
    }
}
