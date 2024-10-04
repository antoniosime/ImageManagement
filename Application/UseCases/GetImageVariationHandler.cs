using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class GetImageVariationHandler
    {
        private readonly IImageService _imageService;

        public GetImageVariationHandler(IImageService imageService)
        {
            _imageService = imageService;
        }

        public async Task<string> Handle(Guid imageId, long height)
        {
            return await _imageService.GetImageVariationAsync(imageId, height);
        }
    }
}
