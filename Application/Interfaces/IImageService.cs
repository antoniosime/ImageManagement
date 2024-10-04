using Application.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IImageService
    {
        Task<ImageDto> UploadImageAsync(IFormFile file);

        Task<ImageDto> GetImageAsync(Guid imageId);

        Task<string> GetImageVariationAsync(Guid imageId, long height);
        Task<string> ResizeImage(Guid imageId, long height);

    }
}
