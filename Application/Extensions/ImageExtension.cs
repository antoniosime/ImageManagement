using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Extensions
{
    public static class ImageExtension
    {
        public static ImageDto Map(this Image image)
        {
            return new ImageDto
            {
                Id = image.Id,
                FileName = image.FileName,
                FilePath = image.FilePath,
                UploadDate = image.UploadDate
            };
        }
    }
}
