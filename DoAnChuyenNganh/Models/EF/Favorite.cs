using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DoAnChuyenNganh.Models.EF
{
    [Table("Favorite")]
    public class Favorite
    {
        [Key]
        public int FavoriteId { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        public int SanPhamId { get; set; }

        [ForeignKey("SanPhamId")]
        public virtual SanPham SanPham { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}