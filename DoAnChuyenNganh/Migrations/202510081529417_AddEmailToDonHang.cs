namespace DoAnChuyenNganh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmailToDonHang : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DonHang", "Email", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DonHang", "Email");
        }
    }
}
