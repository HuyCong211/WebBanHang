using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Text;
using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using System.Net.Mail;
using System.Net;

namespace DoAnChuyenNganh.Controllers
{
    public class ThanhToanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ✅ Thanh toán VNPay
        public ActionResult ThanhToanVnPay(long donHangId)
        {
            var donHang = db.DonHangs.FirstOrDefault(x => x.DonHangId == donHangId);
            if (donHang == null)
                return HttpNotFound();

            string url = ConfigurationManager.AppSettings["vnp_Url"];
            string returnUrl = ConfigurationManager.AppSettings["vnp_ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];


            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", tmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(donHang.TongTien * 100)).ToString());
            vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng: " + donHang.MaDonHang);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnpay.AddRequestData("vnp_TxnRef", donHang.MaDonHang);

            string paymentUrl = vnpay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);
        }

        // ✅ Kết quả trả về từ VNPay
        public ActionResult VnPayReturn()
        {
            string hashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
            var vnpayData = Request.QueryString;
            VnPayLibrary vnpay = new VnPayLibrary();


            foreach (string key in vnpayData)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    vnpay.AddResponseData(key, vnpayData[key]);
            }

            string vnp_SecureHash = vnpayData["vnp_SecureHash"];
            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, hashSecret);

            DonHang donHang = null; // ⚠️ khai báo ngoài if

            if (checkSignature)
            {
                string orderId = vnpay.GetResponseData("vnp_TxnRef");
                string responseCode = vnpay.GetResponseData("vnp_ResponseCode");

               donHang = db.DonHangs.AsNoTracking().Include("DiaChiGiaoHang").FirstOrDefault(x => x.MaDonHang == orderId);
                if (donHang == null)
                {
                    TempData["Error"] = "Không tìm thấy đơn hàng: " + orderId;
                    return View("VnPayError");
                }

                if (responseCode == "00")
                {
                    donHang.TrangThai = 0; // Đã thanh toán
                    db.SaveChanges();
                    // 🔹 Gửi email xác nhận thanh toán thành công
                    GuiMailThanhToanThanhCong(donHang, "VNPay");

                    return RedirectToAction("ThanhCong", "GioHangs", new { id = donHang.DonHangId });



                }
                else
                {
                    donHang.TrangThai = -1;
                    db.SaveChanges();
                    ViewBag.ResponseCode = responseCode;
                    return View("VnPayError");
                }
            }
            else
            {
                TempData["Error"] = "Sai chữ ký xác thực.";
                return View("VnPayError");
            }

        }



        // ✅ Thanh toán bằng chuyển khoản ngân hàng
        // GET: hiển thị form chuyển khoản demo
        public ActionResult ChuyenKhoan(long donHangId)
        {
            var donHang = db.DonHangs.FirstOrDefault(x => x.DonHangId == donHangId);
            if (donHang == null) return HttpNotFound();

            ViewBag.DonHang = donHang;
            return View();
        }

        // POST: demo người dùng nhập xong bấm xác nhận
        [HttpPost]
        public ActionResult XacNhanChuyenKhoan(long donHangId, string tenNguoiChuyen, string nganHangGui, string soTaiKhoanGui)
        {
            var donHang = db.DonHangs.AsNoTracking().Include("DiaChiGiaoHang").FirstOrDefault(x => x.DonHangId == donHangId);
            if (donHang == null) return HttpNotFound();

            // ❗ DEMO thôi: coi như đã thanh toán thành công
            donHang.TrangThai = 1; // Đã thanh toán
            db.SaveChanges();
            // 🔹 Gửi mail thông báo thanh toán thành công (Chuyển khoản)
            GuiMailThanhToanThanhCong(donHang, "Chuyển khoản");
            TempData["Success"] = $"Thanh toán demo thành công từ {tenNguoiChuyen} ({nganHangGui})";
            return RedirectToAction("ThanhCong", "GioHangs", new { id = donHang.DonHangId });
        }

        // ============================================================
        // 📨 HÀM GỬI MAIL KHI THANH TOÁN THÀNH CÔNG
        // ============================================================
        private void GuiMailThanhToanThanhCong(DonHang donHang, string phuongThuc)
        {
            try
            {
                var diaChi = donHang.DiaChiGiaoHang;
                string toEmail = diaChi?.Email;
                string hoTen = diaChi?.HoTen ?? "Quý khách";

                // Lấy chi tiết sản phẩm
                var chiTietDonHang = db.DonHangChiTiets
                    .Include("BienTheSanPham.SanPham")
                    .Include("BienTheSanPham.GiaTriThuocTinhs.ThuocTinh")
                    .Where(ct => ct.DonHangId == donHang.DonHangId)
                    .ToList();

                // Tạo bảng HTML sản phẩm
                StringBuilder chiTietHtml = new StringBuilder();
                chiTietHtml.Append("<table border='1' cellspacing='0' cellpadding='6' style='border-collapse: collapse; width:100%; font-family: Arial, sans-serif;'>");
                chiTietHtml.Append("<tr style='background-color:#f2f2f2; text-align:center; font-weight:bold;'>");
                chiTietHtml.Append("<th>Tên sản phẩm</th><th>Số lượng</th><th>Đơn giá</th><th>Thành tiền</th></tr>");

                foreach (var ct in chiTietDonHang)
                {
                    string tenSp = ct.BienTheSanPham?.SanPham?.TenSanPham ?? "Sản phẩm";
                    string mau = ct.BienTheSanPham?.GiaTriThuocTinhs?
                        .FirstOrDefault(g => g.ThuocTinh.TenThuocTinh == "Màu sắc")?.TenGiaTri;
                    string size = ct.BienTheSanPham?.GiaTriThuocTinhs?
                        .FirstOrDefault(g => g.ThuocTinh.TenThuocTinh == "Size")?.TenGiaTri;

                    string bienThe = "";
                    if (!string.IsNullOrEmpty(mau)) bienThe += $"Màu: {mau} ";
                    if (!string.IsNullOrEmpty(size)) bienThe += $" - Size: {size}";

                    chiTietHtml.Append("<tr>");
                    chiTietHtml.Append($"<td>{tenSp}<br/><small>{bienThe}</small></td>");
                    chiTietHtml.Append($"<td align='center'>{ct.SoLuong}</td>");
                    chiTietHtml.Append($"<td align='right'>{ct.DonGia:N0} đ</td>");
                    chiTietHtml.Append($"<td align='right'>{ct.ThanhTien:N0} đ</td>");
                    chiTietHtml.Append("</tr>");
                }

                chiTietHtml.Append("</table>");

                string subject = $"Xác nhận thanh toán thành công - Đơn hàng #{donHang.MaDonHang}";
                string body = $@"
                    <div style='font-family: Arial, sans-serif; font-size:14px; color:#333;'>
                        <h2 style='color:#2e7d32;'>Thanh toán thành công!</h2>
                        <p>Xin chào <strong>{hoTen}</strong>,</p>
                        <p>Bạn đã thanh toán thành công cho đơn hàng <strong>#{donHang.MaDonHang}</strong>.</p>
                        <p><strong>Ngày thanh toán:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}<br/>
                           <strong>Phương thức:</strong> {phuongThuc}<br/>
                           <strong>Tổng tiền:</strong> {donHang.TongTien:N0} đ
                        </p>

                        <h3>Chi tiết đơn hàng</h3>
                        {chiTietHtml}

                        <p style='margin-top:20px;'>Địa chỉ giao hàng:<br/>
                           <strong>{diaChi?.DiaChiChiTiet}, {diaChi?.Xa}, {diaChi?.Huyen}, {diaChi?.Tinh}</strong>
                        </p>

                        <p style='margin-top:25px;'>Cảm ơn bạn đã mua sắm tại <strong>KRIK SHOP</strong>.<br/>
                        Chúng tôi sẽ giao hàng sớm nhất có thể!</p>

                        <p style='margin-top:30px;'>
                            Trân trọng,<br/>
                            <strong>KRIK SHOP</strong><br/>
                            <a href='https://krikshop.vn' style='color:#d32f2f;'>https://krikshop.vn</a>
                        </p>
                    </div>
                ";

                if (!string.IsNullOrEmpty(toEmail))
                    SendMail(toEmail, subject, body);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ Lỗi gửi mail thanh toán: " + ex.Message);
            }
        }

        // ============================================================
        // 📨 HÀM GỬI MAIL CHUNG
        // ============================================================
        private void SendMail(string toEmail, string subject, string body)
        {
            try
            {
                var fromEmail = "krikshop.dacn.n2@gmail.com"; // ✅ Email cửa hàng
                var fromPassword = "cafl dmri cnuq zdqb";      // ✅ App password Gmail

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(fromEmail, "KRIK SHOP");
                    message.To.Add(toEmail);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential(fromEmail, fromPassword);
                        smtp.Send(message);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ Lỗi gửi mail: " + ex.Message);
            }
        }
    }
}


//- Tài khoản test: https://sandbox.vnpayment.vn

//- TMN Code: 2QXUI4J4

//- HashSecret: SECRETKEYDEMO

//- Test card:
//    + Số thẻ: 9704198526191432198
//    + Tên: NGUYEN VAN A
//    + Ngày: 07/15
//    + OTP: 123456