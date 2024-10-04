using Application.DTOs;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class UploadImageHandler
    {
        private readonly IImageService _imageService;
        private readonly IQueueService _queueService;

        public UploadImageHandler(IImageService imageService, IQueueService queueService)
        {
            _imageService = imageService;
            _queueService = queueService;   
        }

        public async Task<ImageDto> Handle(UploadImageRequest request)
        {
            var image = await _imageService.UploadImageAsync(request.File);

            await ResizePredifinedImages(image);    

            return image;
        }


        private async Task ResizePredifinedImages(ImageDto image)
        {
            // Resize the image for predefined sizes
            // get this from the database or from the config file
            var sizes = new List<int> { 160};

            foreach (var size in sizes)
            {
                var messsage = new ResizeImageDto
                {
                    Id = image.Id,
                    Height = size
                };  
              await _queueService.AddMessageToQueueAsync("resize-image", JsonSerializer.Serialize(messsage));
            }
        }   
    }
}
