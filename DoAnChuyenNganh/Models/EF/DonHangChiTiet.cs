namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DonHangChiTiet")]
    public partial class DonHangChiTiet
    {
        public long DonHangChiTietId { get; set; }

        public long DonHangId { get; set; }

        public int BienTheId { get; set; }

        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }

        public decimal? ThanhTien { get; set; }

        [StringLength(500)]
        public string GhiChu { get; set; }

        public virtual BienTheSanPham BienTheSanPham { get; set; }

        public virtual DonHang DonHang { get; set; }
    }
}
