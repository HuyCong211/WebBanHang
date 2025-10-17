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
    public class HinhAnhTinTucsController : AdminBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/HinhAnhTinTucs
        public ActionResult Index(string tim, int? page)
        {

            var anhTinTuc = db.HinhAnhTinTucs.Include(a => a.TinTuc);

            if (!string.IsNullOrEmpty(tim))
            {
                anhTinTuc = anhTinTuc.Where(p => p.TinTuc.TieuDe.Contains(tim));
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;

            var items = anhTinTuc.OrderBy(x => x.HinhAnhId).ToPagedList(pageNumber, pageSize);  //OrderByDescending để hiện thị bản ghi có ID lớn trước OrderBy ngược lại
            return View(items);
        }

        // GET: Admin/HinhAnhTinTucs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hinhAnhTinTuc = db.HinhAnhTinTucs.Include(d => d.TinTuc).FirstOrDefault(d => d.HinhAnhId == id);
            //HinhAnhTinTuc hinhAnhTinTuc = db.HinhAnhTinTucs.Find(id);
            if (hinhAnhTinTuc == null)
            {
                return HttpNotFound();
            }
            return View(hinhAnhTinTuc);
        }

        // GET: Admin/HinhAnhTinTucs/Create
        public ActionResult Create(int? tinTucId)
        {
            if (tinTucId.HasValue)
            {
                // Nếu có sanPhamId thì select đúng sản phẩm đó
                ViewBag.TinTucId = new SelectList(db.TinTucs, "TinTucId", "TieuDe", tinTucId);
            }
            else
            {
                // Nếu không thì cho chọn toàn bộ
                ViewBag.TinTucId = new SelectList(db.TinTucs, "TinTucId", "TieuDe");
            }

            return View();
        }

        // POST: Admin/HinhAnhTinTucs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HinhAnhId,TinTucId,Url")] HinhAnhTinTuc hinhAnhTinTuc, HttpPostedFileBase UploadImage)
        {
            if (UploadImage != null && UploadImage.ContentLength > 0)
            {
                string extension = Path.GetExtension(UploadImage.FileName);
                string fileName = Guid.NewGuid().ToString() + extension;
                string folderPath = Server.MapPath("~/Uploads/HinhAnhTinTuc");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string path = Path.Combine(folderPath, fileName);
                UploadImage.SaveAs(path);

                // Gán lại Url từ ảnh upload
                hinhAnhTinTuc.Url = "/Uploads/HinhAnhTinTuc/" + fileName;

                // Xóa lỗi mặc định của Url (do [Required])
                ModelState["Url"].Errors.Clear();
            }

            if (ModelState.IsValid)
            {
                db.HinhAnhTinTucs.Add(hinhAnhTinTuc);
                var tinTuc = db.TinTucs.Find(hinhAnhTinTuc.TinTucId);

                db.SaveChanges();
                return RedirectToAction("Index", "TinTucs");
            }

            ViewBag.TinTucId = new SelectList(db.TinTucs, "TinTucId", "TieuDe", hinhAnhTinTuc.TinTucId);
            return View(hinhAnhTinTuc);
        }

        // GET: Admin/HinhAnhTinTucs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HinhAnhTinTuc hinhAnhTinTuc = db.HinhAnhTinTucs.Find(id);
            if (hinhAnhTinTuc == null)
            {
                return HttpNotFound();
            }
            ViewBag.TinTucId = new SelectList(db.TinTucs, "TinTucId", "TieuDe", hinhAnhTinTuc.TinTucId);
            return View(hinhAnhTinTuc);
        }

        // POST: Admin/HinhAnhTinTucs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HinhAnhId,TinTucId,Url")] HinhAnhTinTuc hinhAnhTinTuc, HttpPostedFileBase UploadImage)
        {
            if (UploadImage != null && UploadImage.ContentLength > 0)
            {
                string extension = Path.GetExtension(UploadImage.FileName);
                string fileName = Guid.NewGuid().ToString() + extension;
                string folderPath = Server.MapPath("~/Uploads/HinhAnhTinTuc");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string path = Path.Combine(folderPath, fileName);
                UploadImage.SaveAs(path);

                // Gán lại Url từ ảnh upload
                hinhAnhTinTuc.Url = "/Uploads/HinhAnhTinTuc/" + fileName;

                // Xóa lỗi mặc định của Url (do [Required])
                ModelState["Url"].Errors.Clear();
            }
            if (ModelState.IsValid)
            {
                db.Entry(hinhAnhTinTuc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TinTucId = new SelectList(db.TinTucs, "TinTucId", "TieuDe", hinhAnhTinTuc.TinTucId);
            return View(hinhAnhTinTuc);


        }

        // GET: Admin/HinhAnhTinTucs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hinhAnhTinTuc = db.HinhAnhTinTucs.Include(d => d.TinTuc).FirstOrDefault(d => d.HinhAnhId == id);
            //HinhAnhTinTuc hinhAnhTinTuc = db.HinhAnhTinTucs.Find(id);
            if (hinhAnhTinTuc == null)
            {
                return HttpNotFound();
            }
            return View(hinhAnhTinTuc);
        }

        // POST: Admin/HinhAnhTinTucs/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            var hinhAnhTinTuc = db.HinhAnhTinTucs.Find(id);

            if (hinhAnhTinTuc == null)
            {
                return Json(new { success = false, message = "Không tìm thấy ảnh tin tức." });
            }

            db.HinhAnhTinTucs.Remove(hinhAnhTinTuc);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa ảnh tin tức thành công." });

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
            var anhTinTuc = db.HinhAnhTinTucs.Where(sp => idList.Contains(sp.HinhAnhId)).ToList();

            if (!anhTinTuc.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy ảnh tin tức." });
            }

            db.HinhAnhTinTucs.RemoveRange(anhTinTuc);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa thành công." });
        }
    }
}
