using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DoAnChuyenNganh.Models.EF
{
    [Table("AnhSanPham_BienThe")]
    public class AnhSanPham_BienThe
    {
        [Key]
        [Column(Order = 1)]
        public int AnhSanPhamId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int BienTheId { get; set; }

        // Navigation properties
        [ForeignKey("AnhSanPhamId")]
        public virtual AnhSanPham AnhSanPham { get; set; }

        [ForeignKey("BienTheId")]
        public virtual BienTheSanPham BienTheSanPham { get; set; }
    }
}