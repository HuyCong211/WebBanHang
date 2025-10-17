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
    public class PhuongThucThanhToansController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/PhuongThucThanhToans
        public ActionResult Index(string tim, int? page)
        {
            var phuongThucThanhToan = db.PhuongThucThanhToans.AsQueryable();

            // Tìm kiếm theo tên thuộc tính
            if (!string.IsNullOrEmpty(tim))
            {
                phuongThucThanhToan = phuongThucThanhToan.Where(x => x.TenPhuongThuc.Contains(tim));
            }

            // Kích thước trang
            int pageSize = 10;
            int pageNumber = page ?? 1;

            // Lưu giá trị tìm kiếm để hiện lại trên view
            ViewBag.CurrentFilter = tim;

            return View(phuongThucThanhToan.OrderBy(x => x.PhuongThucId).ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/PhuongThucThanhToans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhuongThucThanhToan phuongThucThanhToan = db.PhuongThucThanhToans.Find(id);
            if (phuongThucThanhToan == null)
            {
                return HttpNotFound();
            }
            return View(phuongThucThanhToan);
        }

        // GET: Admin/PhuongThucThanhToans/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/PhuongThucThanhToans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PhuongThucId,TenPhuongThuc,MoTa,Active")] PhuongThucThanhToan phuongThucThanhToan)
        {
            if (ModelState.IsValid)
            {
                db.PhuongThucThanhToans.Add(phuongThucThanhToan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(phuongThucThanhToan);
        }

        // GET: Admin/PhuongThucThanhToans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhuongThucThanhToan phuongThucThanhToan = db.PhuongThucThanhToans.Find(id);
            if (phuongThucThanhToan == null)
            {
                return HttpNotFound();
            }
            return View(phuongThucThanhToan);
        }

        // POST: Admin/PhuongThucThanhToans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PhuongThucId,TenPhuongThuc,MoTa,Active")] PhuongThucThanhToan phuongThucThanhToan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phuongThucThanhToan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(phuongThucThanhToan);
        }

        // GET: Admin/PhuongThucThanhToans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhuongThucThanhToan phuongThucThanhToan = db.PhuongThucThanhToans.Find(id);
            if (phuongThucThanhToan == null)
            {
                return HttpNotFound();
            }
            return View(phuongThucThanhToan);
        }

        // POST: Admin/PhuongThucThanhToans/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var phuongThucThanhToan = db.PhuongThucThanhToans.Find(id);

            if (phuongThucThanhToan == null)
            {
                return Json(new { success = false, message = "Không tìm thấy phương thức thanh toán này." });
            }

            db.PhuongThucThanhToans.Remove(phuongThucThanhToan);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa phương thức thanh toán thành công." });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
