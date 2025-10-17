using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnChuyenNganh.Services;
using Microsoft.AspNet.Identity;



namespace DoAnChuyenNganh.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Products
        public ActionResult Index(string slug, int? id, decimal? minPrice, decimal? maxPrice, int page = 1, int pageSize = 6)
        {
            // ---- add begin ----
            var prevLL = db.Configuration.LazyLoadingEnabled;
            var prevADC = db.Configuration.AutoDetectChangesEnabled;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                var now = DateTime.Now;
                var sw = System.Diagnostics.Stopwatch.StartNew();

                // ✅ Chuẩn bị query cơ bản, không tracking
                var itemsQuery = db.SanPhams
                    .AsNoTracking()
                    .Include(s => s.AnhSanPhams)
                    .Include(s => s.DanhMuc)
                    .AsQueryable();

                // ✅ Lưu slug/id để partial menu biết danh mục hiện tại
                ViewBag.Slug = slug;
                ViewBag.DanhMucId = id;

                // ✅ Lọc theo danh mục
                if (!string.IsNullOrEmpty(slug))
                {
                    var cate = db.DanhMucs
                        .AsNoTracking()
                        .FirstOrDefault(x => x.Slug == slug || x.DanhMucId == id);

                    if (cate != null)
                    {
                        var allCateIds = GetAllCategoryIds(cate.DanhMucId);
                        allCateIds.Add(cate.DanhMucId);

                        itemsQuery = itemsQuery.Where(x => x.DanhMucId.HasValue && allCateIds.Contains(x.DanhMucId.Value));
                        ViewBag.CateName = cate.TenDanhMuc;
                        ViewBag.PageSize = pageSize;
                    }
                }

                // ✅ Lọc theo giá
                if (minPrice.HasValue)
                    itemsQuery = itemsQuery.Where(x => x.GiaBan >= minPrice.Value);
                if (maxPrice.HasValue)
                    itemsQuery = itemsQuery.Where(x => x.GiaBan <= maxPrice.Value);

                // ✅ Phân trang
                int totalItems = itemsQuery.Count();
                int totalPages = (int)Math.Ceiling((decimal)totalItems / pageSize);
                page = Math.Max(1, Math.Min(page, totalPages));

                var pagedItems = itemsQuery
                    .OrderByDescending(x => x.NgayTao)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();


                if (User.Identity.IsAuthenticated)
                {
                    var userId = User.Identity.GetUserId();
                    var favoriteIds = db.Favorites
                        .Where(f => f.UserId == userId)
                        .Select(f => f.SanPhamId)
                        .ToList();

                    foreach (var sp in pagedItems)
                    {
                        sp.IsFavorite = favoriteIds.Contains(sp.SanPhamId);
                    }
                }

                // ✅ Lấy danh sách ID
                var productIds = pagedItems.Select(x => x.SanPhamId).ToList();

                if (!productIds.Any())
                {
                    ViewBag.Page = page;
                    ViewBag.TotalPages = totalPages;
                    return View(pagedItems);
                }

                // ✅ Lấy biến thể 1 lần cho toàn bộ sản phẩm (gom nhóm)
                var allVariants = db.BienTheSanPhams
                    .AsNoTracking()
                    .Where(v => productIds.Contains(v.SanPhamId))
                    .GroupBy(v => v.SanPhamId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // ✅ Lấy khuyến mãi hợp lệ 1 lần (gom nhóm)
                var allPromos = db.KhuyenMais
                    .AsNoTracking()
                    .Where(km => (km.TrangThai ?? false) && km.NgayBatDau <= now && km.NgayKetThuc >= now)
                    .SelectMany(km => km.SanPhams.Select(sp => new
                    {
                        sp.SanPhamId,
                        km.TenKM,
                        km.Loai,
                        km.GiaTri,
                        km.TrangThai,
                        km.NgayBatDau,
                        km.NgayKetThuc
                    }))
                    .Where(p => productIds.Contains(p.SanPhamId))
                    .ToList()
                    .GroupBy(p => p.SanPhamId)
                    .ToDictionary(g => g.Key, g => g.Select(x => new KhuyenMai
                    {
                        TenKM = x.TenKM,
                        Loai = x.Loai,
                        GiaTri = x.GiaTri,
                        TrangThai = x.TrangThai,
                        NgayBatDau = x.NgayBatDau,
                        NgayKetThuc = x.NgayKetThuc
                    }).ToList());

                // ✅ Tính giá cuối cùng
                foreach (var sp in pagedItems)
                {
                    sp.BienTheSanPhams = allVariants.ContainsKey(sp.SanPhamId) ? allVariants[sp.SanPhamId] : new List<BienTheSanPham>();
                    sp.KhuyenMais = allPromos.ContainsKey(sp.SanPhamId) ? allPromos[sp.SanPhamId] : new List<KhuyenMai>();

                    var priceInfo = ProductPriceService.GetFinalPriceWithVariant(sp);
                    sp.ViewBag_FinalPrice = priceInfo.finalPrice;
                    sp.ViewBag_DiscountPercent = priceInfo.discountPercent;
                    sp.ViewBag_PromoName = priceInfo.promoName;
                }

                // ✅ Truyền view data
                ViewBag.FilterMin = minPrice ?? 100000m;
                ViewBag.FilterMax = maxPrice ?? 5000000m;
                ViewBag.Page = page;
                ViewBag.TotalPages = totalPages;

                sw.Stop();
                System.Diagnostics.Debug.WriteLine($"⏱ ProductsController.Index executed in {sw.ElapsedMilliseconds} ms for {pagedItems.Count} items.");

                return View(pagedItems);
            }

            finally
            {
                db.Configuration.LazyLoadingEnabled = prevLL;
                db.Configuration.AutoDetectChangesEnabled = prevADC;
            }
            
        }
        private List<int> GetAllCategoryIds(int parentId)
        {
            // ✅ Lấy toàn bộ danh mục 1 lần, nhẹ nhất có thể
            var all = db.DanhMucs
                .AsNoTracking()
                .Select(x => new { x.DanhMucId, x.DanhMucChaId })
                .ToList();

            // ✅ Lọc bỏ key null trước khi tạo dictionary
            var lookup = all
                .Where(x => x.DanhMucChaId.HasValue) // tránh null key
                .GroupBy(x => x.DanhMucChaId.Value)
                .ToDictionary(g => g.Key, g => g.Select(c => c.DanhMucId).ToList());

            var result = new List<int>();
            var stack = new Stack<int>();
            stack.Push(parentId);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (lookup.ContainsKey(current))
                {
                    foreach (var childId in lookup[current])
                    {
                        result.Add(childId);
                        stack.Push(childId);
                    }
                }
            }

            return result;
        }

        public ActionResult Detail(string slug, int? id)
        {
            if (id == null) return HttpNotFound();

            // 🔹 Chỉ load dữ liệu thật cần thiết và không tracking
            var item = db.SanPhams
                .AsNoTracking()
                .Include(s => s.KhuyenMais)
                .Include(s => s.AnhSanPhams.Select(a => a.AnhSanPham_BienThes))
                .Include(s => s.DanhMuc)
                .FirstOrDefault(x => x.SanPhamId == id);

            if (item == null) return HttpNotFound();
            // ✅ Tăng lượt xem sản phẩm (chỉ +1 mỗi lần người dùng load trang)
            try
            {
                db.Database.ExecuteSqlCommand(
                    "UPDATE SanPham SET LuotXem = ISNULL(LuotXem, 0) + 1, NgayCapNhat = GETDATE() WHERE SanPhamId = @p0",
                    item.SanPhamId
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Lỗi cập nhật lượt xem: " + ex.Message);
            }


            // 🔹 Lấy biến thể và thuộc tính bằng truy vấn riêng, tránh Include đệ quy nặng
            var bienThes = db.BienTheSanPhams
                .AsNoTracking()
                .Where(bt => bt.SanPhamId == id)
                .Include(bt => bt.GiaTriThuocTinhs.Select(g => g.ThuocTinh))
                .ToList();

            // Gắn lại thủ công (nếu View cần)
            item.BienTheSanPhams = bienThes;



            // ✅ Lấy danh sách ID biến thể trước
            var bienTheIds = bienThes.Select(bt => bt.BienTheId).ToList();

            // ✅ Sau đó truy vấn tồn kho
            var tonKhoList = db.TonKhoes
                .Where(t => bienTheIds.Contains(t.BienTheId))
                .GroupBy(t => t.BienTheId)
                .Select(g => new { BienTheId = g.Key, SoLuong = g.Sum(x => x.SoLuong) ?? 0 })
                .ToDictionary(x => x.BienTheId, x => x.SoLuong);


            // ✅ Làm mới tồn kho trong DB nếu có biến thể tồn âm (phòng sau khi hủy đơn cập nhật)
            if (tonKhoList.Values.Any(v => v < 0))
            {
                var tonAmIds = tonKhoList.Where(x => x.Value < 0).Select(x => x.Key).ToList();
                var tonAmList = db.TonKhoes.Where(t => tonAmIds.Contains(t.BienTheId)).ToList();
                foreach (var t in tonAmList)
                {
                    t.SoLuong = Math.Max(0, t.SoLuong ?? 0);
                    t.NgayCapNhat = DateTime.Now;
                }
                db.SaveChanges();
                tonKhoList = tonKhoList.ToDictionary(x => x.Key, x => Math.Max(0, x.Value));
            }


            // Tổng tồn kho sản phẩm
            ViewBag.TotalQuantity = tonKhoList.Values.Sum();

            // Tồn kho từng biến thể
            ViewBag.StockByVariant = tonKhoList
                            .ToDictionary(k => k.Key.ToString(), v => v.Value);


            // 🔹 Lấy danh sách màu sắc
            var colors = bienThes
                .SelectMany(bt => bt.GiaTriThuocTinhs)
                .Where(g => g.ThuocTinh.TenThuocTinh == "Màu sắc")
                .Select(g => g.TenGiaTri)
                .Distinct()
                .ToList();

            // 🔹 Lấy danh sách size
            var sizes = bienThes
                .SelectMany(bt => bt.GiaTriThuocTinhs)
                .Where(g => g.ThuocTinh.TenThuocTinh == "Size")
                .Select(g => g.TenGiaTri)
                .Distinct()
                .ToList();

            ViewBag.Colors = colors;
            ViewBag.Sizes = sizes;

            // ✅ Tính giá cuối cùng và gửi sang View
            var priceInfo = ProductPriceService.GetFinalPriceWithVariant(item);
            ViewBag.FinalPrice = priceInfo.finalPrice;
            ViewBag.DiscountPercent = priceInfo.discountPercent;
            ViewBag.PromoName = priceInfo.promoName;

            // ⭐ Tổng số review & sao trung bình (để hiển thị tĩnh ban đầu)
            var qReviews = db.BinhLuans.AsNoTracking()
                .Where(b => b.SanPhamId == id && (b.TrangThai ?? true) && b.Sao.HasValue);

            ViewBag.ReviewCount = qReviews.Count();
            ViewBag.AvgRating = qReviews.Any() ? qReviews.Average(b => b.Sao.Value) : 0;



            return View(item);

        }




        // ====================== BÌNH LUẬN ==========================
        public ActionResult GetReviews(int productId)
        {
            var reviews = db.BinhLuans
                .AsNoTracking()
                .Include(b => b.User) // join AspNetUsers
                .Where(b => b.SanPhamId == productId && (b.TrangThai ?? true))
                .OrderByDescending(b => b.NgayTao)
                .Select(b => new
                {
                    b.BinhLuanId,
                    b.NoiDung,
                    b.Sao,
                    b.NgayTao,
                    TenNguoi = b.User != null
                        ? (!string.IsNullOrEmpty(b.User.Fullname) ? b.User.Fullname : b.User.UserName)
                        : "Ẩn danh"
                })
                .ToList();

            return Json(reviews, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(int productId, string message, int rating = 0)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, msg = "Vui lòng đăng nhập để gửi đánh giá." });

            if (string.IsNullOrWhiteSpace(message))
                return Json(new { success = false, msg = "Vui lòng nhập nội dung bình luận." });

            try
            {
                var bl = new BinhLuan
                {
                    SanPhamId = productId,
                    UserId = userId,
                    NoiDung = message.Trim(),
                    Sao = rating > 0 && rating <= 5 ? rating : (int?)null,
                    NgayTao = DateTime.Now,
                    TrangThai = true
                };

                db.BinhLuans.Add(bl);
                db.SaveChanges();

                return Json(new { success = true, msg = "Cảm ơn bạn đã gửi đánh giá!" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Lỗi thêm bình luận: " + ex.Message);
                return Json(new { success = false, msg = "Có lỗi xảy ra, vui lòng thử lại." });
            }
        }







        public ActionResult Partial_ItemByCateId(int cateid = 0)
        {
            // ---- add begin (EF read-only mode) ----
            var prevLL = db.Configuration.LazyLoadingEnabled;
            var prevADC = db.Configuration.AutoDetectChangesEnabled;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.AutoDetectChangesEnabled = false;
            try
            {
                // 1) Lấy danh mục 1 lần
                var allCategories = db.DanhMucs
                    .Where(x => x.TrangThai == true)
                    .Select(x => new { x.DanhMucId, x.DanhMucChaId })
                    .AsNoTracking()
                    .ToList();

                // 2) Xác định danh sách danh mục con cần lấy
                List<int> danhMucConIds;
                if (cateid == 0)
                {
                    var danhMucChaIds = allCategories
                        .Where(x => x.DanhMucChaId == null)
                        .Select(x => x.DanhMucId)
                        .ToList();

                    danhMucConIds = allCategories
                        .Where(x => x.DanhMucChaId != null && danhMucChaIds.Contains(x.DanhMucChaId.Value))
                        .Select(x => x.DanhMucId)
                        .ToList();
                }
                else
                {
                    danhMucConIds = allCategories
                        .Where(x => x.DanhMucChaId == cateid)
                        .Select(x => x.DanhMucId)
                        .ToList();

                    if (!danhMucConIds.Any())
                        danhMucConIds.Add(cateid);
                }

                if (!danhMucConIds.Any())
                    return PartialView("Partial_ItemByCateId", new List<SanPham>());

                // 3A) Lấy nhẹ danh sách ứng viên (chỉ Id + DanhMucId + NgayTao)
                var candidates = db.SanPhams
                    .AsNoTracking()
                    .Where(sp => sp.DanhMucId.HasValue && danhMucConIds.Contains(sp.DanhMucId.Value))
                    .Select(sp => new { sp.SanPhamId, sp.DanhMucId, sp.NgayTao })
                    .ToList();

                // 3B) Group & chọn Top 3 ID theo mỗi danh mục con (ở RAM)
                var topIds = candidates
                    .GroupBy(x => x.DanhMucId.Value)
                    .SelectMany(g => g.OrderByDescending(x => x.NgayTao).Take(3).Select(x => x.SanPhamId))
                    .Distinct()
                    .ToList();

                if (!topIds.Any())
                    return PartialView("Partial_ItemByCateId", new List<SanPham>());

                // 4) Nạp đầy đủ chi tiết CHỈ CHO NHỮNG ID này (đủ để View render)
                var sanPhams = db.SanPhams
                    .AsNoTracking()
                    .Include("AnhSanPhams")
                    .Include("DanhMuc")
                    .Include("DanhMuc.DanhMuc2")
                    .Where(sp => topIds.Contains(sp.SanPhamId))
                    .ToList();

                // 5) Gộp biến thể & khuyến mãi cho đúng các ID (giống Index, 2 query gộp)
                var now = DateTime.Now;

                var allVariants = db.BienTheSanPhams
                    .AsNoTracking()
                    .Where(v => topIds.Contains(v.SanPhamId))
                    .GroupBy(v => v.SanPhamId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var allPromos = db.KhuyenMais
                    .AsNoTracking()
                    .Where(km => (km.TrangThai ?? false) && km.NgayBatDau <= now && km.NgayKetThuc >= now)
                    .SelectMany(km => km.SanPhams.Select(sp => new
                    {
                        sp.SanPhamId,
                        km.TenKM,
                        km.Loai,
                        km.GiaTri,
                        km.TrangThai,
                        km.NgayBatDau,
                        km.NgayKetThuc
                    }))
                    .Where(p => topIds.Contains(p.SanPhamId))
                    .ToList()
                    .GroupBy(p => p.SanPhamId)
                    .ToDictionary(g => g.Key, g => g.Select(x => new KhuyenMai
                    {
                        TenKM = x.TenKM,
                        Loai = x.Loai,
                        GiaTri = x.GiaTri,
                        TrangThai = x.TrangThai,
                        NgayBatDau = x.NgayBatDau,
                        NgayKetThuc = x.NgayKetThuc
                    }).ToList());

                // 6) Gắn lại & tính giá bằng service (logic y nguyên)
                foreach (var sp in sanPhams)
                {
                    sp.BienTheSanPhams = allVariants.ContainsKey(sp.SanPhamId) ? allVariants[sp.SanPhamId] : new List<BienTheSanPham>();
                    sp.KhuyenMais = allPromos.ContainsKey(sp.SanPhamId) ? allPromos[sp.SanPhamId] : new List<KhuyenMai>();

                    var priceInfo = ProductPriceService.GetFinalPriceWithVariant(sp);
                    sp.ViewBag_FinalPrice = priceInfo.finalPrice;
                    sp.ViewBag_DiscountPercent = priceInfo.discountPercent;
                    sp.ViewBag_PromoName = priceInfo.promoName;
                }

                // 7) Duy trì đúng thứ tự: theo nhóm danh mục con & NgayTao giảm dần
                var sanPhamsTop3Ordered = candidates
                    .GroupBy(x => x.DanhMucId.Value)
                    .SelectMany(g => g.OrderByDescending(x => x.NgayTao).Take(3))
                    .Select(x => x.SanPhamId)
                    .Join(sanPhams, id => id, sp => sp.SanPhamId, (id, sp) => sp)
                    .ToList();

                if (User.Identity.IsAuthenticated)
                {
                    var userId = User.Identity.GetUserId();
                    var favoriteIds = db.Favorites
                        .Where(f => f.UserId == userId)
                        .Select(f => f.SanPhamId)
                        .ToList();

                    foreach (var sp in sanPhamsTop3Ordered)
                        sp.IsFavorite = favoriteIds.Contains(sp.SanPhamId);
                }



                return PartialView("Partial_ItemByCateId", sanPhamsTop3Ordered);
            }
            finally
            {
                // ---- add end ----
                db.Configuration.LazyLoadingEnabled = prevLL;
                db.Configuration.AutoDetectChangesEnabled = prevADC;
            }
        }


        public ActionResult Partial_ProductSale()
        {
            var now = DateTime.Now;

            // Lấy sản phẩm có ít nhất 1 khuyến mãi hợp lệ
            var items = db.SanPhams
                .Include("DanhMuc")
                .Include("DanhMuc.DanhMuc2")
                .Include("AnhSanPhams")
                .Include("KhuyenMais")
                .Where(sp => sp.KhuyenMais.Any(km =>
                    km.TrangThai == true &&
                    km.NgayBatDau <= now &&
                    km.NgayKetThuc >= now))
                .OrderByDescending(sp => sp.NgayTao) // tuỳ chọn: sort mới nhất trước
                .AsNoTracking()  //  Thêm dòng này load nhanh hơn
                .ToList();
            // ✅ Áp dụng service tính giá, tránh tính tay từng nơi
            foreach (var sp in items)
            {
                var priceInfo = ProductPriceService.GetFinalPrice(sp);
                sp.ViewBag_FinalPrice = priceInfo.finalPrice;
                sp.ViewBag_DiscountPercent = priceInfo.discountPercent;
                sp.ViewBag_PromoName = priceInfo.promoName;
            }

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var favoriteIds = db.Favorites
                    .Where(f => f.UserId == userId)
                    .Select(f => f.SanPhamId)
                    .ToList();

                foreach (var sp in items)
                    sp.IsFavorite = favoriteIds.Contains(sp.SanPhamId);
            }


            return PartialView("_Partial_ProductSale", items);

        }

    }
}














