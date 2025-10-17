namespace DoAnChuyenNganh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMapping : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DonHang", "DiaChiGiaoHangId");
            RenameColumn(table: "dbo.DonHang", name: "DiaChiGiaoHang_DiaChiId", newName: "DiaChiGiaoHangId");
            RenameIndex(table: "dbo.DonHang", name: "IX_DiaChiGiaoHang_DiaChiId", newName: "IX_DiaChiGiaoHangId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.DonHang", name: "IX_DiaChiGiaoHangId", newName: "IX_DiaChiGiaoHang_DiaChiId");
            RenameColumn(table: "dbo.DonHang", name: "DiaChiGiaoHangId", newName: "DiaChiGiaoHang_DiaChiId");
            AddColumn("dbo.DonHang", "DiaChiGiaoHangId", c => c.Int());
        }
    }
}
