namespace WebApplication1.Models
{
    public class JobApplication
    {
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int VacancyId { get; set; }
        public Vacancy Vacancy { get; set; }

        public string Status { get; set; }
        public string Resume { get; set; }
        public string CoverLetter { get; set; }
    }
}