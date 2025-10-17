namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LichSuGia")]
    public partial class LichSuGia
    {
        public int LichSuGiaId { get; set; }

        public int BienTheId { get; set; }

        public decimal? GiaCu { get; set; }

        public decimal? GiaMoi { get; set; }

        public Guid? NguoiThucHien { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? NgayThucHien { get; set; }

        public virtual BienTheSanPham BienTheSanPham { get; set; }
    }
}
