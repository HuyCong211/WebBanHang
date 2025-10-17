using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnChuyenNganh.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ThongKeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/ThongKe
        public ActionResult Index()
        {
            var tongDon = db.DonHangs.AsNoTracking();

            ViewBag.TotalOrders = tongDon.Count();
            ViewBag.TotalCompleted = tongDon.Count(x => x.TrangThai == 3);
            ViewBag.TotalPending = tongDon.Count(x => x.TrangThai == 0 || x.TrangThai == 1 || x.TrangThai == 2);
            ViewBag.TotalCanceled = tongDon.Count(x => x.TrangThai == 4);

            return View();
        }


        // ===============================
        // ✅ API: Thống kê doanh thu theo thời gian
        // ===============================
        [HttpGet]
        public ActionResult GetDoanhThu(string mode = "day", string from = null, string to = null, int? month = null, int? year = null)
        {
            try
            {
                DateTime today = DateTime.Today;
                DateTime fromDate = today, toDate = today;

                IQueryable<ThongKeDoanhThu> query = db.ThongKeDoanhThus.AsNoTracking();
                var list = new List<object>();

                // ======================= THEO NGÀY =======================
                if (mode == "day")
                {
                    if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
                    {
                        fromDate = today;
                        toDate = today;
                    }
                    else
                    {
                        fromDate = DateTime.Parse(from);
                        toDate = DateTime.Parse(to);
                    }

                    var data = query
                        .Where(x => DbFunctions.TruncateTime(x.Ngay) >= fromDate && DbFunctions.TruncateTime(x.Ngay) <= toDate)
                        .GroupBy(x => DbFunctions.TruncateTime(x.Ngay))
                        .Select(g => new { Ngay = g.Key.Value, DoanhThu = g.Sum(x => (decimal?)x.DoanhThu) ?? 0 })
                        .ToList();

                    for (var d = fromDate; d <= toDate; d = d.AddDays(1))
                    {
                        var found = data.FirstOrDefault(x => x.Ngay == d);
                        list.Add(new { Label = d.ToString("dd/MM/yyyy"), DoanhThu = found?.DoanhThu ?? 0 });
                    }
                }

                // ======================= THEO THÁNG =======================
                else if (mode == "month")
                {
                    int selectedMonth = month ?? today.Month;
                    int selectedYear = year ?? today.Year;
                    fromDate = new DateTime(selectedYear, selectedMonth, 1);
                    toDate = new DateTime(selectedYear, selectedMonth, DateTime.DaysInMonth(selectedYear, selectedMonth));

                    var data = query
                        .Where(x => x.Ngay.Year == selectedYear && x.Ngay.Month == selectedMonth)
                        .GroupBy(x => x.Ngay.Day)
                        .Select(g => new { Day = g.Key, DoanhThu = g.Sum(x => (decimal?)x.DoanhThu) ?? 0 })
                        .ToList();

                    for (int d = 1; d <= DateTime.DaysInMonth(selectedYear, selectedMonth); d++)
                    {
                        var found = data.FirstOrDefault(x => x.Day == d);
                        list.Add(new { Label = $"Ngày {d}/{selectedMonth}", DoanhThu = found?.DoanhThu ?? 0 });
                    }
                }

                // ======================= THEO NĂM =======================
                else if (mode == "year")
                {
                    int selectedYear = year ?? today.Year;
                    fromDate = new DateTime(selectedYear, 1, 1);
                    toDate = new DateTime(selectedYear, 12, 31);

                    var data = query
                        .Where(x => x.Ngay.Year == selectedYear)
                        .GroupBy(x => x.Ngay.Month)
                        .Select(g => new { Month = g.Key, DoanhThu = g.Sum(x => (decimal?)x.DoanhThu) ?? 0 })
                        .ToList();

                    for (int m = 1; m <= 12; m++)
                    {
                        var found = data.FirstOrDefault(x => x.Month == m);
                        list.Add(new { Label = $"Tháng {m}/{selectedYear}", DoanhThu = found?.DoanhThu ?? 0 });
                    }
                }

                decimal tongDoanhThu = list.Sum(x => (decimal)x.GetType().GetProperty("DoanhThu").GetValue(x));

                return Json(new
                {
                    data = list,
                    tongDoanhThu,
                    mode,
                    from = fromDate.ToString("dd/MM/yyyy"),
                    to = toDate.ToString("dd/MM/yyyy"),
                    month,
                    year
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }




        // ===============================
        // ✅ API: Biểu đồ sản phẩm (lượt xem / bán / hủy)
        // ===============================
        [HttpGet]
        public ActionResult GetThongKeSanPham(string type)
        {
            var query = db.ThongKeSanPhams
                .AsNoTracking()
                .Include(x => x.SanPham)
                .Select(x => new
                {
                    TenSanPham = x.SanPham.TenSanPham,
                    x.LuotXem,
                    x.SoLuongBan,
                    SoLuongHuy = x.SoLuongHuy_Khach // ✅ chỉ còn 1 cột hủy
                })
                .ToList();

            IEnumerable<object> data;

            if (type == "view")
            {
                data = query.OrderByDescending(x => x.LuotXem)
                            .Take(10)
                            .Select(x => new { x.TenSanPham, GiaTri = x.LuotXem });
            }
            else if (type == "buy")
            {
                data = query.OrderByDescending(x => x.SoLuongBan)
                            .Take(10)
                            .Select(x => new { x.TenSanPham, GiaTri = x.SoLuongBan });
            }
            else if (type == "cancel")
            {
                data = query.OrderByDescending(x => x.SoLuongHuy)
                            .Take(10)
                            .Select(x => new { x.TenSanPham, GiaTri = x.SoLuongHuy });
            }
            else
            {
                data = new List<object>();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // ===============================
        // ✅ API: Lý do hủy đơn phổ biến
        // ===============================
        [HttpGet]
        public ActionResult GetLyDoHuy()
        {
            var data = db.DonHangs.AsNoTracking()
                .Where(d => d.TrangThai == 4)
                .GroupBy(d => d.GhiChu)
                .Select(g => new
                {
                    LyDo = g.Key ?? "(Không ghi chú)",
                    SoLan = g.Count()
                })
                .OrderByDescending(x => x.SoLan)
                .Take(10)
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CapNhatThongKe()
        {
            var today = DateTime.Now.Date;

            // Xóa thống kê cũ nếu có
            var existing = db.ThongKeDoanhThus.FirstOrDefault(x => DbFunctions.TruncateTime(x.Ngay) == today);
            if (existing != null)
                db.ThongKeDoanhThus.Remove(existing);

            // --- Lấy đơn hoàn thành hôm nay ---
            var donHoanThanh = db.DonHangs
                .Include(d => d.DonHangChiTiets)
                .Where(d => d.TrangThai == 3 && DbFunctions.TruncateTime(d.NgayCapNhat) == today)
                .ToList();

            var tongDoanhThu = donHoanThanh.Sum(x => x.TongTien);
            var tongDon = donHoanThanh.Count;
            var tongSP = donHoanThanh.SelectMany(x => x.DonHangChiTiets).Sum(ct => ct.SoLuong);

            db.ThongKeDoanhThus.Add(new ThongKeDoanhThu
            {
                Ngay = today,
                DoanhThu = tongDoanhThu,
                DonHangHoanThanh = tongDon,
                SanPhamBanRa = tongSP,
                NgayCapNhat = DateTime.Now
            });

            // --- Cập nhật thống kê sản phẩm ---
            var allSP = db.SanPhams.AsNoTracking().Select(x => new { x.SanPhamId, x.LuotXem }).ToList();
            foreach (var sp in allSP)
            {
                var tk = db.ThongKeSanPhams.FirstOrDefault(x => x.SanPhamId == sp.SanPhamId);
                if (tk == null)
                {
                    tk = new ThongKeSanPham { SanPhamId = sp.SanPhamId };
                    db.ThongKeSanPhams.Add(tk);
                }

                var soLuongBan = db.DonHangChiTiets
                    .Where(ct => ct.BienTheSanPham.SanPhamId == sp.SanPhamId && ct.DonHang.TrangThai == 3)
                    .Sum(ct => (int?)ct.SoLuong) ?? 0;

                var soLuongHuy = db.DonHangChiTiets
                    .Where(ct => ct.BienTheSanPham.SanPhamId == sp.SanPhamId && ct.DonHang.TrangThai == 4)
                    .Sum(ct => (int?)ct.SoLuong) ?? 0;

                tk.LuotXem = sp.LuotXem ?? 0;
                tk.SoLuongBan = soLuongBan;
                tk.SoLuongHuy_Khach = soLuongHuy;
                tk.NgayCapNhat = DateTime.Now;
            }

            db.SaveChanges();

            return Json(new { success = true, message = "✅ Đã cập nhật dữ liệu thống kê thành công!" });
        }

    }
}