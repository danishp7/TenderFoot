using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class AppUser : IdentityUser
    {
        public byte[] HashPassword { get; set; }
        public byte[] Key { get; set; }
        public ICollection<JobApplication> JobApplications { get; set; }
        public ICollection<JobVacancy> JobVacancies { get; set; }
    }
}
