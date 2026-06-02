using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebService.DTO
{
    public class FileRequestDto
    {
        public string clienid { get; set; }
        public string filename { get; set; }
    }

    public class FileResponseDto
    {
        public string clienid { get; set; }
        public string filename { get; set; }
        public string content { get; set; }

    }
}
