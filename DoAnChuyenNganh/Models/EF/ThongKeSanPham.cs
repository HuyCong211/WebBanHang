using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DoAnChuyenNganh.Models.EF
{
    [Table("ThongKeSanPham")]
    public class ThongKeSanPham
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SanPhamId { get; set; }

        [ForeignKey("SanPhamId")]
        public virtual SanPham SanPham { get; set; }

        public int LuotXem { get; set; } = 0;
        public int SoLuongBan { get; set; } = 0;
        public int SoLuongHuy_Khach { get; set; } = 0;
        public int SoLuongHuy_Admin { get; set; } = 0;

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
    }
}