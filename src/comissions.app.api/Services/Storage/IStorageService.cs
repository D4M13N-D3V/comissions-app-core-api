namespace comissions.app.api.Services.Storage;

public interface IStorageService
{
    public Task<string> UploadImageAsync(Stream fileStream, string fileName);
    public Task<Stream> DownloadImageAsync(string fileRefrence);
    public string GetMimeType(string fileReference);

}