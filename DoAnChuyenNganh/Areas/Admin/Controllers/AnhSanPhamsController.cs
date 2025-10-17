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

namespace DoAnChuyenNganh.Areas.Admin.Controllers
{
    public class AnhSanPhamsController : AdminBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/AnhSanPhams
        public ActionResult Index(string tim, int? page)
        {
            var anhSanPhams = db.AnhSanPhams.Include(a => a.SanPham)
                                            .Include(a => a.AnhSanPham_BienThes.Select(b => b.BienTheSanPham));

            if (!string.IsNullOrEmpty(tim))
            {
                anhSanPhams = anhSanPhams.Where(p => p.SanPham.MaSanPham.Contains(tim));
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;

            var items = anhSanPhams.OrderByDescending(x => x.AnhSanPhamId).ToPagedList(pageNumber, pageSize);  //OrderByDescending để hiện thị bản ghi có ID lớn trước OrderBy ngược lại
            return View(items);
        }

        // GET: Admin/AnhSanPhams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var anhSanPham = db.AnhSanPhams.Include(d => d.SanPham)
                                           .Include(a => a.AnhSanPham_BienThes.Select(b => b.BienTheSanPham))
                                           .FirstOrDefault(d => d.AnhSanPhamId == id);
            if (anhSanPham == null)
            {
                return HttpNotFound();
            }
            return View(anhSanPham);
        }

        // GET: Admin/AnhSanPhams/Create
        public ActionResult Create(int? sanPhamId)
        {
            if (sanPhamId.HasValue)
            {
                // Nếu có sanPhamId thì select đúng sản phẩm đó
                ViewBag.SanPhamId = new SelectList(db.SanPhams, "SanPhamId", "MaSanPham", sanPhamId);
            }
            else
            {
                // Nếu không thì cho chọn toàn bộ
                ViewBag.SanPhamId = new SelectList(db.SanPhams, "SanPhamId", "MaSanPham");
            }

            ViewBag.BienThes = db.BienTheSanPhams.Where(x => x.SanPhamId == sanPhamId).ToList();
            return View(); 
        }

        // POST: Admin/AnhSanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AnhSanPhamId,SanPhamId,BienTheId,Url,ThuTu,MacDinh,MoTa")] AnhSanPham anhSanPham, HttpPostedFileBase UploadImage, int[] SelectedBienTheIds)
        {
            if (UploadImage != null && UploadImage.ContentLength > 0)
            {
                string extension = Path.GetExtension(UploadImage.FileName);
                string fileName = Guid.NewGuid().ToString() + extension;
                string folderPath = Server.MapPath("~/Uploads/AnhSanPham");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string path = Path.Combine(folderPath, fileName);
                UploadImage.SaveAs(path);

                // Gán lại Url từ ảnh upload
                anhSanPham.Url = "/Uploads/AnhSanPham/" + fileName;

                // Xóa lỗi mặc định của Url (do [Required])
                ModelState["Url"].Errors.Clear();
            }

            if (ModelState.IsValid)
            {
                db.AnhSanPhams.Add(anhSanPham);
                var sanPham = db.SanPhams.Find(anhSanPham.SanPhamId);
                if (sanPham != null)
                {
                    sanPham.NgayCapNhat = DateTime.Now;
                    db.Entry(sanPham).State = EntityState.Modified;
                }
                // Thêm quan hệ nhiều biến thể
                if (SelectedBienTheIds != null && SelectedBienTheIds.Any())
                {
                    foreach (var bienTheId in SelectedBienTheIds)
                    {
                        db.AnhSanPham_BienThe.Add(new AnhSanPham_BienThe
                        {
                            AnhSanPhamId = anhSanPham.AnhSanPhamId,
                            BienTheId = bienTheId
                        });
                    }
                    db.SaveChanges();
                }
                db.SaveChanges();
                return RedirectToAction("Index", "SanPhams");
            }

            ViewBag.SanPhamId = new SelectList(db.SanPhams, "SanPhamId", "MaSanPham", anhSanPham.SanPhamId);
            return View(anhSanPham);
        }

        // GET: Admin/AnhSanPhams/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Load kèm các liên kết tới biến thể
            var anhSanPham = db.AnhSanPhams
                .Include(a => a.AnhSanPham_BienThes)
                .FirstOrDefault(a => a.AnhSanPhamId == id);
            //AnhSanPham anhSanPham = db.AnhSanPhams.Find(id);
            if (anhSanPham == null)
            {
                return HttpNotFound();
            }
            ViewBag.SanPhamId = new SelectList(db.SanPhams, "SanPhamId", "MaSanPham", anhSanPham.SanPhamId);
            // Load biến thể của sản phẩm đó
            ViewBag.BienThes = db.BienTheSanPhams
                .Where(x => x.SanPhamId == anhSanPham.SanPhamId)
                .ToList();

            return View(anhSanPham);
        }

        // POST: Admin/AnhSanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AnhSanPhamId,SanPhamId,BienTheId,Url,ThuTu,MacDinh,MoTa")] AnhSanPham anhSanPham, HttpPostedFileBase UploadImage, int[] SelectedBienTheIds)
        {
            if (UploadImage != null && UploadImage.ContentLength > 0)
            {
                string extension = Path.GetExtension(UploadImage.FileName);
                string fileName = Guid.NewGuid().ToString() + extension;
                string folderPath = Server.MapPath("~/Uploads/AnhSanPham");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string path = Path.Combine(folderPath, fileName);
                UploadImage.SaveAs(path);

                // Gán lại Url từ ảnh upload
                anhSanPham.Url = "/Uploads/AnhSanPham/" + fileName;

                // Xóa lỗi mặc định của Url (do [Required])
                ModelState["Url"].Errors.Clear();
            }
            if (ModelState.IsValid)
            {
                db.Entry(anhSanPham).State = EntityState.Modified;
                // Cập nhật ngày sửa cho sản phẩm liên quan
                var sanPham = db.SanPhams.Find(anhSanPham.SanPhamId);
                if (sanPham != null)
                {
                    sanPham.NgayCapNhat = DateTime.Now;
                    db.Entry(sanPham).State = EntityState.Modified;
                }
                // Xóa liên kết cũ
                var oldLinks = db.AnhSanPham_BienThe.Where(x => x.AnhSanPhamId == anhSanPham.AnhSanPhamId);
                db.AnhSanPham_BienThe.RemoveRange(oldLinks);

                // Thêm liên kết mới
                if (SelectedBienTheIds != null && SelectedBienTheIds.Any())
                {
                    foreach (var bienTheId in SelectedBienTheIds)
                    {
                        db.AnhSanPham_BienThe.Add(new AnhSanPham_BienThe
                        {
                            AnhSanPhamId = anhSanPham.AnhSanPhamId,
                            BienTheId = bienTheId
                        });
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SanPhamId = new SelectList(db.SanPhams, "SanPhamId", "MaSanPham", anhSanPham.SanPhamId);
            return View(anhSanPham);
        }

        // GET: Admin/AnhSanPhams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var anhSanPham = db.AnhSanPhams.Include(d => d.SanPham)
                .Include(a => a.AnhSanPham_BienThes.Select(b => b.BienTheSanPham))
                .FirstOrDefault(d => d.AnhSanPhamId == id);
            if (anhSanPham == null)
            {
                return HttpNotFound();
            }
            return View(anhSanPham);
        }

        // POST: Admin/AnhSanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            var anhSanPham = db.AnhSanPhams.Find(id);

            if (anhSanPham == null)
            {
                return Json(new { success = false, message = "Không tìm thấy ảnh sản phẩm." });
            }

            db.AnhSanPhams.Remove(anhSanPham);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa ảnh sản phẩm thành công." });
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
            var anhSanPhams = db.AnhSanPhams.Where(sp => idList.Contains(sp.AnhSanPhamId)).ToList();

            if (!anhSanPhams.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
            }

            db.AnhSanPhams.RemoveRange(anhSanPhams);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa thành công." });
        }
    }
}
