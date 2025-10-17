using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnChuyenNganh.Models.EF
{
    [Table("ThongKeDoanhThu")]
    public class ThongKeDoanhThu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Ngay { get; set; }

        [Required]
        [Column(TypeName = "decimal")]
        [Range(0, 9999999999999999.99)]
        public decimal DoanhThu { get; set; } = 0;

        [Required]
        public int DonHangHoanThanh { get; set; } = 0;

        [Required]
        public int SanPhamBanRa { get; set; } = 0;

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
    }
}