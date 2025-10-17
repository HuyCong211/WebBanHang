namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DiaChiGiaoHang")]
    public partial class DiaChiGiaoHang
    {
        [Key]
        public int DiaChiId { get; set; }

        public int? KhachHangId { get; set; }
        [StringLength(200)]
        public string Email { get; set; }

        [Required]
        [StringLength(200)]
        public string HoTen { get; set; }

        [Required]
        [StringLength(50)]
        public string DienThoai { get; set; }

        [Required]
        [StringLength(500)]
        public string DiaChiChiTiet { get; set; }

        [StringLength(200)]
        public string Tinh { get; set; }

        [StringLength(200)]
        public string Huyen { get; set; }

        [StringLength(200)]
        public string Xa { get; set; }

        public bool? MacDinh { get; set; }

        public virtual KhachHang KhachHang { get; set; }
    }
}
