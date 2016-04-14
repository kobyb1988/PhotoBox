namespace ImageMaker.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_last_photo_time : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sessions", "LastPhotoTime", c => c.Long());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sessions", "LastPhotoTime");
        }
    }
}
