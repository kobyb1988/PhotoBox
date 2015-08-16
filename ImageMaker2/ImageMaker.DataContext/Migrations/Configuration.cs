using ImageMaker.Common.Extensions;
using ImageMaker.DataContext.Contexts;
using ImageMaker.Entities;

namespace ImageMaker.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PatternContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PatternContext context)
        {
            context.Set<Pattern>().AddOrUpdate(x => x.PatternType, Enum.GetValues(typeof (PatternType))
                .OfType<PatternType>()
                .Select(x => new Pattern(x.GetDescription(), x)).ToArray());
        }
    }
}
