namespace WebApplication1.Models
{
    public class JobVacancy
    {
        //public string AdminId { get; set; }
        //public Admin Admin { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int VacancyId { get; set; }
        public Vacancy Vacancy { get; set; }
    }
}