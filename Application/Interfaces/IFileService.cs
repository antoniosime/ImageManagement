using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, Guid imageId);
        Task<string> CopyFileAsync(string source, string destination);
        Task<string> CreateImageVariationAsync(string originalFilePath, int height);
    }
}
