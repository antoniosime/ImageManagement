using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Persistence
{
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddImageAsync(Image image)
        {
            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task<Image> GetImageByIdAsync(Guid imageId)
        {
            return await _context.Images.FirstOrDefaultAsync(img => img.Id == imageId);
        }

        public async Task DeleteImageAsync(Guid imageId)
        {
            var image = await _context.Images.FirstOrDefaultAsync(img => img.Id == imageId);
            if (image != null)
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddImageVariantAsync(Guid imageId, long height, string filePath)
        {
            var image =  _context.Images.Include(x=>x.Variations).FirstOrDefault(img => img.Id == imageId);
            if (image != null)
            {
                _context.ImageVariations.Add(new ImageVariation
                {
                    Height = height,
                    Id = Guid.NewGuid(),
                    ImageId = imageId,
                    Path = filePath,
                    UploadDate = DateTime.UtcNow    
                });
                await _context.SaveChangesAsync();
            } 
            
        }

        public async Task<ImageVariation> GetImageVariationByIdAsync(Guid imageId, long height)
        {
            return await _context.ImageVariations.FirstOrDefaultAsync(x=>x.Height==height && x.ImageId==imageId);  
        }
    }
}
