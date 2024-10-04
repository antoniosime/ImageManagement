using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ImageVariation
    {
        public Guid Id { get; set; }
        public long Height { get; set; }
        public string Path { get; set; }
        public Guid ImageId { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
