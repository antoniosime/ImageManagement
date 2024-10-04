using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class GetImageResponse
    {
        public string ImagePath { get; set; }
        public bool IsRunning { get; set; }
        public bool IsCompleted { get; set; }
    }
}

