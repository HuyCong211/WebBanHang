namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TonKho")]
    public partial class TonKho
    {
        [DisplayName("Mã tồn kho")]
        public int TonKhoId { get; set; }
        
        public int BienTheId { get; set; }
      
        public int KhoId { get; set; }
        [DisplayName("Số lượng")]
        public int? SoLuong { get; set; }
        [DisplayName("Ngày cập nhật")]
        [Column(TypeName = "datetime2")]
        public DateTime? NgayCapNhat { get; set; }

        public virtual BienTheSanPham BienTheSanPham { get; set; }

        public virtual Kho Kho { get; set; }
    }
}
