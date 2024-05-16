using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AspFileUploadPractice.Models;

namespace AspFileUploadPractice.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<AspFileUploadPractice.Models.Speaker> Speaker { get; set; } = default!;
    }
}
