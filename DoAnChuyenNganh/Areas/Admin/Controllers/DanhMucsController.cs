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
    public class DanhMucsController : AdminBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/DanhMucs
        public ActionResult Index(string tim, int? page)
        {
            var query = db.DanhMucs.Include(d => d.DanhMuc2);
            if (!string.IsNullOrEmpty(tim))
            {
                query = query.Where(p => p.TenDanhMuc.Contains(tim) || p.Slug.Contains(tim) || p.DanhMuc2.TenDanhMuc.Contains(tim));
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;

            var items = query.OrderBy(x => x.DanhMucId).ToPagedList(pageNumber, pageSize);  //OrderByDescending để hiện thị bản ghi có ID lớn trước OrderBy ngược lại
            return View(items);
        }

        // GET: Admin/DanhMucs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var danhMuc = db.DanhMucs
                            .Include(d => d.DanhMuc2) // load luôn danh mục cha
                            .FirstOrDefault(d => d.DanhMucId == id);
            if (danhMuc == null)
            {
                return HttpNotFound();
            }
            return View(danhMuc);
        }

        // GET: Admin/DanhMucs/Create
        public ActionResult Create()
        {
            ViewBag.DanhMucChaId = new SelectList(db.DanhMucs, "DanhMucId", "TenDanhMuc");
            return View();
        }

        // POST: Admin/DanhMucs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "DanhMucId,TenDanhMuc,Slug,MoTa,DanhMucChaId,TrangThai,NgayTao,NgayCapNhat")] DanhMuc danhMuc)
        {
            if (ModelState.IsValid)
            {
                danhMuc.NgayTao = DateTime.Now;
                danhMuc.Slug = DoAnChuyenNganh.Models.Common.Filter.FilterChar(danhMuc.TenDanhMuc);
                db.DanhMucs.Add(danhMuc);
                db.SaveChanges();
                return Json(new { success = true, message = "Thêm danh mục thành công" });
            }
            else
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lê." });
            }
        }

        // GET: Admin/DanhMucs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DanhMuc danhMuc = db.DanhMucs.Find(id);
            if (danhMuc == null)
            {
                return HttpNotFound();
            }
            ViewBag.DanhMucChaId = new SelectList(db.DanhMucs, "DanhMucId", "TenDanhMuc", danhMuc.DanhMucChaId);
            return View(danhMuc);
        }

        // POST: Admin/DanhMucs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit([Bind(Include = "DanhMucId,TenDanhMuc,Slug,MoTa,DanhMucChaId,TrangThai,NgayTao,NgayCapNhat")] DanhMuc danhMuc)
        {
            if (ModelState.IsValid)
            {
                danhMuc.NgayCapNhat = DateTime.Now;
                danhMuc.Slug = DoAnChuyenNganh.Models.Common.Filter.FilterChar(danhMuc.TenDanhMuc);
                db.Entry(danhMuc).State = System.Data.Entity.EntityState.Modified;
                // giữ nguyên ngày tạo
                db.Entry(danhMuc).Property(x => x.NgayTao).IsModified = false;

                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            else
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lê!" });
            }
        }

        // GET: Admin/DanhMucs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var danhMuc = db.DanhMucs
                          .Include(d => d.DanhMuc2) // load luôn danh mục cha
                          .FirstOrDefault(d => d.DanhMucId == id);
            if (danhMuc == null)
            {
                return HttpNotFound();
            }
            // Lấy danh sách con
            var children = db.DanhMucs.Where(dm => dm.DanhMucChaId == id).ToList();

            if (children.Any())
            {
                ViewBag.Children = children;   // Truyền danh sách con qua View
                return View("DeleteConfirmWithChildren", danhMuc);
            }

            return View("Delete", danhMuc); // view bình thường
        }

        // POST: Admin/DanhMucs/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, bool? forceDelete)
        {
            var danhMuc = db.DanhMucs.Find(id);
            if (danhMuc == null)
            {
                return Json(new { success = false, message = "Không tìm thấy danh mục." });
            }

            // Lấy danh sách con trực tiếp của danh mục
            var children = db.DanhMucs.Where(dm => dm.DanhMucChaId == id).ToList();

            // Nếu có mục con và chưa xác nhận forceDelete
            if (children.Any() && forceDelete != true)
            {
                var childNames = children.Select(c => c.TenDanhMuc).ToList();

                return Json(new
                {
                    success = false,
                    hasChild = true,
                    message = $"Danh mục '{danhMuc.TenDanhMuc}' có chứa các mục con: {childNames}"
                            + ". Bạn có chắc chắn muốn xóa toàn bộ không?"

                });
            }

            // Nếu người dùng đồng ý xóa toàn bộ
            if (children.Any() && forceDelete == true)
            {
                db.DanhMucs.RemoveRange(children);
            }

            // Nếu không có mục con thì vẫn xóa bình thường
            db.DanhMucs.Remove(danhMuc);
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
        public JsonResult DeleteAll(string ids, bool? forceDelete)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return Json(new { success = false, message = "Không có bản ghi nào được chọn." });
            }

            var idList = ids.Split(',').Select(x => Convert.ToInt32(x)).ToList();
            var danhMucs = db.DanhMucs.Where(dm => idList.Contains(dm.DanhMucId)).ToList();

            if (!danhMucs.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy danh mục." });
            }

            // Tìm danh mục có con
            var hasChild = new List<string>();
            foreach (var dm in danhMucs)
            {
                var children = db.DanhMucs.Where(c => c.DanhMucChaId == dm.DanhMucId).ToList();
                if (children.Any())
                {
                    hasChild.Add($"- Danh mục {dm.TenDanhMuc} có chứa mục con: {string.Join(", ", children.Select(c => c.TenDanhMuc))}");
                }
            }

            // Nếu có mục cha chứa con mà chưa forceDelete -> hỏi lại
            if (hasChild.Any() && forceDelete != true)
            {
                return Json(new
                {
                    success = false,
                    hasChild = true,
                    message = string.Join("\n", hasChild) + "\n\nBạn có chắc chắn muốn xóa toàn bộ không?"
                });
            }

            // Nếu người dùng đồng ý xóa hết (forceDelete = true)
            foreach (var dm in danhMucs)
            {
                var children = db.DanhMucs.Where(c => c.DanhMucChaId == dm.DanhMucId).ToList();
                if (children.Any())
                {
                    db.DanhMucs.RemoveRange(children);
                }
                db.DanhMucs.Remove(dm);
            }
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa thành công." });
        }
    }
}
