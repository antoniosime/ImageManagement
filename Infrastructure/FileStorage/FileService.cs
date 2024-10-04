using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FileStorage
{
    public class FileService : IFileService
    {
        private readonly string _uploadPath = "uploads/";

        public FileService()
        {
            // Ensure the upload directory exists
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, Guid imageId)
        {
            var filePath = Path.Combine(_uploadPath, Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        public async Task<string> CreateImageVariationAsync(string originalFilePath, int height)
        {
            // Create a new variation path
            var variationPath = Path.Combine(_uploadPath, $"{Path.GetFileNameWithoutExtension(originalFilePath)}_{height}px{Path.GetExtension(originalFilePath)}");

            // Simulate image variation by copying the original file
            await Task.Run(() => File.Copy(originalFilePath, variationPath, true));

            return variationPath;
        }

        public async Task<string> SaveLargeFileAsync(Stream fileStream, string fileName)
        {
            var filePath = Path.Combine(_uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await fileStream.CopyToAsync(stream);
            }

            return filePath;
        }

        public Task<string> CopyFileAsync(string source, string destination)
        {
            throw new NotImplementedException();
        }
    }
}
