using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace DoAnChuyenNganh.Areas.Admin.Controllers
{
    public class TinTucsController : AdminBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/TinTucs
        public ActionResult Index(string tim, int? page)
        {
            var query = db.TinTucs.Include(s => s.HinhAnhTinTucs);
            if (!string.IsNullOrEmpty(tim))
            {
                query = query.Where(p => p.TieuDe.Contains(tim) || p.Slug.Contains(tim));
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;

            var items = query.OrderBy(x => x.TinTucId).ToPagedList(pageNumber, pageSize);  //OrderByDescending để hiện thị bản ghi có ID lớn trước OrderBy ngược lại
            return View(items);
        }

        // GET: Admin/TinTucs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tinTuc = db.TinTucs
                          .Include(d => d.HinhAnhTinTucs)
                          .FirstOrDefault(d => d.TinTucId == id);
            //TinTuc tinTuc = db.TinTucs.Find(id);
            if (tinTuc == null)
            {
                return HttpNotFound();
            }
            return View(tinTuc);
        }

        // GET: Admin/TinTucs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/TinTucs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "TinTucId,TieuDe,Slug,TomTat,NoiDung,NgayDang,TrangThai")] TinTuc tinTuc)
        {
            if (ModelState.IsValid)
            {
                tinTuc.NgayDang = DateTime.Now;
                tinTuc.Slug = DoAnChuyenNganh.Models.Common.Filter.FilterChar(tinTuc.TieuDe);
                db.TinTucs.Add(tinTuc);
                db.SaveChanges();
                return Json(new { success = true, message = "Thêm tin tức thành công" });
            }

            else
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lê." });
            }
        }

        // GET: Admin/TinTucs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tinTuc = db.TinTucs.Include("HinhAnhTinTucs").FirstOrDefault(sp => sp.TinTucId == id);
            if (tinTuc == null)
            {
                return HttpNotFound();
            }
            return View(tinTuc);
        }

        // POST: Admin/TinTucs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit([Bind(Include = "TinTucId,TieuDe,Slug,TomTat,NoiDung,NgayDang,TrangThai")] TinTuc tinTuc, HttpPostedFileBase UploadImage, string Url)
        {
            if (ModelState.IsValid)
            {

                tinTuc.Slug = DoAnChuyenNganh.Models.Common.Filter.FilterChar(tinTuc.TieuDe);
                db.Entry(tinTuc).State = System.Data.Entity.EntityState.Modified;
                // giữ nguyên ngày tạo
                db.Entry(tinTuc).Property(x => x.NgayDang).IsModified = false;

                db.SaveChanges();
                // Nếu có upload ảnh từ máy
                if (UploadImage != null && UploadImage.ContentLength > 0)
                {
                    string path = Path.Combine(Server.MapPath("~/Uploads/HinhAnhTinTuc"), Path.GetFileName(UploadImage.FileName));
                    UploadImage.SaveAs(path);

                    var img = new HinhAnhTinTuc
                    {
                        TinTucId = tinTuc.TinTucId,
                        Url = "/Uploads/HinhAnhTinTuc/" + UploadImage.FileName
                    };
                    db.HinhAnhTinTucs.Add(img);
                    db.SaveChanges();
                }

                // Nếu có chọn ảnh từ CKFinder
                if (!string.IsNullOrEmpty(Url))
                {
                    var img = new HinhAnhTinTuc
                    {
                        TinTucId = tinTuc.TinTucId,
                        Url = Url
                    };
                    db.HinhAnhTinTucs.Add(img);
                    db.SaveChanges();
                }
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            else
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lê!" });
            }
        }

        // GET: Admin/TinTucs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tinTuc = db.TinTucs
                        .Include(d => d.HinhAnhTinTucs)
                        .FirstOrDefault(d => d.TinTucId == id);
            if (tinTuc == null)
            {
                return HttpNotFound();
            }
            return View(tinTuc);
        }

        // POST: Admin/TinTucs/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            var tinTuc = db.TinTucs.Find(id);

            if (tinTuc == null)
            {
                return Json(new { success = false, message = "Không tìm thấy tin tức." });
            }

            db.TinTucs.Remove(tinTuc);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa tin tức thành công." });
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
            var tinTuc = db.TinTucs.Where(sp => idList.Contains(sp.TinTucId)).ToList();

            if (!tinTuc.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy tin tức." });
            }

            db.TinTucs.RemoveRange(tinTuc);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa thành công." });
        }
    }
}
