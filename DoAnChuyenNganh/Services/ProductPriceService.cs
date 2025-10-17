using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;

namespace DoAnChuyenNganh.Services
{
    public class ProductPriceService
    {
        /// <summary>
        /// ✅ Tính giá cuối cùng (sản phẩm không có biến thể)
        /// </summary>
        public static (decimal finalPrice, decimal discountPercent, string promoName) GetFinalPrice(SanPham product)
        {
            if (product == null) return (0, 0, null);

            decimal giaGoc = product.GiaGoc ?? product.GiaBan;
            decimal giaBan = product.GiaBan;
            decimal giaCuoi = giaBan;
            string promoName = null;

            // 🔹 Lấy giá biến thể nhỏ nhất nếu có
            if (product.BienTheSanPhams?.Count > 0)
            {
                decimal? minPrice = null;
                foreach (var bt in product.BienTheSanPhams)
                {
                    var price = bt.Gia ?? giaBan;
                    if (minPrice == null || price < minPrice)
                        minPrice = price;
                }
                if (minPrice.HasValue && minPrice.Value > 0)
                    giaBan = minPrice.Value;
                giaCuoi = giaBan;
            }

            // 🔹 Áp dụng khuyến mãi (chỉ tính nếu có)
            if (product.KhuyenMais?.Count > 0)
            {
                var now = DateTime.Now;
                foreach (var km in product.KhuyenMais)
                {
                    if (km.TrangThai != true || km.NgayBatDau > now || km.NgayKetThuc < now)
                        continue;

                    promoName = promoName == null ? km.TenKM : promoName + " + " + km.TenKM;

                    if (!string.IsNullOrEmpty(km.Loai))
                    {
                        string loai = km.Loai.ToLowerInvariant();
                        decimal gt = km.GiaTri ?? 0;
                        if (loai.Contains("phantram"))
                            giaCuoi -= giaCuoi * (gt / 100);
                        else
                            giaCuoi -= gt;
                    }
                }
            }

            if (giaCuoi < 0) giaCuoi = 0;
            decimal discountPercent = giaGoc > 0 ? Math.Round((giaGoc - giaCuoi) / giaGoc * 100, 2) : 0;

            return (giaCuoi, discountPercent, promoName);
        }

        /// <summary>
        /// ✅ Tính giá cuối cùng (sản phẩm có biến thể)
        /// </summary>
        public static (decimal finalPrice, decimal discountPercent, string promoName) GetFinalPriceWithVariant(SanPham product)
        {
            if (product == null) return (0, 0, null);

            decimal giaGoc = product.GiaGoc ?? product.GiaBan;
            decimal giaBan = product.GiaBan;
            decimal giaCuoi = giaBan;
            string promoName = null;
            var now = DateTime.Now; // ✅ cache thời gian

            // 🔹 Tính giá biến thể nhanh hơn (thay vì LINQ)
            if (product.BienTheSanPhams?.Count > 0)
            {
                decimal minPromo = decimal.MaxValue;
                decimal minBase = decimal.MaxValue;

                foreach (var bt in product.BienTheSanPhams)
                {
                    if (bt.GiaKhuyenMai.HasValue && bt.GiaKhuyenMai.Value > 0 && bt.GiaKhuyenMai.Value < minPromo)
                        minPromo = bt.GiaKhuyenMai.Value;
                    if (bt.Gia.HasValue && bt.Gia.Value > 0 && bt.Gia.Value < minBase)
                        minBase = bt.Gia.Value;
                }

                if (minPromo != decimal.MaxValue)
                    giaBan = minPromo;
                else if (minBase != decimal.MaxValue)
                    giaBan = minBase;

                giaCuoi = giaBan;
            }

            // 🔹 Tính khuyến mãi nhanh hơn (bỏ LINQ)
            if (product.KhuyenMais?.Count > 0)
            {
                foreach (var km in product.KhuyenMais)
                {
                    if (km.TrangThai != true || km.NgayBatDau > now || km.NgayKetThuc < now)
                        continue;

                    promoName = promoName == null ? km.TenKM : promoName + " + " + km.TenKM;

                    if (!string.IsNullOrEmpty(km.Loai))
                    {
                        string loai = km.Loai.ToLowerInvariant();
                        decimal gt = km.GiaTri ?? 0;
                        if (loai.Contains("phantram"))
                            giaCuoi -= giaCuoi * (gt / 100);
                        else
                            giaCuoi -= gt;
                    }
                }
            }

            if (giaCuoi < 0) giaCuoi = 0;
            decimal discountPercent = giaGoc > 0 ? Math.Round((giaGoc - giaCuoi) / giaGoc * 100, 2) : 0;

            return (giaCuoi, discountPercent, promoName);
        }
    }
}











