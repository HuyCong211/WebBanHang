using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace DoAnChuyenNganh.Controllers
{

    public class KhachHangController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // 🟢 Danh sách đơn hàng của tôi
        // - Nếu ĐĂNG NHẬP: lấy theo NguoiDungId
        // - Nếu KHÁCH: lấy theo các id đã "mở quyền" trong Session (GuestOrderIds)
        // GET: KhachHang
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DonHangCuaToi()
        {
            var isAuth = User.Identity.IsAuthenticated;
            List<DonHang> donHangs;

            if (isAuth)
            {
                var userId = User.Identity.GetUserId();
                {
                    Guid guidUserId = Guid.Parse(userId);
                    donHangs = db.DonHangs
                        .Include(d => d.DiaChiGiaoHang)
                        .Where(d => d.NguoiDungId == guidUserId)
                        .OrderByDescending(d => d.NgayTao)
                        .ToList();
                }

            }
            else
            {
                var ids = (List<long>)Session["GuestOrderIds"] ?? new List<long>();
                if (ids.Count == 0)
                {
                    // Không có id nào trong session => mời tra cứu
                    TempData["Info"] = "Bạn chưa đăng nhập. Hãy tra cứu đơn theo mã đơn & email.";
                    return RedirectToAction("TraCuu");
                }

                donHangs = db.DonHangs
                    .Include(d => d.DiaChiGiaoHang)
                    .Where(d => ids.Contains(d.DonHangId))
                    .OrderByDescending(d => d.NgayTao)
                    .ToList();
            }

            return View(donHangs);
        }

        // 🟢 Tra cứu đơn hàng cho KHÁCH: nhập Mã đơn + Email
        [HttpGet]
        public ActionResult TraCuu()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TraCuu(string maDonHang, string email)
        {
            if (string.IsNullOrWhiteSpace(maDonHang) || string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Vui lòng nhập đủ Mã đơn và Email.";
                return RedirectToAction("TraCuu");
            }

            var donHang = db.DonHangs
                .Include(d => d.DiaChiGiaoHang)
                .FirstOrDefault(d => d.MaDonHang == maDonHang && d.DiaChiGiaoHang.Email == email);

            if (donHang == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng phù hợp.";
                return RedirectToAction("TraCuu");
            }

            // 🔹 cấp quyền xem trong session
            var ids = (List<long>)Session["GuestOrderIds"] ?? new List<long>();
            if (!ids.Contains(donHang.DonHangId)) ids.Add(donHang.DonHangId);
            Session["GuestOrderIds"] = ids;

            return RedirectToAction("ChiTietDonHang", new { id = donHang.DonHangId });
        }

        // 🟢 Chi tiết đơn hàng: cho phép xem nếu:
        // - Đăng nhập & là chủ đơn, hoặc
        // - Khách có id trong Session["GuestOrderIds"]
        public ActionResult ChiTietDonHang(long id)
        {
            // ✅ Không cần tracking vì chỉ xem
            var donHang = db.DonHangs
                .AsNoTracking()
                .Include(d => d.DiaChiGiaoHang)
                .Include(d => d.DonHangChiTiets.Select(ct => ct.BienTheSanPham))
                .FirstOrDefault(d => d.DonHangId == id);

            if (donHang == null)
                return HttpNotFound();

            // ✅ Lấy danh sách ID biến thể có trong đơn
            var bienTheIds = donHang.DonHangChiTiets
                .Where(ct => ct.BienTheSanPham != null)
                .Select(ct => ct.BienTheSanPham.BienTheId)
                .ToList();

            if (bienTheIds.Any())
            {
                // ⚡ Lấy ảnh đúng cho biến thể (và kèm tên sản phẩm)
                var bienThes = db.BienTheSanPhams
                    .AsNoTracking()
                    .Where(b => bienTheIds.Contains(b.BienTheId))
                    .Include(b => b.SanPham)
                    .Include(b => b.AnhSanPham_BienThes.Select(ab => ab.AnhSanPham))
                    .Include(b => b.GiaTriThuocTinhs.Select(gt => gt.ThuocTinh))
                    .ToList();

                foreach (var ct in donHang.DonHangChiTiets)
                {
                    var b = bienThes.FirstOrDefault(x => x.BienTheId == ct.BienTheSanPham.BienTheId);
                    if (b != null)
                    {
                        ct.BienTheSanPham = b;
                    }
                }
            }

            // ✅ Kiểm tra quyền truy cập
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                Guid guidUserId = Guid.Parse(userId);
                if (donHang.NguoiDungId != guidUserId)
                    return new HttpUnauthorizedResult();
            }
            else
            {
                var ids = (List<long>)Session["GuestOrderIds"] ?? new List<long>();
                if (!ids.Contains(id))
                    return new HttpUnauthorizedResult();
            }

            return View(donHang);
        }




        // 🟠 Huỷ đơn – chỉ cho phép khi trạng thái 0 (Chờ) hoặc 1 (Đã xác nhận)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HuyDonHang(long id, string lyDo)
        {
            var donHang = db.DonHangs
                .Include(d => d.DiaChiGiaoHang)
                .FirstOrDefault(d => d.DonHangId == id);

            if (donHang == null) return HttpNotFound();

            // Quyền: đã đăng nhập là chủ đơn, hoặc khách có id trong Session
            bool allowed = false;
            if (User.Identity.IsAuthenticated)
            {
                Guid guidUserId = Guid.Parse(User.Identity.GetUserId());
                allowed = (donHang.NguoiDungId == guidUserId);

            }
            else
            {
                var ids = (List<long>)Session["GuestOrderIds"] ?? new List<long>();
                allowed = ids.Contains(id);
            }
            if (!allowed) return new HttpUnauthorizedResult();

            // Check trạng thái
            if (!(donHang.TrangThai == 0 || donHang.TrangThai == 1))
            {
                TempData["Error"] = "Đơn hàng không thể huỷ (đã giao hoặc đã hoàn tất).";
                return RedirectToAction("ChiTietDonHang", new { id = id });
            }

            // Cập nhật DB
            donHang.TrangThai = 4; // Đã huỷ
            donHang.GhiChu = string.IsNullOrWhiteSpace(lyDo) ? "Khách huỷ đơn" : ("Khách huỷ đơn: " + lyDo);
            donHang.NgayCapNhat = DateTime.Now;
            db.SaveChanges();

            // ✅ Hoàn lại tồn kho (tối ưu, không load dư)
            try
            {
                var chiTiets = db.DonHangChiTiets
                    .Where(ct => ct.DonHangId == donHang.DonHangId)
                    .Select(ct => new { ct.BienTheId, ct.SoLuong })
                    .ToList();

                if (chiTiets.Any())
                {
                    var bienTheIds = chiTiets.Select(c => c.BienTheId).ToList();
                    var tonKhoList = db.TonKhoes
                        .Where(t => bienTheIds.Contains(t.BienTheId))
                        .ToList();

                    foreach (var ct in chiTiets)
                    {
                        var tonKho = tonKhoList.FirstOrDefault(t => t.BienTheId == ct.BienTheId);
                        if (tonKho != null)
                        {
                            tonKho.SoLuong = (tonKho.SoLuong ?? 0) + ct.SoLuong;
                            tonKho.NgayCapNhat = DateTime.Now;

                            db.LichSuTonKhoes.Add(new LichSuTonKho
                            {
                                BienTheId = ct.BienTheId,
                                KhoId = tonKho.KhoId,
                                SoThayDoi = ct.SoLuong,
                                GhiChu = $"Hoàn tồn kho khi khách huỷ đơn #{donHang.MaDonHang}",
                                NgayThucHien = DateTime.Now,
                                NguoiThucHien = User.Identity.IsAuthenticated
                                    ? Guid.Parse(User.Identity.GetUserId())
                                    : (Guid?)null
                            });
                        }
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Lỗi hoàn tồn kho khi hủy đơn: " + ex.Message);
            }


            // Gửi mail
            try
            {
                var toEmail = donHang.DiaChiGiaoHang?.Email;
                var hoTen = donHang.DiaChiGiaoHang?.HoTen ?? "Quý khách";
                if (!string.IsNullOrWhiteSpace(toEmail))
                {
                    string subject = $"Xác nhận huỷ đơn hàng #{donHang.MaDonHang}";
                    var sb = new StringBuilder();
                    sb.AppendLine($"<h3>Xin chào {hoTen},</h3>");
                    sb.AppendLine($"<p>Đơn hàng <strong>#{donHang.MaDonHang}</strong> của bạn đã được huỷ thành công.</p>");
                    if (!string.IsNullOrWhiteSpace(lyDo))
                        sb.AppendLine($"<p><strong>Lý do:</strong> {lyDo}</p>");
                    sb.AppendLine($"<p>Ngày huỷ: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
                    sb.AppendLine("<br/><p>Trân trọng,<br/><strong>KRIKSHOP</strong></p>");

                    SendMail(toEmail, subject, sb.ToString());
                }
            }
            catch (Exception ex)
            {
                // log nếu cần
                System.Diagnostics.Debug.WriteLine("Lỗi gửi mail huỷ đơn: " + ex.Message);
            }

            TempData["Success"] = "Huỷ đơn hàng thành công.";
            return RedirectToAction("ChiTietDonHang", new { id = id });
        }

        // 📧 Gửi mail
        private void SendMail(string toEmail, string subject, string bodyHtml)
        {
            var fromEmail = "krikshop.dacn.n2@gmail.com";
            var appPassword = "cafl dmri cnuq zdqb"; // App Password

            using (var msg = new MailMessage())
            {
                msg.From = new MailAddress(fromEmail, "KRIK Shop");
                msg.To.Add(toEmail);
                msg.Subject = subject;
                msg.Body = bodyHtml;
                msg.IsBodyHtml = true;

                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential(fromEmail, appPassword);
                    smtp.Send(msg);
                }
            }
        }
    }
}