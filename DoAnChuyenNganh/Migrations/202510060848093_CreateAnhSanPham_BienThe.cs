namespace DoAnChuyenNganh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CreateAnhSanPham_BienThe : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnhSanPham_BienThe",
                c => new
                {
                    AnhSanPhamId = c.Int(nullable: false),
                    BienTheId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.AnhSanPhamId, t.BienTheId })
                .ForeignKey("dbo.AnhSanPham", t => t.AnhSanPhamId, cascadeDelete: false) // ❌ bỏ cascade
                .ForeignKey("dbo.BienTheSanPham", t => t.BienTheId, cascadeDelete: false) // ❌ bỏ cascade
                .Index(t => t.AnhSanPhamId)
                .Index(t => t.BienTheId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.AnhSanPham_BienThe", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.AnhSanPham_BienThe", "AnhSanPhamId", "dbo.AnhSanPham");
            DropIndex("dbo.AnhSanPham_BienThe", new[] { "BienTheId" });
            DropIndex("dbo.AnhSanPham_BienThe", new[] { "AnhSanPhamId" });
            DropTable("dbo.AnhSanPham_BienThe");
        }
    }
}
