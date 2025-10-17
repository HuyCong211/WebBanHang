using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnChuyenNganh.Areas.Admin.Controllers
{
    public class CauHinhController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // --- Cấu hình Hotline ---
        public ActionResult EditHotline()
        {
            var cauHinh = db.CauHinhs.FirstOrDefault(x => x.KeyName == "Hotline");
            if (cauHinh == null)
            {
                cauHinh = new CauHinh { KeyName = "Hotline", Value = "", MoTa = "Số điện thoại hotline hiển thị trên đầu trang" };
                db.CauHinhs.Add(cauHinh);
                db.SaveChanges();
            }
            return View(cauHinh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditHotline(CauHinh model)
        {
            if (ModelState.IsValid)
            {
                var cauHinh = db.CauHinhs.FirstOrDefault(x => x.KeyName == "Hotline");
                if (cauHinh != null)
                {
                    cauHinh.Value = model.Value;
                    db.SaveChanges();
                    TempData["Success"] = "Cập nhật Hotline thành công!";
                }
                return RedirectToAction("EditHotline");
            }
            return View(model);
        }

        // --- Cấu hình Email ---
        public ActionResult EditEmail()
        {
            var cauHinh = db.CauHinhs.FirstOrDefault(x => x.KeyName == "Email");
            if (cauHinh == null)
            {
                cauHinh = new CauHinh { KeyName = "Email", Value = "", MoTa = "Email hiển thị trên đầu trang" };
                db.CauHinhs.Add(cauHinh);
                db.SaveChanges();
            }
            return View(cauHinh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmail(CauHinh model)
        {
            if (ModelState.IsValid)
            {
                var cauHinh = db.CauHinhs.FirstOrDefault(x => x.KeyName == "Email");
                if (cauHinh != null)
                {
                    cauHinh.Value = model.Value;
                    db.SaveChanges();
                    TempData["Success"] = "Cập nhật Email thành công!";
                }
                return RedirectToAction("EditEmail");
            }
            return View(model);
        }
    }
}