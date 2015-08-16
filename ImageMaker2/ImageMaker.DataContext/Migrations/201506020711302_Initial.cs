namespace ImageMaker.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Compositions", name: "Background_Id", newName: "BackgroundId");
            RenameColumn(table: "dbo.Compositions", name: "Overlay_Id", newName: "OverlayId");
            RenameColumn(table: "dbo.Compositions", name: "Template_Id", newName: "TemplateId");
            RenameIndex(table: "dbo.Compositions", name: "IX_Template_Id", newName: "IX_TemplateId");
            RenameIndex(table: "dbo.Compositions", name: "IX_Background_Id", newName: "IX_BackgroundId");
            RenameIndex(table: "dbo.Compositions", name: "IX_Overlay_Id", newName: "IX_OverlayId");
            AddColumn("dbo.Images", "FileDataId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Images", "FileDataId");
            RenameIndex(table: "dbo.Compositions", name: "IX_OverlayId", newName: "IX_Overlay_Id");
            RenameIndex(table: "dbo.Compositions", name: "IX_BackgroundId", newName: "IX_Background_Id");
            RenameIndex(table: "dbo.Compositions", name: "IX_TemplateId", newName: "IX_Template_Id");
            RenameColumn(table: "dbo.Compositions", name: "TemplateId", newName: "Template_Id");
            RenameColumn(table: "dbo.Compositions", name: "OverlayId", newName: "Overlay_Id");
            RenameColumn(table: "dbo.Compositions", name: "BackgroundId", newName: "Background_Id");
        }
    }
}
