using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnChuyenNganh.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BinhLuanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/BinhLuan
        public ActionResult Index(string tim, int? page)
        {
            var ds = db.BinhLuans
        .Include(b => b.SanPham)
        .Include(b => b.User)
        .OrderByDescending(b => b.NgayTao)
        .AsQueryable();

            if (!string.IsNullOrEmpty(tim))
            {
                string keyword = tim.Trim().ToLower();
                ds = ds.Where(b =>
                    (b.SanPham != null && b.SanPham.TenSanPham.ToLower().Contains(keyword)) ||
                    b.NoiDung.ToLower().Contains(keyword) ||
                    (b.User != null && b.User.UserName.ToLower().Contains(keyword))
                );
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;
            ViewBag.Search = tim;

            // ✅ Ép EF load navigation property (SanPham, User)
            var list = ds
                .Include(b => b.SanPham)
                .Include(b => b.User)
                .ToList();

            return View(list.ToPagedList(pageNumber, pageSize));

        }
        // ✅ Duyệt bình luận (AJAX)
        [HttpPost]
        public JsonResult Duyet(int id)
        {
            var bl = db.BinhLuans.Find(id);
            if (bl == null)
                return Json(new { success = false, message = "Không tìm thấy bình luận." });

            bl.TrangThai = true;
            db.SaveChanges();
            return Json(new { success = true, message = "Đã duyệt bình luận." });
        }

        // ✅ Ẩn bình luận (AJAX)
        [HttpPost]
        public JsonResult An(int id)
        {
            var bl = db.BinhLuans.Find(id);
            if (bl == null)
                return Json(new { success = false, message = "Không tìm thấy bình luận." });

            bl.TrangThai = false;
            db.SaveChanges();
            return Json(new { success = true, message = "Đã ẩn bình luận." });
        }

        // ✅ Xóa bình luận (AJAX)
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var bl = db.BinhLuans.Find(id);
            if (bl == null)
                return Json(new { success = false, message = "Không tìm thấy bình luận." });

            db.BinhLuans.Remove(bl);
            db.SaveChanges();
            return Json(new { success = true, message = "Đã xóa bình luận." });
        }
    }
}