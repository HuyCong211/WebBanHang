namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GioHangChiTiet")]
    public partial class GioHangChiTiet
    {
        public long GioHangChiTietId { get; set; }

        public long GioHangId { get; set; }

        public int BienTheId { get; set; }

        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }

        public decimal? ThanhTien { get; set; }

        public virtual BienTheSanPham BienTheSanPham { get; set; }

        public virtual GioHang GioHang { get; set; }
    }
}
