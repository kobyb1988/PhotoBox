namespace ImageMaker.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Images : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FileDatas", "Id", "dbo.Images");
            DropIndex("dbo.FileDatas", new[] { "Id" });
            AddColumn("dbo.Images", "Path", c => c.String());
            DropColumn("dbo.Images", "FileDataId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Images", "FileDataId", c => c.Int(nullable: false));
            DropColumn("dbo.Images", "Path");
            CreateIndex("dbo.FileDatas", "Id");
            AddForeignKey("dbo.FileDatas", "Id", "dbo.Images", "Id", cascadeDelete: true);
        }
    }
}
