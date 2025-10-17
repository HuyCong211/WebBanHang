using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace DoAnChuyenNganh.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Favorite
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Toggle(int productId)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Vui lòng đăng nhập để yêu thích sản phẩm." });

            var existing = db.Favorites.FirstOrDefault(f => f.UserId == userId && f.SanPhamId == productId);
            if (existing != null)
            {
                db.Favorites.Remove(existing);
                db.SaveChanges();
                return Json(new { success = true, liked = false });
            }

            db.Favorites.Add(new Favorite { UserId = userId, SanPhamId = productId, NgayTao = DateTime.Now });
            db.SaveChanges();
            return Json(new { success = true, liked = true });
        }

        // Hiển thị danh sách sản phẩm yêu thích
        public ActionResult MyFavorites()
        {
            var userId = User.Identity.GetUserId();
            var items = db.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => f.SanPham)
                .Include("AnhSanPhams")
                .AsNoTracking()
                .ToList();
            return View(items);
        }
    }
}