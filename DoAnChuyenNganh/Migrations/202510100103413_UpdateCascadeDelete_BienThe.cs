namespace DoAnChuyenNganh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCascadeDelete_BienThe : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AnhSanPham_BienThe", "BienTheId", "dbo.BienTheSanPham");
            AddForeignKey("dbo.AnhSanPham_BienThe", "BienTheId", "dbo.BienTheSanPham", "BienTheId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AnhSanPham_BienThe", "BienTheId", "dbo.BienTheSanPham");
            AddForeignKey("dbo.AnhSanPham_BienThe", "BienTheId", "dbo.BienTheSanPham", "BienTheId");
        }
    }
}
