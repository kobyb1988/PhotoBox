using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.Common.Helpers;
using ImageMaker.DataContext.Contexts;
using ImageMaker.Entities;

namespace ImageMaker.DataContext.Migrations
{
    internal sealed class ImageContextConfiguration : DbMigrationsConfiguration<ImageContext>
    {
        public ImageContextConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ImageContext context)
        {
            var users = context.Set<User>();
            if (users.FirstOrDefault(x => x.Name == "admin") != null)
                return;

            var hashBuilder = new HashBuilder();
            string defaultPassword = hashBuilder.HashPassword("qwerty");
            users.AddOrUpdate(x => x.Name, new User { Name = "admin", Password = defaultPassword});
            //todo
            //context.Set<Pattern>().AddOrUpdate(x => x.PatternType, Enum.GetValues(typeof (PatternType))
            //    .OfType<PatternType>()
            //    .Select(x => new Pattern(x.GetDescription(), x)).ToArray());
        }
    }
}
