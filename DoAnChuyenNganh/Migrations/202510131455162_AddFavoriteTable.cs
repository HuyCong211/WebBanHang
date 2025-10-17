namespace DoAnChuyenNganh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFavoriteTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Favorite",
                c => new
                    {
                        FavoriteId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        SanPhamId = c.Int(nullable: false),
                        NgayTao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.FavoriteId)
                .ForeignKey("dbo.SanPham", t => t.SanPhamId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.SanPhamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Favorite", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Favorite", "SanPhamId", "dbo.SanPham");
            DropIndex("dbo.Favorite", new[] { "SanPhamId" });
            DropIndex("dbo.Favorite", new[] { "UserId" });
            DropTable("dbo.Favorite");
        }
    }
}
