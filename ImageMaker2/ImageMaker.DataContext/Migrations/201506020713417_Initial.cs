namespace ImageMaker.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Images", t => t.BackgroundId)
                .ForeignKey("dbo.Images", t => t.OverlayId)
                .ForeignKey("dbo.Templates", t => t.TemplateId, cascadeDelete: true)
                .Index(t => t.TemplateId)
                .Index(t => t.BackgroundId)
                .Index(t => t.OverlayId);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        FileDataId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FileDatas",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Data = c.Binary(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Images", t => t.Id, cascadeDelete: true)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Templates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TemplateImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        X = c.Int(nullable: false),
                        Y = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        TemplateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Templates", t => t.TemplateId, cascadeDelete: true)
                .Index(t => t.TemplateId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Compositions", "TemplateId", "dbo.Templates");
            DropForeignKey("dbo.TemplateImages", "TemplateId", "dbo.Templates");
            DropForeignKey("dbo.Compositions", "OverlayId", "dbo.Images");
            DropForeignKey("dbo.Compositions", "BackgroundId", "dbo.Images");
            DropForeignKey("dbo.FileDatas", "Id", "dbo.Images");
            DropIndex("dbo.TemplateImages", new[] { "TemplateId" });
            DropIndex("dbo.FileDatas", new[] { "Id" });
            DropIndex("dbo.Compositions", new[] { "OverlayId" });
            DropIndex("dbo.Compositions", new[] { "BackgroundId" });
            DropIndex("dbo.Compositions", new[] { "TemplateId" });
            DropTable("dbo.TemplateImages");
            DropTable("dbo.Templates");
            DropTable("dbo.FileDatas");
            DropTable("dbo.Images");
            DropTable("dbo.Compositions");
        }
    }
}
