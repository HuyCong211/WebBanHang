namespace DoAnChuyenNganh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveBienTheIdFromAnhSanPham : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AnhSanPham", "BienTheId", "dbo.BienTheSanPham");
            DropIndex("dbo.AnhSanPham", new[] { "BienTheId" });
            DropColumn("dbo.AnhSanPham", "BienTheId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AnhSanPham", "BienTheId", c => c.Int());
            CreateIndex("dbo.AnhSanPham", "BienTheId");
            AddForeignKey("dbo.AnhSanPham", "BienTheId", "dbo.BienTheSanPham", "BienTheId");
        }
    }
}
