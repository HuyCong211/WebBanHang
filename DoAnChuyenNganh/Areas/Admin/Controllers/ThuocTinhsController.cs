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
    public class ThuocTinhsController : AdminBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/ThuocTinhs
        public ActionResult Index(string tim, int? page)
        {
            var thuocTinhs = db.ThuocTinhs.AsQueryable();

            // Tìm kiếm theo tên thuộc tính
            if (!string.IsNullOrEmpty(tim))
            {
                thuocTinhs = thuocTinhs.Where(x => x.TenThuocTinh.Contains(tim));
            }

            // Kích thước trang
            int pageSize = 10;
            int pageNumber = page ?? 1;

            // Lưu giá trị tìm kiếm để hiện lại trên view
            ViewBag.CurrentFilter = tim;

            return View(thuocTinhs.OrderBy(x => x.ThuocTinhId).ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/ThuocTinhs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThuocTinh thuocTinh = db.ThuocTinhs.Find(id);
            if (thuocTinh == null)
            {
                return HttpNotFound();
            }
            return View(thuocTinh);
        }

        // GET: Admin/ThuocTinhs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ThuocTinhs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ThuocTinhId,TenThuocTinh")] ThuocTinh thuocTinh)
        {
            if (ModelState.IsValid)
            {
                db.ThuocTinhs.Add(thuocTinh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(thuocTinh);
        }

        // GET: Admin/ThuocTinhs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThuocTinh thuocTinh = db.ThuocTinhs.Find(id);
            if (thuocTinh == null)
            {
                return HttpNotFound();
            }
            return View(thuocTinh);
        }

        // POST: Admin/ThuocTinhs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ThuocTinhId,TenThuocTinh")] ThuocTinh thuocTinh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thuocTinh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thuocTinh);
        }

        // GET: Admin/ThuocTinhs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThuocTinh thuocTinh = db.ThuocTinhs.Find(id);
            if (thuocTinh == null)
            {
                return HttpNotFound();
            }
            return View(thuocTinh);
        }

        // POST: Admin/ThuocTinhs/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var thuocTinh = db.ThuocTinhs.Find(id);

            if (thuocTinh == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thuộc tính sản phẩm." });
            }

            db.ThuocTinhs.Remove(thuocTinh);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa thuộc tính sản phẩm thành công." });
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
            var thuocTinhs = db.ThuocTinhs.Where(sp => idList.Contains(sp.ThuocTinhId)).ToList();

            if (!thuocTinhs.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy thuộc tính sản phẩm." });
            }

            db.ThuocTinhs.RemoveRange(thuocTinhs);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa thành công." });
        }
    }
}
