using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnChuyenNganh.Services
{
    public class InventoryService
    {
        private readonly ApplicationDbContext db;

        public InventoryService(ApplicationDbContext context)
        {
            db = context;
        }

        /// <summary>
        /// Cập nhật tồn kho khi mua hàng thành công
        /// </summary>
        public void DecreaseStock(long donHangId, Guid? nguoiThucHien = null)
        {
            var orderDetails = db.DonHangChiTiets
                .Where(ct => ct.DonHangId == donHangId)
                .ToList();

            foreach (var detail in orderDetails)
            {
                var tonKho = db.TonKhoes.FirstOrDefault(t => t.BienTheId == detail.BienTheId);
                if (tonKho != null)
                {
                    tonKho.SoLuong = (tonKho.SoLuong ?? 0) - detail.SoLuong;
                    tonKho.NgayCapNhat = DateTime.Now;

                    // Ghi lịch sử
                    db.LichSuTonKhoes.Add(new LichSuTonKho
                    {
                        BienTheId = detail.BienTheId,
                        KhoId = tonKho.KhoId,
                        SoThayDoi = -detail.SoLuong,
                        GhiChu = $"Giảm tồn kho khi đặt đơn hàng #{donHangId}",
                        NguoiThucHien = nguoiThucHien,
                        NgayThucHien = DateTime.Now
                    });
                }
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Cập nhật tồn kho khi đơn hàng bị hủy
        /// </summary>
        public void IncreaseStock(long donHangId, Guid? nguoiThucHien = null)
        {
            var orderDetails = db.DonHangChiTiets
                .Where(ct => ct.DonHangId == donHangId)
                .ToList();

            foreach (var detail in orderDetails)
            {
                var tonKho = db.TonKhoes.FirstOrDefault(t => t.BienTheId == detail.BienTheId);
                if (tonKho != null)
                {
                    tonKho.SoLuong = (tonKho.SoLuong ?? 0) + detail.SoLuong;
                    tonKho.NgayCapNhat = DateTime.Now;

                    // Ghi lịch sử
                    db.LichSuTonKhoes.Add(new LichSuTonKho
                    {
                        BienTheId = detail.BienTheId,
                        KhoId = tonKho.KhoId,
                        SoThayDoi = detail.SoLuong,
                        GhiChu = $"Tăng tồn kho do hủy đơn hàng #{donHangId}",
                        NguoiThucHien = nguoiThucHien,
                        NgayThucHien = DateTime.Now
                    });
                }
            }
            db.SaveChanges();
        }
    }
}