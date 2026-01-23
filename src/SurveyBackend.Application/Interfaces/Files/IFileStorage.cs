namespace SurveyBackend.Application.Interfaces.Files;

public interface IFileStorage
{
    Task<FileSaveResult> SaveAsync(string fileName, string contentType, Stream content, CancellationToken cancellationToken);
    Task<Stream> OpenReadAsync(string storagePath, CancellationToken cancellationToken);
    Task DeleteAsync(string storagePath, CancellationToken cancellationToken);
}

public sealed record FileSaveResult(string StoragePath, long SizeBytes, string FileName, string ContentType);
