using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DoAnChuyenNganh.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DonHangsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/DonHangs
        public ActionResult Index(string search, int? trangThai)
        {
            IQueryable<DonHang> donHangs = db.DonHangs
                .Include("KhachHang")
                .Include("DonHangChiTiets")
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                donHangs = donHangs.Where(x =>
                    x.MaDonHang.Contains(search)
                    || x.KhachHangId.ToString().Contains(search)
                );
            }

            if (trangThai.HasValue)
            {
                donHangs = donHangs.Where(x => x.TrangThai == trangThai.Value);
            }

            var result = donHangs.OrderByDescending(x => x.NgayTao).ToList();

            ViewBag.TrangThai = trangThai;
            ViewBag.Search = search;

            return View(result);
        }

        // GET: Admin/DonHangs/Details/5
        public ActionResult Details(long? id)
        {
            var donHang = db.DonHangs
                .Include("DonHangChiTiets.BienTheSanPham")
                .Include("KhachHang")
                .Include("DonHangGiaoHangs")
                .AsNoTracking()
                .FirstOrDefault(x => x.DonHangId == id);

            if (donHang == null)
            {
                return HttpNotFound();
            }
            // Load thủ công để đảm bảo có dữ liệu
            foreach (var ct in donHang.DonHangChiTiets)
            {
                ct.BienTheSanPham = db.BienTheSanPhams
                    .Include(b => b.SanPham)
                    .Include(b => b.GiaTriThuocTinhs.Select(gt => gt.ThuocTinh))
                    .FirstOrDefault(b => b.BienTheId == ct.BienTheId);
            }
            // Địa chỉ giao hàng
            if (donHang.DiaChiGiaoHangId.HasValue)
                ViewBag.DiaChi = db.DiaChiGiaoHangs
                    .AsNoTracking()
                    .FirstOrDefault(x => x.DiaChiId == donHang.DiaChiGiaoHangId.Value);

            // Thông tin người dùng
            if (donHang.NguoiDungId.HasValue)
            {
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = userManager.FindById(donHang.NguoiDungId.ToString());
                ViewBag.FullName = user?.Fullname;
                ViewBag.UserPhone = user?.Phone;
                ViewBag.UserEmail = user?.Email;
            }


            return View(donHang);
        }
        // POST: Admin/DonHang/UpdateStatus
        [HttpPost]
        public JsonResult UpdateStatus(long id, int trangThai, string lyDo = null)
        {
            var donHang = db.DonHangs
         .Include("DiaChiGiaoHang")
         .FirstOrDefault(x => x.DonHangId == id);

            if (donHang == null)
                return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });

            donHang.TrangThai = trangThai;
            donHang.NgayCapNhat = DateTime.Now;
            if (!string.IsNullOrEmpty(lyDo))
                donHang.GhiChu = "Admin hủy đơn: " + lyDo;
            db.SaveChanges();


            // ✅ Hoàn lại tồn kho nếu Admin hủy đơn
            if (trangThai == 4)
            {
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
                                    GhiChu = $"Admin hoàn tồn kho khi hủy đơn #{donHang.MaDonHang}",
                                    NgayThucHien = DateTime.Now,
                                    NguoiThucHien = Guid.Parse(User.Identity.GetUserId())
                                });
                            }
                        }
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Lỗi hoàn tồn kho khi Admin hủy đơn: " + ex.Message);
                }
            }



            // 🔹 Chỉ gửi mail nếu đơn hàng bị hủy
            if (trangThai == 4)
            {
                try
                {
                    string toEmail = null;
                    string hoTen = "Quý khách";

                    // Nếu khách có tài khoản (đăng nhập)
                    if (donHang.NguoiDungId.HasValue)
                    {
                        var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                        var user = userManager.FindById(donHang.NguoiDungId.ToString());
                        toEmail = user?.Email;
                        hoTen = user?.Fullname ?? "Quý khách";
                    }
                    // Nếu là khách vãng lai (không có tài khoản)
                    else if (donHang.DiaChiGiaoHang != null)
                    {
                        hoTen = donHang.DiaChiGiaoHang.HoTen;
                        // Nếu bạn có cột Email trong form đặt hàng vãng lai — lấy ở đây:
                        toEmail = donHang.DiaChiGiaoHang.Email; // nếu chưa có, có thể null
                    }

                    // Nếu có email thì gửi
                    if (!string.IsNullOrEmpty(toEmail))
                    {
                        string subject = $"Thông báo hủy đơn hàng #{donHang.MaDonHang}";
                        string body = $@"
                            <h3>Xin chào {hoTen},</h3>
                            <p>Đơn hàng <strong>#{donHang.MaDonHang}</strong> của bạn đã bị hủy bởi quản trị viên.</p>
                            <p><strong>Lý do:</strong> {lyDo ?? "Không có"}</p>
                            <p>Ngày cập nhật: {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                            <p>Xin lỗi vì sự bất tiện này. Vui lòng liên hệ nếu cần hỗ trợ thêm.</p>
                            <br/>
                            <p>Trân trọng,<br/><strong>Hệ thống bán hàng Online của KRIK Shop</strong></p>
                        ";
                        SendMail(toEmail, subject, body);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = true, message = "Đơn hàng đã hủy nhưng gửi email thất bại: " + ex.Message });
                }
            }

            return Json(new { success = true, message = "Cập nhật trạng thái đơn hàng thành công!" });
        }




        private void SendMail(string toEmail, string subject, string body)
        {
            try
            {
                var fromEmail = "krikshop.dacn.n2@gmail.com"; // ✅ Email cửa hàng
                var fromPassword = "cafl dmri cnuq zdqb"; // ✅ App password Gmail (không dùng mật khẩu thật)

                using (var message = new System.Net.Mail.MailMessage())
                {
                    message.From = new System.Net.Mail.MailAddress(fromEmail, "KRIK Shop");
                    message.To.Add(toEmail);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    using (var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.EnableSsl = true;
                        smtp.Credentials = new System.Net.NetworkCredential(fromEmail, fromPassword);
                        smtp.Send(message);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ Lỗi gửi mail: " + ex.Message);
            }
        }


        // GET: Admin/DonHangs/Create
        public ActionResult Create()
        {
            ViewBag.KhachHangId = new SelectList(db.KhachHangs, "KhachHangId", "GioiTinh");
            return View();
        }

        // POST: Admin/DonHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DonHangId,MaDonHang,KhachHangId,NguoiDungId,TongTien,PhiVanChuyen,PhuongThucThanhToanId,DiaChiGiaoHangId,TrangThai,NgayTao,NgayCapNhat,GhiChu")] DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                db.DonHangs.Add(donHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.KhachHangId = new SelectList(db.KhachHangs, "KhachHangId", "GioiTinh", donHang.KhachHangId);
            return View(donHang);
        }

        // GET: Admin/DonHangs/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHang donHang = db.DonHangs.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }
            ViewBag.KhachHangId = new SelectList(db.KhachHangs, "KhachHangId", "GioiTinh", donHang.KhachHangId);
            return View(donHang);
        }

        // POST: Admin/DonHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DonHangId,MaDonHang,KhachHangId,NguoiDungId,TongTien,PhiVanChuyen,PhuongThucThanhToanId,DiaChiGiaoHangId,TrangThai,NgayTao,NgayCapNhat,GhiChu")] DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.KhachHangId = new SelectList(db.KhachHangs, "KhachHangId", "GioiTinh", donHang.KhachHangId);
            return View(donHang);
        }

        // GET: Admin/DonHangs/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHang donHang = db.DonHangs.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }
            return View(donHang);
        }

        // POST: Admin/DonHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            DonHang donHang = db.DonHangs.Find(id);
            db.DonHangs.Remove(donHang);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //thông báo có đơn hàng  mới

        [HttpGet]
        public JsonResult GetNewOrderCount()
        {
            var count = db.DonHangs.Count(x => x.TrangThai == 0); // 0 = Chờ xác nhận
            return Json(new { count = count }, JsonRequestBehavior.AllowGet);
        }


        // ✅ Đếm tổng số đơn bị hủy mà Admin chưa xử lý
        [HttpGet]
        public JsonResult GetNewCanceledOrders()
        {
            var count = db.DonHangs.Count(x => x.TrangThai == 4);
            return Json(new { count = count }, JsonRequestBehavior.AllowGet);
        }




    }
}
