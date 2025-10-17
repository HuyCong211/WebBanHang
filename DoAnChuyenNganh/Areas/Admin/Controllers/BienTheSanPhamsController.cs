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
    public class BienTheSanPhamsController : AdminBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/BienTheSanPhams
        public ActionResult Index(string tim, int? page)
        {
            var bienTheSanPhams = db.BienTheSanPhams.Include(a => a.SanPham).Include(a=>a.GiaTriThuocTinhs.Select(g=>g.ThuocTinh));

            if (!string.IsNullOrEmpty(tim))
            {
                bienTheSanPhams = bienTheSanPhams.Where(p => p.SKU.Contains(tim)||p.SanPham.MaSanPham.Contains(tim));
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;

            var items = bienTheSanPhams.OrderByDescending(x => x.BienTheId).ToPagedList(pageNumber, pageSize);  //OrderByDescending để hiện thị bản ghi có ID lớn trước OrderBy ngược lại
            return View(items);
        }

        // GET: Admin/BienTheSanPhams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var bienTheSanPham = db.BienTheSanPhams.Include(x => x.SanPham)
                                    .Include(x => x.GiaTriThuocTinhs.Select(g => g.ThuocTinh))
                                    .FirstOrDefault(x => x.BienTheId == id);
            if (bienTheSanPham == null)
            {
                return HttpNotFound();
            }
            return View(bienTheSanPham);
        }

        // GET: Admin/BienTheSanPhams/Create
        public ActionResult Create()
        {

            ViewBag.SanPhamIds = new MultiSelectList(db.SanPhams, "SanPhamId", "MaSanPham");  //để thêm 1 thì dùng new SelectList; them nhiều thì dùng new MultiSelectList
            // load danh sách giá trị thuộc tính
            var allGiaTriThuocTinhs = db.GiaTriThuocTinhs.Include(g => g.ThuocTinh).ToList();
            // Truyền vào ViewBag
            ViewBag.giaTriThuocTinhIds = new MultiSelectList(allGiaTriThuocTinhs, "GiaTriId", "TenGiaTri");
            // load danh sách thuộc tính
            ViewBag.ThuocTinhIds = new MultiSelectList(db.ThuocTinhs, "ThuocTinhId", "TenThuocTinh");


            return View();
        }

        // POST: Admin/BienTheSanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "BienTheId,SanPhamId,SKU,Gia,GiaKhuyenMai,MaVach,TrangThai,NgayTao")] BienTheSanPham bienTheSanPham, int[] giaTriThuocTinhIds, int[] sanPhamIds)
        {
            if (sanPhamIds == null || sanPhamIds.Length == 0)
                return Json(new { success = false, message = "Vui lòng chọn ít nhất một sản phẩm áp dụng." });

            if (ModelState.IsValid)
            {
                // Kiểm tra trùng SKU
                bool exists = db.BienTheSanPhams.Any(x => x.SKU == bienTheSanPham.SKU);
                if (exists)
                    return Json(new { success = false, message = "Mã biến thể sản phẩm đã tồn tại, vui lòng nhập mã khác." });

                bienTheSanPham.NgayTao = DateTime.Now;

                // nếu có giá trị thuộc tính
                if (giaTriThuocTinhIds != null && giaTriThuocTinhIds.Any())
                {
                    foreach (var id in giaTriThuocTinhIds)
                    {
                        var giaTri = db.GiaTriThuocTinhs.Find(id);
                        if (giaTri != null)
                            bienTheSanPham.GiaTriThuocTinhs.Add(giaTri);
                    }
                }

                // Lặp qua từng sản phẩm được chọn để thêm biến thể
                foreach (var spId in sanPhamIds)
                {
                    var sp = db.SanPhams.Find(spId);
                    if (sp != null)
                    {
                        var newBienThe = new BienTheSanPham
                        {
                            SanPhamId = spId,
                            SKU = bienTheSanPham.SKU,
                            // Nếu người dùng không nhập giá → lấy giá bán riêng của sản phẩm đó
                            Gia = (bienTheSanPham.Gia == null || bienTheSanPham.Gia == 0)? sp.GiaBan: bienTheSanPham.Gia,
                            //Gia = bienTheSanPham.Gia,
                            GiaKhuyenMai = bienTheSanPham.GiaKhuyenMai,
                            MaVach = bienTheSanPham.MaVach,
                            TrangThai = bienTheSanPham.TrangThai,
                            NgayTao = DateTime.Now,
                            GiaTriThuocTinhs = new List<GiaTriThuocTinh>()
                        };

                        // gán giá trị thuộc tính
                        if (giaTriThuocTinhIds != null)
                        {
                            foreach (var id in giaTriThuocTinhIds)
                            {
                                var gt = db.GiaTriThuocTinhs.Find(id);
                                if (gt != null)
                                    newBienThe.GiaTriThuocTinhs.Add(gt);
                            }
                        }

                        db.BienTheSanPhams.Add(newBienThe);
                    }
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Thêm biến thể áp dụng cho nhiều sản phẩm thành công!" });
            }

            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }

        // GET: Admin/BienTheSanPhams/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var bienTheSanPham = db.BienTheSanPhams
                                   .Include(x => x.GiaTriThuocTinhs.Select(g => g.ThuocTinh))
                                   .FirstOrDefault(x => x.BienTheId == id);
            if (bienTheSanPham == null)
            {
                return HttpNotFound();
            }


            // 🔹 Tìm tất cả sản phẩm đang dùng chung SKU này (nghĩa là cùng biến thể)
            var sanPhamsDaApDung = db.BienTheSanPhams
                .Where(x => x.SKU == bienTheSanPham.SKU)
                .Select(x => x.SanPhamId)
                .ToList();

            // SanPham
            ViewBag.SanPhamIds = new MultiSelectList(db.SanPhams, "SanPhamId", "MaSanPham", sanPhamsDaApDung);

            // Lấy danh sách thuộc tính (đa chọn)
            var selectedThuocTinhIds = bienTheSanPham.GiaTriThuocTinhs.Select(g => g.ThuocTinhId).Distinct().ToList();
            ViewBag.ThuocTinhIds = new MultiSelectList(db.ThuocTinhs, "ThuocTinhId", "TenThuocTinh", selectedThuocTinhIds);

            // Lấy danh sách giá trị thuộc tính (đa chọn)
            var selectedGiaTriIds = bienTheSanPham.GiaTriThuocTinhs.Select(g => g.GiaTriId).ToList();
            var allGiaTriThuocTinhs = db.GiaTriThuocTinhs.Include(g => g.ThuocTinh).ToList();
            ViewBag.giaTriThuocTinhIds = new MultiSelectList(allGiaTriThuocTinhs, "GiaTriId", "TenGiaTri", selectedGiaTriIds);

            return View(bienTheSanPham);

        }

        // POST: Admin/BienTheSanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit([Bind(Include = "BienTheId,SanPhamId,SKU,Gia,GiaKhuyenMai,MaVach,TrangThai,NgayTao")] BienTheSanPham bienTheSanPham, int[] giaTriThuocTinhIds, int[] sanPhamIds)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });

            var existing = db.BienTheSanPhams
                .Include(x => x.GiaTriThuocTinhs)
                .FirstOrDefault(x => x.BienTheId == bienTheSanPham.BienTheId);

            if (existing == null)
                return Json(new { success = false, message = "Không tìm thấy biến thể sản phẩm!" });

            // 🔹 Cập nhật thông tin chính
            //existing.Gia = bienTheSanPham.Gia;

            // Nếu không nhập giá → tự lấy giá theo sản phẩm hiện tại
            if (bienTheSanPham.Gia == null || bienTheSanPham.Gia == 0)
            {
                var sp = db.SanPhams.Find(existing.SanPhamId);
                existing.Gia = sp != null ? sp.GiaBan : existing.Gia;
            }
            else
            {
                existing.Gia = bienTheSanPham.Gia;
            }

            existing.GiaKhuyenMai = bienTheSanPham.GiaKhuyenMai;
            existing.MaVach = bienTheSanPham.MaVach;
            existing.TrangThai = bienTheSanPham.TrangThai;

            // 🔹 Cập nhật lại giá trị thuộc tính
            existing.GiaTriThuocTinhs.Clear();
            if (giaTriThuocTinhIds != null)
            {
                foreach (var id in giaTriThuocTinhIds)
                {
                    var giaTri = db.GiaTriThuocTinhs.Find(id);
                    if (giaTri != null)
                        existing.GiaTriThuocTinhs.Add(giaTri);
                }
            }

            // 🔹 Cập nhật danh sách sản phẩm áp dụng
            var sku = existing.SKU;
            var oldSanPhamIds = db.BienTheSanPhams
                .Where(x => x.SKU == sku)
                .Select(x => x.SanPhamId)
                .ToList();

            // Nếu bỏ chọn sản phẩm → xóa biến thể đó
            foreach (var oldId in oldSanPhamIds)
            {
                if (sanPhamIds == null || !sanPhamIds.Contains(oldId))
                {
                    var removeItem = db.BienTheSanPhams.FirstOrDefault(x => x.SKU == sku && x.SanPhamId == oldId);
                    if (removeItem != null)
                        db.BienTheSanPhams.Remove(removeItem);
                }
            }

            // Nếu chọn thêm sản phẩm mới → thêm biến thể mới
            if (sanPhamIds != null)
            {
                foreach (var newId in sanPhamIds)
                {
                    if (!oldSanPhamIds.Contains(newId))
                    {
                        var sp = db.SanPhams.Find(newId);

                        var newBienThe = new BienTheSanPham
                        {
                            SanPhamId = newId,
                            SKU = existing.SKU,
                            // Nếu không nhập giá hoặc = 0 → lấy giá riêng của sản phẩm đó
                            Gia = (bienTheSanPham.Gia == null || bienTheSanPham.Gia == 0)
                                    ? (sp != null ? sp.GiaBan : existing.Gia)
                                    : bienTheSanPham.Gia,
                            GiaKhuyenMai = bienTheSanPham.GiaKhuyenMai,
                            MaVach = bienTheSanPham.MaVach,
                            TrangThai = bienTheSanPham.TrangThai,
                            NgayTao = DateTime.Now,
                            GiaTriThuocTinhs = new List<GiaTriThuocTinh>()
                        };

                        foreach (var id in giaTriThuocTinhIds ?? new int[] { })
                        {
                            var gt = db.GiaTriThuocTinhs.Find(id);
                            if (gt != null)
                                newBienThe.GiaTriThuocTinhs.Add(gt);
                        }

                        db.BienTheSanPhams.Add(newBienThe);
                    }
                }
            }


            db.SaveChanges();
            return Json(new { success = true, message = "Cập nhật danh sách sản phẩm áp dụng biến thể thành công!" });

        }

        // GET: Admin/BienTheSanPhams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var bienTheSanPham = db.BienTheSanPhams.Include(x => x.SanPham)
                                    .Include(x => x.GiaTriThuocTinhs.Select(g => g.ThuocTinh))  
                                    .FirstOrDefault(x => x.BienTheId == id);
            if (bienTheSanPham == null)
            {
                return HttpNotFound();
            }
            return View(bienTheSanPham);
        }

        // POST: Admin/BienTheSanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var bienTheSanPham = db.BienTheSanPhams.Find(id);

            if (bienTheSanPham == null)
            {
                return Json(new { success = false, message = "Không tìm thấy biến thể sản phẩm." });
            }
            // ❗ Xóa trước liên kết ảnh
            if (bienTheSanPham.AnhSanPham_BienThes != null && bienTheSanPham.AnhSanPham_BienThes.Any())
            {
                db.AnhSanPham_BienThe.RemoveRange(bienTheSanPham.AnhSanPham_BienThes);
            }

            db.BienTheSanPhams.Remove(bienTheSanPham);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa biến thể sản phẩm thành công." });
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
            var bienTheSanPhams = db.BienTheSanPhams
                                .Include(b => b.AnhSanPham_BienThes)
                                .Where(sp => idList.Contains(sp.BienTheId))
                                .ToList();

            if (!bienTheSanPhams.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy biến thể sản phẩm." });
            }
            // ❗ Xóa liên kết ảnh trước
            foreach (var bt in bienTheSanPhams)
            {
                if (bt.AnhSanPham_BienThes != null && bt.AnhSanPham_BienThes.Any())
                {
                    db.AnhSanPham_BienThe.RemoveRange(bt.AnhSanPham_BienThes);
                }
            }
            db.BienTheSanPhams.RemoveRange(bienTheSanPhams);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa thành công." });
        }


        [HttpPost]
        public JsonResult GetGiaTriByThuocTinhs(int[] ids)
        {
            var giaTris = db.GiaTriThuocTinhs
                            .Where(x => ids.Contains(x.ThuocTinhId))
                            .Select(x => new { Value = x.GiaTriId, Text = x.TenGiaTri })
                            .ToList();

            return Json(giaTris);
        }




    }
}
