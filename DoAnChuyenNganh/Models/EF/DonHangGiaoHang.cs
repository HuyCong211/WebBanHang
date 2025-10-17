namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DonHangGiaoHang")]
    public partial class DonHangGiaoHang
    {
        [Key]
        public long DonHangGiaoId { get; set; }

        public long DonHangId { get; set; }

        public Guid? NguoiPhuTrach { get; set; }

        [StringLength(200)]
        public string MaDonHangNhaVanChuyen { get; set; }

        public int? TrangThai { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? NgayGiao { get; set; }

        public virtual DonHang DonHang { get; set; }
    }
}
