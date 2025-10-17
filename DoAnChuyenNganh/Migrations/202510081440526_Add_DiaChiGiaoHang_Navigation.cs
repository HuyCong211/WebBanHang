namespace DoAnChuyenNganh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_DiaChiGiaoHang_Navigation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DonHang", "DiaChiGiaoHang_DiaChiId", c => c.Int());
            CreateIndex("dbo.DonHang", "DiaChiGiaoHang_DiaChiId");
            AddForeignKey("dbo.DonHang", "DiaChiGiaoHang_DiaChiId", "dbo.DiaChiGiaoHang", "DiaChiId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DonHang", "DiaChiGiaoHang_DiaChiId", "dbo.DiaChiGiaoHang");
            DropIndex("dbo.DonHang", new[] { "DiaChiGiaoHang_DiaChiId" });
            DropColumn("dbo.DonHang", "DiaChiGiaoHang_DiaChiId");
        }
    }
}
