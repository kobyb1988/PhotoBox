namespace ImageMaker.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignKeys : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Templates", "BackgroundId", "dbo.Images");
            DropForeignKey("dbo.Templates", "OverlayId", "dbo.Images");

            DropIndex("dbo.Templates", new[] { "OverlayId" });
            DropIndex("dbo.Templates", new[] { "BackgroundId" });

            CreateIndex("dbo.Templates", "BackgroundId");
            CreateIndex("dbo.Templates", "OverlayId");

            AddForeignKey("dbo.Templates", "BackgroundId", "dbo.FileDatas", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Templates", "OverlayId", "dbo.FileDatas", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Templates", "BackgroundId", "dbo.FileDatas");
            DropForeignKey("dbo.Templates", "OverlayId", "dbo.FileDatas");


            DropIndex("dbo.Templates", "BackgroundId");
            DropIndex("dbo.Templates", "OverlayId");

            CreateIndex("dbo.Templates", "BackgroundId");
            CreateIndex("dbo.Templates", "OverlayId");

            AddForeignKey("dbo.Templates", "BackgroundId", "dbo.Images", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Templates", "OverlayId", "dbo.Images", "Id", cascadeDelete: true);
            
        }
    }
}
