using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.DataContext.Migrations;
using ImageMaker.Entities;

namespace ImageMaker.DataContext.Contexts
{
    public class ImageDbContextConfiguration : DbConfiguration
    {
        public ImageDbContextConfiguration()
        {
            this.SetDatabaseInitializer(new MigrateDatabaseToLatestVersion<ImageContext, ImageContextConfiguration>("ImageMakerDb"));
            this.SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
        }
    }

    [DbConfigurationType(typeof(ImageDbContextConfiguration))]
    public class ImageContext : DbContext
    {
        public ImageContext() : base("ImageMakerDb")
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public ImageContext(DbConnection connection)
            : base(connection, true)
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Session>()
                .HasKey(x => x.Id)
                .HasMany(x => x.Images)
                .WithRequired(x => x.Session)
                .HasForeignKey(x => x.SessionId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Template>()
                .HasKey(x => x.Id)
                .HasMany(x => x.Images)
                .WithRequired()
                .HasForeignKey(x => x.TemplateId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Template>()
                .HasOptional(x => x.Background)
                .WithMany()
                .HasForeignKey(x => x.BackgroundId);

            modelBuilder.Entity<Template>()
                .HasOptional(x => x.Overlay)
                .WithMany()
                .HasForeignKey(x => x.OverlayId);

            modelBuilder.Entity<TemplateImage>().HasKey(x => x.Id);

            //modelBuilder.Entity<Composition>()
            //    .HasKey(x => x.Id)
            //    .HasRequired(x => x.Template)
            //    .WithMany()
            //    .HasForeignKey(x => x.TemplateId);

            //modelBuilder.Entity<Composition>()
            //    .HasOptional(x => x.Background)
            //    .WithMany()
            //    .HasForeignKey(x => x.BackgroundId);

            //modelBuilder.Entity<Composition>()
            //    .HasOptional(x => x.Overlay)
            //    .WithMany()
            //    .HasForeignKey(x => x.OverlayId);

            modelBuilder.Entity<Image>()
                .HasKey(x => x.Id)
                .HasRequired(x => x.Data)
                .WithRequiredPrincipal()
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<User>()
                .HasKey(x => x.Id);

            base.OnModelCreating(modelBuilder);
        }


        public virtual DbSet<Template> Templates { get; set; }

        public virtual DbSet<TemplateImage> TemplateImages { get; set; }

        //public virtual DbSet<Composition> Compositions { get; set; }

        public virtual DbSet<Image> Images { get; set; }

        public virtual DbSet<FileData> Files { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Session> Sessions { get; set; } 
    }
}
