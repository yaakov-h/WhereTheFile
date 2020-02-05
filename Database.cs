using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WhereTheFile.Types;

namespace WhereTheFile.Database
{
    public class WTFContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                Trace.WriteLine("Using default database file");
                optionsBuilder.UseSqlite("Data Source=WTF_EF.db");
            }
        }

        public WTFContext(DbContextOptions<WTFContext> options) : base(options)
        {

        }


        public DbSet<HashedFile> Files { get; set; }
        public DbSet<FailedFile> FailedFiles { get; set; }

        public WTFContext()
        {
            base.Database.EnsureCreated();
        }
    }
}