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

 
        public DbSet<ScannedFileInfo> FilePaths { get; set; }
        public DbSet<DriveInfo> Drives { get; set; }

        public WTFContext()
        {
            base.Database.EnsureCreated();
        }
    }
}