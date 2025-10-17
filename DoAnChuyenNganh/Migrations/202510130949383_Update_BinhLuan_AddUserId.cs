namespace DoAnChuyenNganh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_BinhLuan_AddUserId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BinhLuan", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.BinhLuan", "UserId");
            AddForeignKey("dbo.BinhLuan", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BinhLuan", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.BinhLuan", new[] { "UserId" });
            DropColumn("dbo.BinhLuan", "UserId");
        }
    }
}
