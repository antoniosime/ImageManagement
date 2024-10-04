using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Extensions
{
    public static class ImageDtoExtension
    {
        public static Image Map(this ImageDto imageDto)
        {
            var image = new Image
            {
                Id = imageDto.Id,
                FileName = imageDto.FileName,
                FilePath = imageDto.FilePath,
                UploadDate = imageDto.UploadDate,
            };

            return image;
        }
    }
}
