using Microsoft.EntityFrameworkCore;

namespace static_sv.Models
{
    public class StaticContext : DbContext
    {
        public StaticContext(DbContextOptions<StaticContext> options) : base(options)
        {
        }

        public DbSet<Folder> Folders { get; set; }= null!;
        public DbSet<Staticfile> Staticfiles { get; set; }= null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Folder>(f => {
                f.ToTable("folders");
            });

            builder.Entity<Staticfile>(f => {
                f.ToTable("staticfiles");
            });
        }
    }
}