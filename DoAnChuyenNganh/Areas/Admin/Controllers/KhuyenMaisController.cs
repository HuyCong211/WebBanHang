using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using Microsoft.Ajax.Utilities;
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
    public class KhuyenMaisController : AdminBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/KhuyenMais
        public ActionResult Index(string tim, int? page)
        {
            var khuyenMai = db.KhuyenMais.Include(a => a.SanPhams);

            if (!string.IsNullOrEmpty(tim))
            {
                khuyenMai = khuyenMai.Where(p => p.MaKM.Contains(tim)|| p.TenKM.Contains(tim)||p.Loai.Contains(tim));
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;

            var items = khuyenMai.OrderBy(x => x.KhuyenMaiId).ToPagedList(pageNumber, pageSize);  //OrderByDescending để hiện thị bản ghi có ID lớn trước OrderBy ngược lại
            return View(items);
        }

        // GET: Admin/KhuyenMais/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var khuyenMai = db.KhuyenMais.Include(x => x.SanPhams)
                                    .FirstOrDefault(x => x.KhuyenMaiId == id);
            if (khuyenMai == null)
            {
                return HttpNotFound();
            }
            return View(khuyenMai);
        }

        // GET: Admin/KhuyenMais/Create
        public ActionResult Create()
        {
            var km = new KhuyenMai
            {
                NgayBatDau = DateTime.Now,
                NgayKetThuc = DateTime.Now
            };

            // gửi danh sách sản phẩm cho view
            ViewBag.SanPhams = db.SanPhams.ToList();
            return View(km);
        }

        // POST: Admin/KhuyenMais/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "KhuyenMaiId,MaKM,TenKM,Loai,GiaTri,NgayBatDau,NgayKetThuc,DieuKien,TrangThai")] KhuyenMai khuyenMai, int[] SanPhamIds)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng mã khuyến mại
                bool exists = db.KhuyenMais.Any(x => x.MaKM == khuyenMai.MaKM);
                if (exists)
                {
                    return Json(new { success = false, message = "Mã khuyến mại đã tồn tại, vui lòng nhập mã khác." });
                }
               

                db.KhuyenMais.Add(khuyenMai);
                db.SaveChanges();

                // gán sản phẩm vào khuyến mại
                if (SanPhamIds != null)
                {
                    foreach (var spId in SanPhamIds)
                    {
                        db.Database.ExecuteSqlCommand(
                            "INSERT INTO SanPhamKhuyenMai (SanPhamId, KhuyenMaiId) VALUES (@p0, @p1)",
                            spId, khuyenMai.KhuyenMaiId
                        );
                    }
                }
                return Json(new { success = true, message = "Thêm khuyến mại thành công" });
            }
            // gom lỗi validation từ ModelState
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .Where(m => !string.IsNullOrEmpty(m))
                                          .ToList();
           
            return Json(new { success = false, message = string.Join("<br/>", errors) });
            
        }
        

        // GET: Admin/KhuyenMais/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var khuyenMai = db.KhuyenMais
                     .Include(x => x.SanPhams)
                     .FirstOrDefault(x => x.KhuyenMaiId == id);
            if (khuyenMai == null)
            {
                return HttpNotFound();
            }
            // Lấy danh sách sản phẩm
            ViewBag.SanPhams = db.SanPhams.ToList();

            // Lấy danh sách Id sản phẩm đã gán khuyến mãi này
            ViewBag.SelectedSanPhams = khuyenMai.SanPhams.Select(x => x.SanPhamId).ToList();
            return View(khuyenMai);
        }

        // POST: Admin/KhuyenMais/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit([Bind(Include = "KhuyenMaiId,MaKM,TenKM,Loai,GiaTri,NgayBatDau,NgayKetThuc,DieuKien,TrangThai")] KhuyenMai khuyenMai, int[] SanPhamIds)
        {

            if (ModelState.IsValid)
            {
                db.Entry(khuyenMai).State = EntityState.Modified;

                // Xóa hết quan hệ cũ
                db.Database.ExecuteSqlCommand("DELETE FROM SanPhamKhuyenMai WHERE KhuyenMaiId = @p0", khuyenMai.KhuyenMaiId);

                // Thêm lại quan hệ mới
                if (SanPhamIds != null)
                {
                    foreach (var spId in SanPhamIds)
                    {
                        db.Database.ExecuteSqlCommand(
                            "INSERT INTO SanPhamKhuyenMai (SanPhamId, KhuyenMaiId) VALUES (@p0, @p1)",
                            spId, khuyenMai.KhuyenMaiId
                        );
                    }
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật khuyến mãi thành công" });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .Where(m => !string.IsNullOrEmpty(m))
                                          .ToList();

            return Json(new { success = false, message = string.Join("<br/>", errors) });
        }

        // GET: Admin/KhuyenMais/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var khuyenMai = db.KhuyenMais.Include(x => x.SanPhams)
                                    .FirstOrDefault(x => x.KhuyenMaiId == id);
            if (khuyenMai == null)
            {
                return HttpNotFound();
            }
            return View(khuyenMai);
        }

        // POST: Admin/KhuyenMais/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            var khuyenMai = db.KhuyenMais.Find(id);

            if (khuyenMai == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khuyến mại." });
            }

            db.KhuyenMais.Remove(khuyenMai);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa khuyến mại thành công." });
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


