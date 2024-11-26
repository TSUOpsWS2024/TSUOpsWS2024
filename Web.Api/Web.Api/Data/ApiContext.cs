using Microsoft.EntityFrameworkCore;
using Web.Api.Models;

namespace Web.Api.Data
{
    public class ApiContext : DbContext
    {
        public DbSet<GeneratedImage> GeneratedImages { get; set; }
        public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }
    }
}
