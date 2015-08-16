namespace ImageMaker.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Images_Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Compositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Background_Id = c.Int(),
                        Overlay_Id = c.Int(),
                        Template_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Images", t => t.Background_Id)
                .ForeignKey("dbo.Images", t => t.Overlay_Id)
                .ForeignKey("dbo.TemplateImages", t => t.Template_Id, cascadeDelete: true)
                .Index(t => t.Background_Id)
                .Index(t => t.Overlay_Id)
                .Index(t => t.Template_Id);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
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
                .ForeignKey("dbo.Images", t => t.Id)
                .Index(t => t.Id);
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TemplateImages", "TemplateId", "dbo.Templates");
            DropForeignKey("dbo.Compositions", "Template_Id", "dbo.TemplateImages");
            DropForeignKey("dbo.Compositions", "Overlay_Id", "dbo.Images");
            DropForeignKey("dbo.Compositions", "Background_Id", "dbo.Images");
            DropForeignKey("dbo.FileDatas", "Id", "dbo.Images");
            DropIndex("dbo.TemplateImages", new[] { "TemplateId" });
            DropIndex("dbo.FileDatas", new[] { "Id" });
            DropIndex("dbo.Compositions", new[] { "Template_Id" });
            DropIndex("dbo.Compositions", new[] { "Overlay_Id" });
            DropIndex("dbo.Compositions", new[] { "Background_Id" });
            DropTable("dbo.Templates");
            DropTable("dbo.TemplateImages");
            DropTable("dbo.FileDatas");
            DropTable("dbo.Images");
            DropTable("dbo.Compositions");
        }
    }
}
