namespace ImageMaker.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemplatesImages : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Compositions", "BackgroundId", "dbo.Images");
            DropForeignKey("dbo.Compositions", "OverlayId", "dbo.Images");
            DropForeignKey("dbo.Compositions", "TemplateId", "dbo.Templates");
            DropIndex("dbo.Compositions", new[] { "TemplateId" });
            DropIndex("dbo.Compositions", new[] { "BackgroundId" });
            DropIndex("dbo.Compositions", new[] { "OverlayId" });
            CreateTable(
                "dbo.Sessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Images", "SessionId", c => c.Int(nullable: false));
            CreateIndex("dbo.Images", "SessionId");
            AddForeignKey("dbo.Images", "SessionId", "dbo.Sessions", "Id", cascadeDelete: true);
            DropTable("dbo.Compositions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Compositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        TemplateId = c.Int(nullable: false),
                        BackgroundId = c.Int(),
                        OverlayId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.Images", "SessionId", "dbo.Sessions");
            DropIndex("dbo.Images", new[] { "SessionId" });
            DropColumn("dbo.Images", "SessionId");
            DropTable("dbo.Sessions");
            CreateIndex("dbo.Compositions", "OverlayId");
            CreateIndex("dbo.Compositions", "BackgroundId");
            CreateIndex("dbo.Compositions", "TemplateId");
            AddForeignKey("dbo.Compositions", "TemplateId", "dbo.Templates", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Compositions", "OverlayId", "dbo.Images", "Id");
            AddForeignKey("dbo.Compositions", "BackgroundId", "dbo.Images", "Id");
        }
    }
}
