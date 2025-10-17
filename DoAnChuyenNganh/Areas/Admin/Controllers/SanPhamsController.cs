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
    public class SanPhamsController : AdminBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/SanPhams
        public ActionResult Index(string tim, int? page)
        {
            ViewBag.CurrentFilter = tim;
            var query = db.SanPhams.Include(s => s.DanhMuc).Include(s => s.AnhSanPhams);
            if (!string.IsNullOrEmpty(tim))
            {
                query = query.Where(p => p.TenSanPham.Contains(tim) || p.Slug.Contains(tim) || p.DanhMuc.TenDanhMuc.Contains(tim));
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;

            var items = query.OrderByDescending(x => x.SanPhamId).ToPagedList(pageNumber, pageSize);  //OrderByDescending để hiện thị bản ghi có ID lớn trước OrderBy ngược lại
            return View(items);
        }

        // GET: Admin/SanPhams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sanPham = db.SanPhams
                          .Include(d => d.DanhMuc) // load luôn danh mục cha
                          .Include(d => d.AnhSanPhams)
                          .FirstOrDefault(d => d.SanPhamId == id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            return View(sanPham);
        }

        // GET: Admin/SanPhams/Create
        public ActionResult Create()
        {
            ViewBag.DanhMucId = new SelectList(db.DanhMucs, "DanhMucId", "TenDanhMuc");
            return View();
        }

        // POST: Admin/SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "SanPhamId,MaSanPham,TenSanPham,Slug,MoTaNgan,MoTaChiTiet,GiaBan,GiaGoc,DanhMucId,TrangThai,LuotXem,NgayTao,NgayCapNhat")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng mã sản phẩm
                bool exists = db.SanPhams.Any(x => x.MaSanPham == sanPham.MaSanPham);
                if (exists)
                {
                    return Json(new { success = false, message = "Mã sản phẩm đã tồn tại, vui lòng nhập mã khác." });
                }

                sanPham.NgayTao = DateTime.Now;
                sanPham.Slug = DoAnChuyenNganh.Models.Common.Filter.FilterChar(sanPham.TenSanPham);
                db.SanPhams.Add(sanPham);
                db.SaveChanges();
                return Json(new { success = true, message = "Thêm sản phẩm thành công" });
            }
            else
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lê." });
            }
        }

        // GET: Admin/SanPhams/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sanPham = db.SanPhams.Include("AnhSanPhams").FirstOrDefault(sp => sp.SanPhamId == id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            ViewBag.DanhMucId = new SelectList(db.DanhMucs, "DanhMucId", "TenDanhMuc", sanPham.DanhMucId);
            return View(sanPham);
        }

        // POST: Admin/SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit([Bind(Include = "SanPhamId,MaSanPham,TenSanPham,Slug,MoTaNgan,MoTaChiTiet,GiaBan,GiaGoc,DanhMucId,TrangThai,LuotXem,NgayTao,NgayCapNhat")] SanPham sanPham, HttpPostedFileBase UploadImage, string Url)
        {
            if (ModelState.IsValid)
            {
                sanPham.NgayCapNhat = DateTime.Now;
                sanPham.Slug = DoAnChuyenNganh.Models.Common.Filter.FilterChar(sanPham.TenSanPham);
                db.Entry(sanPham).State = System.Data.Entity.EntityState.Modified;
                // giữ nguyên ngày tạo
                db.Entry(sanPham).Property(x => x.NgayTao).IsModified = false;
                // giữ nguyên Mã sản phẩm
                db.Entry(sanPham).Property(x => x.MaSanPham).IsModified = false;

                db.SaveChanges();
                // Nếu có upload ảnh từ máy
                if (UploadImage != null && UploadImage.ContentLength > 0)
                {
                    string path = Path.Combine(Server.MapPath("~/Uploads/AnhSanPham"), Path.GetFileName(UploadImage.FileName));
                    UploadImage.SaveAs(path);

                    var img = new AnhSanPham
                    {
                        SanPhamId = sanPham.SanPhamId,
                        Url = "/Uploads/AnhSanPham/" + UploadImage.FileName
                    };
                    db.AnhSanPhams.Add(img);
                    db.SaveChanges();
                }

                // Nếu có chọn ảnh từ CKFinder
                if (!string.IsNullOrEmpty(Url))
                {
                    var img = new AnhSanPham
                    {
                        SanPhamId = sanPham.SanPhamId,
                        Url = Url
                    };
                    db.AnhSanPhams.Add(img);
                    db.SaveChanges();
                }
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            else
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lê!" });
            }
        }

        // GET: Admin/SanPhams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sanPham = db.SanPhams
                         .Include(d => d.DanhMuc) // load luôn danh mục cha
                         .Include(d => d.AnhSanPhams)
                         .FirstOrDefault(d => d.SanPhamId == id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            return View(sanPham);
        }

        // POST: Admin/SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            var sanPham = db.SanPhams
                            .Include(sp => sp.AnhSanPhams)
                            .Include(sp => sp.BienTheSanPhams.Select(bt => bt.AnhSanPham_BienThes))
                            .Include(sp => sp.BienTheSanPhams.Select(bt => bt.LichSuGias))
                            .Include(sp => sp.BienTheSanPhams.Select(bt => bt.LichSuTonKhoes))
                            .Include(sp => sp.BienTheSanPhams.Select(bt => bt.TonKhoes))
                            .Include(sp => sp.BienTheSanPhams.Select(bt => bt.DonHangChiTiets))
                            .Include(sp => sp.BinhLuans)
                            .FirstOrDefault(sp => sp.SanPhamId == id);

            if (sanPham == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
            }

            // Xóa biến thể
            db.BienTheSanPhams.RemoveRange(sanPham.BienTheSanPhams);

            // Xóa bình luận
            db.BinhLuans.RemoveRange(sanPham.BinhLuans);

            // Xóa ảnh sản phẩm
            foreach (var img in sanPham.AnhSanPhams.ToList())
            {
                var path = Server.MapPath(img.Url);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                db.AnhSanPhams.Remove(img);
            }

            db.SanPhams.Remove(sanPham);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa sản phẩm thành công." });
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
            var sanPhams = db.SanPhams.Where(sp => idList.Contains(sp.SanPhamId)).ToList();

            if (!sanPhams.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
            }

            db.SanPhams.RemoveRange(sanPhams);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa thành công." });
        }
    }
}
