using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IImageRepository
    {
        Task AddImageAsync(Image image);
        Task AddImageVariantAsync(Guid imageId, long height, string filePath);
        Task<Image> GetImageByIdAsync(Guid imageId);
        Task<ImageVariation> GetImageVariationByIdAsync(Guid imageId, long height);
        Task DeleteImageAsync(Guid imageId);
    }
}
