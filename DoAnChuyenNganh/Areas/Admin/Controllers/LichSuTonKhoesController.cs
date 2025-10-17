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
using System.Web.UI;

namespace DoAnChuyenNganh.Areas.Admin.Controllers
{
    public class LichSuTonKhoesController : AdminBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/LichSuTonKhoes
        public ActionResult Index(string tim, int? page)
        {
            var lichSuTonKhoes = db.LichSuTonKhoes.Include(l => l.BienTheSanPham).Include(l => l.Kho);

            // Tìm kiếm theo tên thuộc tính
            if (!string.IsNullOrEmpty(tim))
            {
                lichSuTonKhoes = lichSuTonKhoes.Where(x => x.Kho.TenKho.Contains(tim) || x.BienTheSanPham.SKU.Contains(tim));
            }

            // Kích thước trang
            int pageSize = 10;
            int pageNumber = page ?? 1;

            // Lưu giá trị tìm kiếm để hiện lại trên view
            ViewBag.CurrentFilter = tim;

            return View(lichSuTonKhoes.OrderBy(x => x.LichSuId).ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/LichSuTonKhoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var lichSuTonKho = db.LichSuTonKhoes
                         .Include(d => d.BienTheSanPham) // load luôn danh mục cha
                         .Include(d => d.Kho)
                         .FirstOrDefault(d => d.LichSuId == id);
            if (lichSuTonKho == null)
            {
                return HttpNotFound();
            }
            return View(lichSuTonKho);
        }

        // GET: Admin/LichSuTonKhoes/Create
        public ActionResult Create()
        {
            ViewBag.BienTheId = new SelectList(db.BienTheSanPhams, "BienTheId", "SKU");
            ViewBag.KhoId = new SelectList(db.Khoes, "KhoId", "TenKho");
            return View();
        }

        // POST: Admin/LichSuTonKhoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LichSuId,BienTheId,KhoId,SoThayDoi,GhiChu,NguoiThucHien,NgayThucHien")] LichSuTonKho lichSuTonKho)
        {
            if (ModelState.IsValid)
            {
                lichSuTonKho.NgayThucHien = DateTime.Now;
                db.LichSuTonKhoes.Add(lichSuTonKho);
                // Tìm tồn kho hiện tại
                var tonKho = db.TonKhoes.FirstOrDefault(x => x.BienTheId == lichSuTonKho.BienTheId && x.KhoId == lichSuTonKho.KhoId);
                if (tonKho != null)
                {
                    // Cập nhật số lượng
                    tonKho.SoLuong += lichSuTonKho.SoThayDoi;
                    tonKho.NgayCapNhat = DateTime.Now;
                    db.Entry(tonKho).State = EntityState.Modified;
                }
                else
                {
                    // Nếu chưa có thì tạo mới
                    var newTonKho = new TonKho
                    {
                        BienTheId = lichSuTonKho.BienTheId,
                        KhoId = lichSuTonKho.KhoId,
                        SoLuong = lichSuTonKho.SoThayDoi,
                        NgayCapNhat = DateTime.Now
                    };
                    db.TonKhoes.Add(newTonKho);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BienTheId = new SelectList(db.BienTheSanPhams, "BienTheId", "SKU", lichSuTonKho.BienTheId);
            ViewBag.KhoId = new SelectList(db.Khoes, "KhoId", "TenKho", lichSuTonKho.KhoId);
            return View(lichSuTonKho);
        }

        // GET: Admin/LichSuTonKhoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LichSuTonKho lichSuTonKho = db.LichSuTonKhoes.Find(id);
            if (lichSuTonKho == null)
            {
                return HttpNotFound();
            }
            ViewBag.BienTheId = new SelectList(db.BienTheSanPhams, "BienTheId", "SKU", lichSuTonKho.BienTheId);
            ViewBag.KhoId = new SelectList(db.Khoes, "KhoId", "TenKho", lichSuTonKho.KhoId);
            return View(lichSuTonKho);
        }

        // POST: Admin/LichSuTonKhoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LichSuId,BienTheId,KhoId,SoThayDoi,GhiChu,NguoiThucHien,NgayThucHien")] LichSuTonKho lichSuTonKho)
        {
            if (ModelState.IsValid)
            {
                var old = db.LichSuTonKhoes.AsNoTracking().FirstOrDefault(x => x.LichSuId == lichSuTonKho.LichSuId);

                if (old != null)
                {
                    // Tìm tồn kho hiện tại
                    var tonKho = db.TonKhoes.FirstOrDefault(x => x.BienTheId == old.BienTheId && x.KhoId == old.KhoId);

                    if (tonKho != null)
                    {
                        // Trừ đi số cũ, cộng số mới
                        tonKho.SoLuong = tonKho.SoLuong - old.SoThayDoi + lichSuTonKho.SoThayDoi;
                        tonKho.NgayCapNhat = DateTime.Now;
                        db.Entry(tonKho).State = EntityState.Modified;
                    }
                }
                lichSuTonKho.NgayThucHien = DateTime.Now;
                db.Entry(lichSuTonKho).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BienTheId = new SelectList(db.BienTheSanPhams, "BienTheId", "SKU", lichSuTonKho.BienTheId);
            ViewBag.KhoId = new SelectList(db.Khoes, "KhoId", "TenKho", lichSuTonKho.KhoId);
            return View(lichSuTonKho);
        }

        // GET: Admin/LichSuTonKhoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var lichSuTonKho = db.LichSuTonKhoes
                        .Include(d => d.BienTheSanPham) // load luôn danh mục cha
                        .Include(d => d.Kho)
                        .FirstOrDefault(d => d.LichSuId == id);
            if (lichSuTonKho == null)
            {
                return HttpNotFound();
            }
            return View(lichSuTonKho);
        }

        // POST: Admin/LichSuTonKhoes/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var lichSuTonKho = db.LichSuTonKhoes.Find(id);

            if (lichSuTonKho == null)
            {
                return Json(new { success = false, message = "Không tìm thấy lịch sử tồn kho này." });
            }
            // Cập nhật tồn kho ngược lại
            var tonKho = db.TonKhoes.FirstOrDefault(x => x.BienTheId == lichSuTonKho.BienTheId && x.KhoId == lichSuTonKho.KhoId);
            if (tonKho != null)
            {
                tonKho.SoLuong -= lichSuTonKho.SoThayDoi; // hoàn tác thay đổi
                tonKho.NgayCapNhat = DateTime.Now;
                db.Entry(tonKho).State = EntityState.Modified;
            }

            db.LichSuTonKhoes.Remove(lichSuTonKho);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa thành công." });
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
            var lichSuTonKho = db.LichSuTonKhoes.Where(sp => idList.Contains(sp.LichSuId)).ToList();

            if (!lichSuTonKho.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy tồn kho." });
            }

            foreach (var lichSu in lichSuTonKho)
            {
                var tonKho = db.TonKhoes.FirstOrDefault(x => x.BienTheId == lichSu.BienTheId && x.KhoId == lichSu.KhoId);
                if (tonKho != null)
                {
                    tonKho.SoLuong -= lichSu.SoThayDoi;
                    tonKho.NgayCapNhat = DateTime.Now;
                    db.Entry(tonKho).State = EntityState.Modified;
                }
            }

            db.LichSuTonKhoes.RemoveRange(lichSuTonKho);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa thành công." });
        }
    }
}
