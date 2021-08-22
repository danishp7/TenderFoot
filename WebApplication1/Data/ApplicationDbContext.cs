using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        //public DbSet<Admin> Admins { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<JobVacancy> JobVacancies { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        //public DbSet<VacancyAudit> VacancyAudits { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region admin and vacancy many to many relationship
            builder.Entity<JobVacancy>()
                .HasKey(k => new { k.AppUserId, k.VacancyId });

            builder.Entity<JobVacancy>()
                .HasOne(a => a.AppUser)
                .WithMany(v => v.JobVacancies)
                .HasForeignKey(fk => fk.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobVacancy>()
                .HasOne(a => a.Vacancy)
                .WithMany(v => v.JobVacancies)
                .HasForeignKey(fk => fk.VacancyId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion admin and vacancy many to many relationship

            #region appuser and vacancy many to many relationship
            builder.Entity<JobApplication>()
                .HasKey(k => new { k.AppUserId, k.VacancyId });

            builder.Entity<JobApplication>()
                .HasOne(a => a.AppUser)
                .WithMany(ap => ap.JobApplications)
                .HasForeignKey(fk => fk.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobApplication>()
                .HasOne(a => a.Vacancy)
                .WithMany(ap => ap.JobApplications)
                .HasForeignKey(fk => fk.VacancyId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion appuser and vacancy many to many relationship

        }
    }
}
