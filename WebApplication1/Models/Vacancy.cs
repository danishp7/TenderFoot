using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Vacancy
    {
        public int Id { get; set; }
        public string Type { get; set; } // this will be enum
        public string Title { get; set; }
        public string Responsibility { get; set; }
        public string Salary { get; set; }
        public string MinimumRequirement { get; set; }
        public string Description { get; set; }
        public ICollection<JobVacancy> JobVacancies { get; set; }
        public ICollection<JobApplication> JobApplications { get; set; }
    }
}
