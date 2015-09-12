namespace ImageMaker.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThemeSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "ThemeSettings", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "ThemeSettings");
        }
    }
}
