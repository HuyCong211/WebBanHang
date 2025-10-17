namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BinhLuan")]
    public partial class BinhLuan
    {
        public int BinhLuanId { get; set; }

        public int SanPhamId { get; set; }
        [StringLength(200)]
        public string UserId { get; set; }
        public int? KhachHangId { get; set; }

        [Required]
        [StringLength(2000)]
        public string NoiDung { get; set; }

        public int? Sao { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? NgayTao { get; set; }

        public bool? TrangThai { get; set; }
        [ForeignKey("SanPhamId")]
        public virtual SanPham SanPham { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
