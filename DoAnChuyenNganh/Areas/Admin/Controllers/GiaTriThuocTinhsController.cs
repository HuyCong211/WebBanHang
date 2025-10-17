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
    public class GiaTriThuocTinhsController : AdminBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/GiaTriThuocTinhs
        public ActionResult Index(string tim, int? page)
        {
            var giaTriThuocTinhs = db.GiaTriThuocTinhs.Include(g => g.ThuocTinh);

            if (!string.IsNullOrEmpty(tim))
            {
                giaTriThuocTinhs = giaTriThuocTinhs.Where(p => p.TenGiaTri.Contains(tim));
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;

            var items = giaTriThuocTinhs.OrderByDescending(x => x.GiaTriId).ToPagedList(pageNumber, pageSize);  //OrderByDescending để hiện thị bản ghi có ID lớn trước OrderBy ngược lại
            return View(items);
        }

        // GET: Admin/GiaTriThuocTinhs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiaTriThuocTinh giaTriThuocTinh = db.GiaTriThuocTinhs.Find(id);
            if (giaTriThuocTinh == null)
            {
                return HttpNotFound();
            }
            return View(giaTriThuocTinh);
        }

        // GET: Admin/GiaTriThuocTinhs/Create
        public ActionResult Create()
        {
            ViewBag.ThuocTinhId = new SelectList(db.ThuocTinhs, "ThuocTinhId", "TenThuocTinh");
            return View();
        }

        // POST: Admin/GiaTriThuocTinhs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "GiaTriId,ThuocTinhId,TenGiaTri")] GiaTriThuocTinh giaTriThuocTinh)
        {
            if (ModelState.IsValid)
            {
                
                db.GiaTriThuocTinhs.Add(giaTriThuocTinh);
                db.SaveChanges();
                return Json(new { success = true, message = "Thêm giá trị thuộc tính thành công" });
            }
            else
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lê." });
            }
        }

        // GET: Admin/GiaTriThuocTinhs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiaTriThuocTinh giaTriThuocTinh = db.GiaTriThuocTinhs.Find(id);
            if (giaTriThuocTinh == null)
            {
                return HttpNotFound();
            }
            ViewBag.ThuocTinhId = new SelectList(db.ThuocTinhs, "ThuocTinhId", "TenThuocTinh", giaTriThuocTinh.ThuocTinhId);
            return View(giaTriThuocTinh);
        }

        // POST: Admin/GiaTriThuocTinhs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit([Bind(Include = "GiaTriId,ThuocTinhId,TenGiaTri")] GiaTriThuocTinh giaTriThuocTinh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(giaTriThuocTinh).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            else
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lê!" });
            }
        }

        // GET: Admin/GiaTriThuocTinhs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiaTriThuocTinh giaTriThuocTinh = db.GiaTriThuocTinhs.Find(id);
            if (giaTriThuocTinh == null)
            {
                return HttpNotFound();
            }
            return View(giaTriThuocTinh);
        }

        // POST: Admin/GiaTriThuocTinhs/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var giaTriThuocTinh = db.GiaTriThuocTinhs.Find(id);

            if (giaTriThuocTinh == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thuộc tính sản phẩm." });
            }

            db.GiaTriThuocTinhs.Remove(giaTriThuocTinh);
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
            var giaTriThuocTinh = db.GiaTriThuocTinhs.Where(sp => idList.Contains(sp.GiaTriId)).ToList();

            if (!giaTriThuocTinh.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy giá trị thuộc tính sản phẩm." });
            }

            db.GiaTriThuocTinhs.RemoveRange(giaTriThuocTinh);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa thành công." });
        }
    }
}
