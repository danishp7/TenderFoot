using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Dtos
{
    public class ApplicationDto
    {
        [Required]
        public IFormFile Resume { get; set; }
        [Required]
        public IFormFile CoverLetter { get; set; }
    }
}
