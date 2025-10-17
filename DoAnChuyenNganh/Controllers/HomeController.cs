using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;


namespace DoAnChuyenNganh.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Title = "Giới thiệu";
            ViewBag.Message = "Chào mừng bạn đến với cửa hàng của chúng tôi!";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Title = "Liên hệ";
            ViewBag.Message = "Chúng tôi luôn sẵn sàng hỗ trợ bạn 24/7!";
            return View(new ContactModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(ContactModel model)
        {
            ViewBag.Title = "Liên hệ";
            ViewBag.Message = "Chúng tôi luôn sẵn sàng hỗ trợ bạn 24/7!";

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // --- Cấu hình gửi email ---
                var fromAddress = new MailAddress("krikshop.dacn.n2@gmail.com", "KRIK Shop");
                var toAddress = new MailAddress("krikshop.dacn.n2@gmail.com");
                const string fromPassword = "cafl dmri cnuq zdqb"; // thay bằng App Password 16 ký tự
                string subject = $"Liên hệ mới từ {model.Name}";
                string body = $"Họ và tên: {model.Name}\nEmail: {model.Email}\n\nNội dung:\n{model.Message}";

                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(fromAddress.Address, fromPassword);
                    smtp.EnableSsl = true;

                    var mail = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false
                    };

                    await smtp.SendMailAsync(mail);
                }

                TempData["Success"] = "Cảm ơn bạn! Liên hệ của bạn đã được gửi thành công.";
                return RedirectToAction("Contact");
            }
            catch (Exception)
            {
                TempData["Error"] = "Gửi thất bại! Vui lòng thử lại sau.";
                return View(model);
            }
        }
    }
}