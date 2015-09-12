namespace ImageMaker.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Templates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Templates", "BackgroundId", c => c.Int());
            AddColumn("dbo.Templates", "OverlayId", c => c.Int());
            CreateIndex("dbo.Templates", "BackgroundId");
            CreateIndex("dbo.Templates", "OverlayId");
            AddForeignKey("dbo.Templates", "BackgroundId", "dbo.Images", "Id");
            AddForeignKey("dbo.Templates", "OverlayId", "dbo.Images", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Templates", "OverlayId", "dbo.Images");
            DropForeignKey("dbo.Templates", "BackgroundId", "dbo.Images");
            DropIndex("dbo.Templates", new[] { "OverlayId" });
            DropIndex("dbo.Templates", new[] { "BackgroundId" });
            DropColumn("dbo.Templates", "OverlayId");
            DropColumn("dbo.Templates", "BackgroundId");
        }
    }
}
