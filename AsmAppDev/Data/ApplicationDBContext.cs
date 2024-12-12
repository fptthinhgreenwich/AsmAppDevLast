using AsmAppDev.Models;
using AsmAppDev.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AsmAppDev.Data
{
	public class ApplicationDBContext : IdentityDbContext
	{
		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<Job> Jobs { get; set; }
		public DbSet<Category> Categories { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
		public ApplicationDBContext(DbContextOptions options) : base(options)
		{ }
        
    }
}
