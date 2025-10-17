namespace DoAnChuyenNganh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Email_To_DiaChiGiaoHang : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DiaChiGiaoHang", "Email", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DiaChiGiaoHang", "Email");
        }
    }
}
