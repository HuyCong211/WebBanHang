using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using PagedList;
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
    public class TonKhoesController : AdminBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/TonKhoes
        public ActionResult Index(string tim, int? page)
        {
            ViewBag.CurrentFilter = tim;
            var tonKho = db.TonKhoes.Include(t => t.BienTheSanPham.SanPham).Include(t => t.Kho);

            // Tìm kiếm theo tên thuộc tính
            if (!string.IsNullOrEmpty(tim))
            {
                tonKho = tonKho.Where(x => x.Kho.TenKho.Contains(tim)
                                        || x.BienTheSanPham.SKU.Contains(tim)
                                        || x.BienTheSanPham.SanPham.TenSanPham.Contains(tim));
            }

            // Kích thước trang
            int pageSize = 10;
            int pageNumber = page ?? 1;

            // Lưu giá trị tìm kiếm để hiện lại trên view
            ViewBag.CurrentFilter = tim;

            return View(tonKho.OrderByDescending(x => x.TonKhoId).ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/TonKhoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tonKho = db.TonKhoes
                          .Include(d => d.BienTheSanPham) // load luôn danh mục cha
                          .Include(d => d.Kho)
                          .FirstOrDefault(d => d.TonKhoId == id);
            if (tonKho == null)
            {
                return HttpNotFound();
            }
            return View(tonKho);
        }

        // GET: Admin/TonKhoes/Create
        public ActionResult Create()
        {
            //ViewBag.BienTheId = new SelectList(db.BienTheSanPhams, "BienTheId", "SKU");
            ViewBag.BienTheList = db.BienTheSanPhams.Include(x => x.SanPham).ToList();
            ViewBag.KhoId = new SelectList(db.Khoes, "KhoId", "TenKho");
            return View();
        }

        // POST: Admin/TonKhoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TonKhoId,BienTheId,KhoId,SoLuong,NgayCapNhat")] TonKho tonKho, int KhoId, int[] BienTheIds, int SoLuong)
        {
            if (BienTheIds == null || BienTheIds.Length == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất một biến thể.");
            }

            if (ModelState.IsValid)
            {
                foreach (var id in BienTheIds)
                {
                    var ton = new TonKho
                    {
                        BienTheId = id,
                        KhoId = KhoId,
                        SoLuong = SoLuong,
                        NgayCapNhat = DateTime.Now
                    };
                    db.TonKhoes.Add(ton);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BienTheList = db.BienTheSanPhams.Include(x => x.SanPham).ToList();
            ViewBag.KhoId = new SelectList(db.Khoes, "KhoId", "TenKho", KhoId);
            return View();

        }

        // GET: Admin/TonKhoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TonKho tonKho = db.TonKhoes.Find(id);
            if (tonKho == null)
            {
                return HttpNotFound();
            }
            ViewBag.BienTheId = new SelectList(db.BienTheSanPhams, "BienTheId", "SKU", tonKho.BienTheId);
            ViewBag.KhoId = new SelectList(db.Khoes, "KhoId", "TenKho", tonKho.KhoId);
            return View(tonKho);
        }

        // POST: Admin/TonKhoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TonKhoId,BienTheId,KhoId,SoLuong,NgayCapNhat")] TonKho tonKho)
        {
            if (ModelState.IsValid)
            {
                tonKho.NgayCapNhat = DateTime.Now;
                db.Entry(tonKho).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BienTheId = new SelectList(db.BienTheSanPhams, "BienTheId", "SKU", tonKho.BienTheId);
            ViewBag.KhoId = new SelectList(db.Khoes, "KhoId", "TenKho", tonKho.KhoId);
            return View(tonKho);
        }

        // GET: Admin/TonKhoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tonKho = db.TonKhoes
                        .Include(d => d.BienTheSanPham) 
                        .Include(d => d.Kho)
                        .FirstOrDefault(d => d.TonKhoId == id);
            if (tonKho == null)
            {
                return HttpNotFound();
            }
            return View(tonKho);
        }

        // POST: Admin/TonKhoes/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var tonKho = db.TonKhoes.Find(id);

            if (tonKho == null)
            {
                return Json(new { success = false, message = "Không tìm thấy tồn kho này." });
            }

            db.TonKhoes.Remove(tonKho);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa kho thành công." });

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }




        [HttpPost]
        public JsonResult DeleteAll(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return Json(new { success = false, message = "Không có bản ghi nào được chọn." });
            }

            var idList = ids.Split(',').Select(x => Convert.ToInt32(x)).ToList();
            var tonKho = db.TonKhoes.Where(sp => idList.Contains(sp.TonKhoId)).ToList();

            if (!tonKho.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy tồn kho." });
            }

            db.TonKhoes.RemoveRange(tonKho);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa thành công." });
        }


        // API hỗ trợ Select2 tìm kiếm biến thể
        [HttpGet]
        public JsonResult GetBienTheSanPhams(string q)
        {
            var list = db.BienTheSanPhams
                .Include(b => b.SanPham)
                .Where(b => string.IsNullOrEmpty(q) || b.SKU.Contains(q) || b.SanPham.TenSanPham.Contains(q))
                .Select(b => new
                {
                    id = b.BienTheId,
                    text = b.SKU + " - " + b.SanPham.TenSanPham
                }).Take(20).ToList();

            return Json(new { results = list }, JsonRequestBehavior.AllowGet);
        }

    }
}
