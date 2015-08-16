using System.Data.Entity;
using System.Data.Entity.SqlServer;
using ImageMaker.DataContext.Migrations;
using ImageMaker.Entities;

namespace ImageMaker.DataContext.Contexts
{
    class DbContextConfiguration : DbConfiguration
    {
        public DbContextConfiguration()
        {
          //  this.SetDatabaseInitializer(new MigrateDatabaseToLatestVersion<PatternContext, Configuration>("ImageMakerDb"));
            this.SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
        }
    }

    [DbConfigurationType(typeof(DbContextConfiguration))]
    public class PatternContext : DbContext
    {
        public PatternContext() : base("ImageMakerDb")
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<PatternContext, Configuration>("ImageMakerDb"));
            
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pattern>()
                .HasKey(x => x.PatternType)
                .HasMany(x => x.Children)
                .WithRequired()
                .HasForeignKey(x => x.PatternType);

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<PatternData> Files { get; set; } 

        public virtual DbSet<Pattern> Patterns { get; set; } 
    }
}
