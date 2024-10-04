using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Image
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public List<ImageVariation> Variations { get; set; }
        public DateTime UploadDate { get; set; }
        public string FilePath { get; set; }

        public Image()
        {
            Variations = new List<ImageVariation>();
        }

        public void AddVariation(long height, string variationPath)
        {
            Variations.Add(new ImageVariation
            {
                Id = Guid.NewGuid(),
                Height = height,
                Path = variationPath,
                ImageId = this.Id,
                UploadDate = DateTime.Now
            });
        }
    }
}
