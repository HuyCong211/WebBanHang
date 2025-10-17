namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LichSuTonKho")]
    public partial class LichSuTonKho
    {
        [Key]
        public int LichSuId { get; set; }

        public int BienTheId { get; set; }

        public int KhoId { get; set; }
        [DisplayName("Số thay đổi")]
        public int SoThayDoi { get; set; }
        [DisplayName("Ghi chú")]
        [StringLength(500)]
        public string GhiChu { get; set; }
        [DisplayName("Người thực hiện")]
        public Guid? NguoiThucHien { get; set; }
        [DisplayName("Ngày thực hiện")]
        [Column(TypeName = "datetime2")]
        public DateTime? NgayThucHien { get; set; }

        public virtual BienTheSanPham BienTheSanPham { get; set; }

        public virtual Kho Kho { get; set; }
    }
}
