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
    public class KhoesController : AdminBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Khoes
        public ActionResult Index(string tim, int? page)
        {

            var kho = db.Khoes.AsQueryable();

            // Tìm kiếm theo tên thuộc tính
            if (!string.IsNullOrEmpty(tim))
            {
                kho = kho.Where(x => x.TenKho.Contains(tim));
            }

            // Kích thước trang
            int pageSize = 10;
            int pageNumber = page ?? 1;

            // Lưu giá trị tìm kiếm để hiện lại trên view
            ViewBag.CurrentFilter = tim;

            return View(kho.OrderBy(x => x.KhoId).ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/Khoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kho kho = db.Khoes.Find(id);
            if (kho == null)
            {
                return HttpNotFound();
            }
            return View(kho);
        }

        // GET: Admin/Khoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Khoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "KhoId,TenKho,DiaChi,DienThoai,TrangThai")] Kho kho)
        {
            if (ModelState.IsValid)
            {
                db.Khoes.Add(kho);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(kho);
        }

        // GET: Admin/Khoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kho kho = db.Khoes.Find(id);
            if (kho == null)
            {
                return HttpNotFound();
            }
            return View(kho);
        }

        // POST: Admin/Khoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "KhoId,TenKho,DiaChi,DienThoai,TrangThai")] Kho kho)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kho).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kho);
        }

        // GET: Admin/Khoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kho kho = db.Khoes.Find(id);
            if (kho == null)
            {
                return HttpNotFound();
            }
            return View(kho);
        }

        // POST: Admin/Khoes/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var kho = db.Khoes.Find(id);

            if (kho == null)
            {
                return Json(new { success = false, message = "Không tìm thấy kho này." });
            }

            db.Khoes.Remove(kho);
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
            var kho = db.Khoes.Where(sp => idList.Contains(sp.KhoId)).ToList();

            if (!kho.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy kho." });
            }

            db.Khoes.RemoveRange(kho);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa thành công." });
        }
    }
}
