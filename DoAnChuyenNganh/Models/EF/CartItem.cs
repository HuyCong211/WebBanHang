using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnChuyenNganh.Models.EF
{
    public class CartItem
    {
        public int BienTheId { get; set; }
        public string Name { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public string Image { get; set; }

        // thêm biến thể cho cart session
        public string Mau { get; set; }
        public string Size { get; set; }

        // Property tính tổng tiền của item
        public decimal ThanhTien
        {
            get { return DonGia * SoLuong; }
        }
    }
}