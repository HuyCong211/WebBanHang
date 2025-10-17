namespace DoAnChuyenNganh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAnhSanPhamBienThe : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AnhSanPham_BienThe", "AnhSanPhamId", "dbo.AnhSanPham");
            DropForeignKey("dbo.AnhSanPham_BienThe", "BienTheId", "dbo.BienTheSanPham");
            AddForeignKey("dbo.AnhSanPham_BienThe", "AnhSanPhamId", "dbo.AnhSanPham", "AnhSanPhamId");
            AddForeignKey("dbo.AnhSanPham_BienThe", "BienTheId", "dbo.BienTheSanPham", "BienTheId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AnhSanPham_BienThe", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.AnhSanPham_BienThe", "AnhSanPhamId", "dbo.AnhSanPham");
            AddForeignKey("dbo.AnhSanPham_BienThe", "BienTheId", "dbo.BienTheSanPham", "BienTheId", cascadeDelete: true);
            AddForeignKey("dbo.AnhSanPham_BienThe", "AnhSanPhamId", "dbo.AnhSanPham", "AnhSanPhamId", cascadeDelete: true);
        }
    }
}
