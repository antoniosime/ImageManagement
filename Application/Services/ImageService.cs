using Application.DTOs;
using Application.Extensions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ImageService : IImageService
    {
        private readonly IFileService _fileService;
        private readonly IImageRepository _imageRepository;

        public ImageService(IFileService fileService, IImageRepository imageRepository)
        {
            _fileService = fileService;
            _imageRepository = imageRepository;
        }

        public async Task<ImageDto> UploadImageAsync(IFormFile file)
        {
            var imageId =  Guid.NewGuid(); 

            // Save the file to the file system
            var filePath = await _fileService.SaveFileAsync(file, imageId);

            // Create and save the image entity in the database
            var image = new Image
            {
                Id = imageId,
                FileName = Path.GetFileName(filePath),
                FilePath = filePath,
                UploadDate = DateTime.UtcNow,
            };
            await _imageRepository.AddImageAsync(image);

            // Return the DTO representing the uploaded image
            return image.Map();
        }

        public async Task<ImageDto> GetImageAsync(Guid imageId)
        {
            var image = await _imageRepository.GetImageByIdAsync(imageId);
            if (image == null)
            {
                throw new FileNotFoundException($"Image with ID {imageId} not found.");
            }

            // Return the DTO representing the uploaded image
            return image.Map();
        }

        public async Task<string> GetImageVariationAsync(Guid imageId, long height)
        {
            var image = await _imageRepository.GetImageVariationByIdAsync(imageId, height);
            if (image == null)
            {
                return "";
            }

            return image.Path;
        }

        public async Task<string> ResizeImage(Guid imageId, long height)
        {
            var image = await _imageRepository.GetImageByIdAsync(imageId);
            if (image == null)
            {
                throw new FileNotFoundException($"Image with ID {imageId} not found.");
            }

            //await Task.Delay(20000); // Simulate image resizing

            var filePath = await _fileService.CopyFileAsync(image.FilePath, $"{image.Id}/{height}.png");

            await _imageRepository.AddImageVariantAsync(imageId, height, filePath);

            return filePath; 
        }
    }
}
