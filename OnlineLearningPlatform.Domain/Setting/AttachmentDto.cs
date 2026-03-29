using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Domain.Setting
{
    public class AttachmentDto
    {
        public string FileName { get; set; } 
        public byte[] Content { get; set; }  
        public string ContentType { get; set; }
    }
}
