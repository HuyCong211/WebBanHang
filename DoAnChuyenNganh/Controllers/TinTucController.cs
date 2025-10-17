using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnChuyenNganh.Controllers
{
    
    public class TinTucController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        // GET: TinTuc
        public ActionResult Index(int page = 1, int pageSize = 6)
        {
            var tinTucsData = db.TinTucs
                 .Where(t => t.TrangThai == true)
                 .OrderByDescending(t => t.NgayDang)
                 .Select(t => new
                 {
                     t.TinTucId,
                     t.TieuDe,
                     t.Slug,
                     t.TomTat,
                     t.NgayDang,
                     Anh = t.HinhAnhTinTucs.Select(a => a.Url).FirstOrDefault()
                 })
                 .ToList(); // ⚡ Thực thi query tại đây — tránh lỗi LINQ to Entities

            var tinTucs = tinTucsData
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TinTuc
                {
                    TinTucId = t.TinTucId,
                    TieuDe = t.TieuDe,
                    Slug = t.Slug,
                    TomTat = t.TomTat,
                    NgayDang = t.NgayDang,
                    HinhAnhTinTucs = new List<HinhAnhTinTuc> { new HinhAnhTinTuc { Url = t.Anh } }
                })
                .ToList();

            return View(tinTucs);
        }

        // Trang chi tiết tin tức
        public ActionResult Detail(string slug)
        {
            if (string.IsNullOrEmpty(slug)) return RedirectToAction("Index");

            var tinData = db.TinTucs
                .Where(t => t.Slug == slug && t.TrangThai == true)
                .Select(t => new
                {
                    t.TinTucId,
                    t.TieuDe,
                    t.NoiDung,
                    t.NgayDang,
                    HinhAnh = t.HinhAnhTinTucs.Select(a => a.Url).ToList()
                })
                .FirstOrDefault();

            if (tinData == null) return HttpNotFound();

            var tin = new TinTuc
            {
                TinTucId = tinData.TinTucId,
                TieuDe = tinData.TieuDe,
                NoiDung = tinData.NoiDung,
                NgayDang = tinData.NgayDang,
                HinhAnhTinTucs = tinData.HinhAnh.Select(a => new HinhAnhTinTuc { Url = a }).ToList()
            };

            return View(tin);
        }

        // Partial cho trang chủ - 3 tin mới nhất
        [ChildActionOnly]
        public ActionResult Partial_LatestNews()
        {
            var tinMoi = db.TinTucs
                .Where(t => t.TrangThai == true)
                .OrderByDescending(t => t.NgayDang)
                .Take(3)
                .Select(t => new
                {
                    t.TinTucId,
                    t.TieuDe,
                    t.Slug,
                    t.NgayDang,
                    t.TomTat,
                    Anh = t.HinhAnhTinTucs.Select(a => a.Url).FirstOrDefault()
                })
                .ToList() // ⚡ thực thi query tại đây
                .Select(t => new TinTuc
                {
                    TinTucId = t.TinTucId,
                    TieuDe = t.TieuDe,
                    Slug = t.Slug,
                    NgayDang = t.NgayDang,
                    TomTat = t.TomTat,
                    HinhAnhTinTucs = new List<HinhAnhTinTuc> { new HinhAnhTinTuc { Url = t.Anh } }
                })
                .ToList();

            return PartialView("_Partial_LatestNews", tinMoi);
        }
    }
}