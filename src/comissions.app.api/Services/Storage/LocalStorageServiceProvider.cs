
using System;
using System.IO;
using System.Threading.Tasks;

namespace comissions.app.api.Services.Storage
{
    public class LocalStorageServiceProvider : IStorageService
    {
        private readonly string _storageFolderPath;

        public LocalStorageServiceProvider()
        {
            _storageFolderPath = "/requestbox/";

            // Create storage folder if it does not exist
            if (!Directory.Exists(_storageFolderPath))
            {
                Directory.CreateDirectory(_storageFolderPath);
            }
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
        {
            // Generate a GUID for the file reference
            string fileReference = Guid.NewGuid().ToString();

            // Save the file to the storage folder with the GUID as the file name
            string filePath = Path.Combine(_storageFolderPath, fileReference);
            using (FileStream outputFileStream = File.Create(filePath))
            {
                await fileStream.CopyToAsync(outputFileStream);
            }

            return fileReference;
        }

        public async Task<Stream> DownloadImageAsync(string fileReference)
        {
            // Get the file path based on the provided file reference
            string filePath = Path.Combine(_storageFolderPath, fileReference);

            // Check if the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            // Open the file and return the stream
            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
    }
}
