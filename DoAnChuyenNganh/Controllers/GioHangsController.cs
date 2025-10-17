using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using System.Net.Mail;
using System.Text;


namespace DoAnChuyenNganh.Controllers
{
    public class GioHangsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: GioHangs
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                Guid nguoiDungId = Guid.Parse(User.Identity.GetUserId());
                var gioHang = db.GioHangs
                    .AsNoTracking()
                    .Include(g => g.GioHangChiTiets.Select(ct => ct.BienTheSanPham))
                    .FirstOrDefault(x => x.NguoiDungId == nguoiDungId && x.TrangThai == 0);

                if (gioHang != null && gioHang.GioHangChiTiets.Any())
                {
                    var bienTheIds = gioHang.GioHangChiTiets.Select(ct => ct.BienTheId).ToList();

                    // Nạp sẵn ảnh & thuộc tính cho đúng biến thể có trong giỏ hàng
                    var bienThes = db.BienTheSanPhams
                        .AsNoTracking()
                        .Where(b => bienTheIds.Contains(b.BienTheId))
                        .Include(b => b.AnhSanPham_BienThes.Select(ab => ab.AnhSanPham))
                        .Include(b => b.SanPham.AnhSanPhams)
                        .Include(b => b.GiaTriThuocTinhs.Select(gt => gt.ThuocTinh))
                        .ToList();

                    // Gán lại navigation (để View đọc được)
                    foreach (var ct in gioHang.GioHangChiTiets)
                    {
                        ct.BienTheSanPham = bienThes.FirstOrDefault(b => b.BienTheId == ct.BienTheId);
                    }
                }

                // Không có giỏ hàng => view sẽ hiển thị "Giỏ hàng trống"
                if (gioHang == null) return View("Index", null);

                return View("Index", gioHang);
            }
            else
            {
                var cart = Session["Cart"] as List<CartItem>;
                return View("Index", cart);
            }
        }

        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            // Lấy biến thể
            var bienThe = db.BienTheSanPhams
                        .AsNoTracking()
                        .Include(b => b.AnhSanPham_BienThes.Select(ab => ab.AnhSanPham))
                        //.Include(b => b.SanPham.AnhSanPhams.Select(a => a.AnhSanPham_BienThes))
                        .Include(b => b.SanPham.KhuyenMais)
                        .Include(b => b.GiaTriThuocTinhs.Select(gt => gt.ThuocTinh))
                        .FirstOrDefault(x => x.BienTheId == id);

            if (bienThe == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });

            decimal donGia = TinhGia(bienThe);

            // Lấy giá trị biến thể (màu, size) nếu có
            var mau = bienThe.GiaTriThuocTinhs
                    .FirstOrDefault(x =>
                        x.ThuocTinh.TenThuocTinh.Equals("Màu sắc", StringComparison.OrdinalIgnoreCase) ||
                        x.ThuocTinh.TenThuocTinh.Equals("Màu", StringComparison.OrdinalIgnoreCase))
                    ?.TenGiaTri;

            var size = bienThe.GiaTriThuocTinhs
                .FirstOrDefault(x =>
                    x.ThuocTinh.TenThuocTinh.Equals("Size", StringComparison.OrdinalIgnoreCase) ||
                    x.ThuocTinh.TenThuocTinh.Equals("Kích thước", StringComparison.OrdinalIgnoreCase))
                ?.TenGiaTri;

            // ===== ĐÃ ĐĂNG NHẬP → LƯU DB =====
            if (User.Identity.IsAuthenticated)
            {
                Guid nguoiDungId = Guid.Parse(User.Identity.GetUserId());
                var gioHang = db.GioHangs.FirstOrDefault(x => x.NguoiDungId == nguoiDungId && x.TrangThai == 0);

                if (gioHang == null)
                {
                    gioHang = new GioHang
                    {
                        NguoiDungId = nguoiDungId,
                        NgayTao = DateTime.Now,
                        TrangThai = 0
                    };
                    db.GioHangs.Add(gioHang);
                    db.SaveChanges();
                }

                var chiTiet = db.GioHangChiTiets
                                .FirstOrDefault(x => x.GioHangId == gioHang.GioHangId && x.BienTheId == id);

                if (chiTiet != null)
                {
                    chiTiet.SoLuong += quantity;
                    chiTiet.ThanhTien = chiTiet.SoLuong * chiTiet.DonGia; // DonGia là decimal (non-null)
                }
                else
                {
                    // ✅ Lấy ảnh đúng với biến thể
                    var variantImage = bienThe.AnhSanPham_BienThes
                                            .Select(x => x.AnhSanPham.Url)
                                            .FirstOrDefault()
                                        ?? bienThe.SanPham.AnhSanPhams
                                            .FirstOrDefault(x => x.MacDinh == true)?.Url;

                    db.GioHangChiTiets.Add(new GioHangChiTiet
                    {
                        GioHangId = gioHang.GioHangId,
                        BienTheId = id,
                        SoLuong = quantity,
                        DonGia = donGia,
                        ThanhTien = quantity * donGia,
                        //HinhAnh = variantImage   // 👈 THÊM DÒNG NÀY để lưu URL ảnh biến thể
                    });
                }

                db.SaveChanges();

                int count = db.GioHangChiTiets
                              .Where(x => x.GioHangId == gioHang.GioHangId)
                              .Sum(x => x.SoLuong);

                return Json(new { success = true, message = "Đã thêm vào giỏ hàng", count = count });
            }

            // ===== CHƯA ĐĂNG NHẬP → LƯU SESSION =====
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.BienTheId == id);
            if (item != null)
            {
                item.SoLuong += quantity;
                item.Mau = mau ?? item.Mau;
                item.Size = size ?? item.Size;
            }
            else
            {
                var variantImage = bienThe.AnhSanPham_BienThes
                                        .Select(x => x.AnhSanPham.Url)
                                        .FirstOrDefault()
                                   ?? bienThe.SanPham.AnhSanPhams
                                        .FirstOrDefault(x => x.MacDinh == true)?.Url;

                cart.Add(new CartItem
                {
                    BienTheId = id,
                    Name = bienThe.SanPham.TenSanPham,
                    DonGia = donGia,
                    SoLuong = quantity,
                    Image = variantImage,
                    Mau = mau,
                    Size = size
                });
            }

            Session["Cart"] = cart;
            int sessionCount = cart.Sum(x => x.SoLuong);

            return Json(new { success = true, message = "Đã thêm vào giỏ hàng", count = sessionCount });
        }

        private decimal TinhGia(BienTheSanPham bienThe)
        {
            // DonGia gốc: ưu tiên giá khuyến mãi biến thể -> giá biến thể -> giá bán SP
            decimal gia = bienThe.GiaKhuyenMai ?? bienThe.Gia ?? bienThe.SanPham.GiaBan;

            var km = bienThe.SanPham.KhuyenMais.FirstOrDefault(x =>
                            (x.TrangThai ?? false) &&
                             x.NgayBatDau <= DateTime.Now &&
                             x.NgayKetThuc >= DateTime.Now);

            if (km != null && km.GiaTri.HasValue)
            {
                gia = km.Loai == "PERCENT"
                    ? gia - (gia * km.GiaTri.Value / 100)
                    : gia - km.GiaTri.Value;
                if (gia < 0) gia = 0;
            }
            return gia;
        }

        [ChildActionOnly]
        public PartialViewResult CartCount()
        {
            int count = 0;
            if (User.Identity.IsAuthenticated)
            {
                var userId = Guid.Parse(User.Identity.GetUserId());
                count = db.GioHangChiTiets
                          .Where(c => c.GioHang.NguoiDungId == userId && c.GioHang.TrangThai == 0)
                          .Sum(c => (int?)c.SoLuong) ?? 0;
            }
            else
            {
                var cart = Session["Cart"] as List<CartItem>;
                if (cart != null) count = cart.Sum(x => x.SoLuong);
            }

            return PartialView("CartCount", count);
        }

        [HttpGet]
        public ActionResult GetCartCount()
        {
            int count = 0;
            if (User.Identity.IsAuthenticated)
            {
                var userId = Guid.Parse(User.Identity.GetUserId());
                count = db.GioHangChiTiets
                          .Where(c => c.GioHang.NguoiDungId == userId && c.GioHang.TrangThai == 0)
                          .Sum(c => (int?)c.SoLuong) ?? 0;
            }
            else
            {
                var cart = Session["Cart"] as List<CartItem>;
                if (cart != null) count = cart.Sum(x => x.SoLuong);
            }
            return Content(count.ToString());
        }

        public JsonResult FindVariant(int productId, string color, string size)
        {
            var variants = db.BienTheSanPhams
                            .Include("GiaTriThuocTinhs.ThuocTinh")
                            .Where(x => x.SanPhamId == productId)
                            .ToList();

            // Tìm đúng biến thể theo màu & size
            var bienThe = variants.FirstOrDefault(v =>
                v.GiaTriThuocTinhs.Any(g =>
                    (g.ThuocTinh.TenThuocTinh.Equals("Màu sắc", StringComparison.OrdinalIgnoreCase) ||
                     g.ThuocTinh.TenThuocTinh.Equals("Màu", StringComparison.OrdinalIgnoreCase)) &&
                     g.TenGiaTri.Trim().ToLower() == color.Trim().ToLower())
                &&
                v.GiaTriThuocTinhs.Any(g =>
                    (g.ThuocTinh.TenThuocTinh.Equals("Kích thước", StringComparison.OrdinalIgnoreCase) ||
                     g.ThuocTinh.TenThuocTinh.Equals("Size", StringComparison.OrdinalIgnoreCase)) &&
                     g.TenGiaTri.Trim().ToLower() == size.Trim().ToLower())
            );

            if (bienThe != null)
                return Json(new { success = true, variantId = bienThe.BienTheId }, JsonRequestBehavior.AllowGet);

            return Json(new { success = false, message = "Không tìm thấy biến thể phù hợp." }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UpdateQuantity(int variantId, int quantity)
        {
            if (quantity <= 0)
                return Json(new { success = false, message = "Số lượng không hợp lệ." });

            // ===== ĐÃ ĐĂNG NHẬP =====
            if (User.Identity.IsAuthenticated)
            {
                Guid nguoiDungId = Guid.Parse(User.Identity.GetUserId());
                var gioHang = db.GioHangs
                                .Include("GioHangChiTiets")
                                .FirstOrDefault(x => x.NguoiDungId == nguoiDungId && x.TrangThai == 0);

                if (gioHang == null)
                    return Json(new { success = false, message = "Không tìm thấy giỏ hàng." });

                var chiTiet = gioHang.GioHangChiTiets.FirstOrDefault(x => x.BienTheId == variantId);
                if (chiTiet == null)
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });

                chiTiet.SoLuong = quantity;
                chiTiet.ThanhTien = chiTiet.DonGia * chiTiet.SoLuong;
                db.SaveChanges();

                decimal tongTien = gioHang.GioHangChiTiets.Sum(x => x.ThanhTien ?? 0);
                return Json(new { success = true, message = "Cập nhật thành công.", total = tongTien.ToString("N0") + " ₫" });
            }

            // ===== CHƯA ĐĂNG NHẬP =====
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null) return Json(new { success = false, message = "Giỏ hàng trống." });

            var item = cart.FirstOrDefault(x => x.BienTheId == variantId);
            if (item == null)
                return Json(new { success = false, message = "Không tìm thấy sản phẩm." });

            item.SoLuong = quantity;
            Session["Cart"] = cart;

            decimal tongSession = cart.Sum(x => x.SoLuong * x.DonGia);
            return Json(new { success = true, message = "Cập nhật thành công.", total = tongSession.ToString("N0") + " ₫" });
        }



        [HttpPost]
        public ActionResult RemoveItem(int variantId)
        {
            // ===== ĐÃ ĐĂNG NHẬP =====
            if (User.Identity.IsAuthenticated)
            {
                Guid nguoiDungId = Guid.Parse(User.Identity.GetUserId());
                var gioHang = db.GioHangs
                                .Include("GioHangChiTiets")
                                .FirstOrDefault(x => x.NguoiDungId == nguoiDungId && x.TrangThai == 0);

                if (gioHang == null)
                    return Json(new { success = false, message = "Không tìm thấy giỏ hàng." });

                var chiTiet = gioHang.GioHangChiTiets.FirstOrDefault(x => x.BienTheId == variantId);
                if (chiTiet == null)
                    return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });

                db.GioHangChiTiets.Remove(chiTiet);
                db.SaveChanges();

                decimal tongTien = gioHang.GioHangChiTiets.Sum(x => x.ThanhTien ?? 0);
                return Json(new { success = true, message = "Đã xóa sản phẩm.", total = tongTien.ToString("N0") + " ₫" });
            }

            // ===== CHƯA ĐĂNG NHẬP =====
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null) return Json(new { success = false, message = "Giỏ hàng trống." });

            var item = cart.FirstOrDefault(x => x.BienTheId == variantId);
            if (item == null)
                return Json(new { success = false, message = "Không tìm thấy sản phẩm." });

            cart.Remove(item);
            Session["Cart"] = cart;

            decimal tongSession = cart.Sum(x => x.SoLuong * x.DonGia);
            return Json(new { success = true, message = "Đã xóa sản phẩm.", total = tongSession.ToString("N0") + " ₫" });
        }



        [HttpPost]
        public ActionResult ClearCart()
        {
            if (User.Identity.IsAuthenticated)
            {
                Guid userId = Guid.Parse(User.Identity.GetUserId());
                var gioHang = db.GioHangs
                                .Include("GioHangChiTiets")
                                .FirstOrDefault(x => x.NguoiDungId == userId && x.TrangThai == 0);

                if (gioHang != null)
                {
                    db.GioHangChiTiets.RemoveRange(gioHang.GioHangChiTiets);
                    db.SaveChanges();
                }
            }
            else
            {
                Session["Cart"] = null;
            }
            return Json(new { success = true, message = "Đã xóa toàn bộ giỏ hàng." });
        }

        [HttpPost]
        public ActionResult RemoveSelected(int[] variantIds)
        {
            if (variantIds == null || variantIds.Length == 0)
                return Json(new { success = false, message = "Không có sản phẩm nào được chọn." });

            if (User.Identity.IsAuthenticated)
            {
                Guid nguoiDungId = Guid.Parse(User.Identity.GetUserId());
                var gioHang = db.GioHangs
                                .Include("GioHangChiTiets")
                                .FirstOrDefault(x => x.NguoiDungId == nguoiDungId && x.TrangThai == 0);
                if (gioHang == null)
                    return Json(new { success = false, message = "Không tìm thấy giỏ hàng." });

                var toRemove = gioHang.GioHangChiTiets
                    .Where(ct => variantIds.Contains(ct.BienTheId))
                    .ToList();

                db.GioHangChiTiets.RemoveRange(toRemove);
                db.SaveChanges();

                return Json(new { success = true, message = "Đã xóa các sản phẩm đã chọn." });
            }

            // Chưa đăng nhập
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
                return Json(new { success = false, message = "Giỏ hàng trống." });

            cart.RemoveAll(x => variantIds.Contains(x.BienTheId));
            Session["Cart"] = cart;
            return Json(new { success = true, message = "Đã xóa các sản phẩm đã chọn." });
        }

        public ActionResult ThanhToan(string selected)
        {
            int[] selectedIds = string.IsNullOrEmpty(selected)
                ? null
                : selected.Split(',').Select(int.Parse).ToArray();

            // ====== ĐÃ ĐĂNG NHẬP ======
            if (User.Identity.IsAuthenticated)
            {
                var userId = Guid.Parse(User.Identity.GetUserId());

                // 1️⃣ Lấy toàn bộ thông tin user và địa chỉ chỉ với 2 truy vấn gọn
                var user = db.Users
                    .AsNoTracking()
                    .Where(u => u.Id == userId.ToString())
                    .Select(u => new { u.Fullname, u.Email, u.PhoneNumber })
                    .FirstOrDefault();

                if (user == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin người dùng.";
                    return RedirectToAction("Index");
                }

                ViewBag.FullName = user.Fullname;
                ViewBag.Email = user.Email;
                ViewBag.Phone = user.PhoneNumber;
                ViewBag.Selected = selected;

                // 2️⃣ Lấy danh sách địa chỉ (có sắp xếp mặc định trước)
                var diaChis = db.DiaChiGiaoHangs
                    .AsNoTracking()
                    .Where(x => x.Email == user.Email)
                    .OrderByDescending(x => x.MacDinh)
                    .ThenByDescending(x => x.DiaChiId)
                    .ToList();

                ViewBag.DanhSachDiaChi = diaChis;
                ViewBag.DiaChiMacDinh = diaChis.FirstOrDefault();
                // 1️⃣ Lấy giỏ hàng chính
                var gioHang = db.GioHangs
                    .AsNoTracking()
                    .Include(g => g.GioHangChiTiets)
                    .FirstOrDefault(x => x.NguoiDungId == userId && x.TrangThai == 0);

                if (gioHang == null)
                {
                    TempData["Error"] = "Giỏ hàng trống.";
                    return RedirectToAction("Index");
                }

                // 2️⃣ Lấy danh sách biến thể cần thiết (theo ID)
                var bienTheIds = gioHang.GioHangChiTiets.Select(ct => ct.BienTheId).ToList();

                var bienThes = db.BienTheSanPhams
                    .AsNoTracking()
                    .Where(b => bienTheIds.Contains(b.BienTheId))
                    .Include(b => b.SanPham)
                    .Include(b => b.SanPham.AnhSanPhams)
                    .Include(b => b.GiaTriThuocTinhs.Select(gt => gt.ThuocTinh))
                    .ToList();

                // 3️⃣ Gắn lại navigation thủ công (rất nhanh, không tốn JOIN)
                foreach (var ct in gioHang.GioHangChiTiets)
                {
                    ct.BienTheSanPham = bienThes.FirstOrDefault(b => b.BienTheId == ct.BienTheId);
                }


                // 4️⃣ Lọc các sản phẩm được chọn
                if (selectedIds != null)
                {
                    gioHang.GioHangChiTiets = gioHang.GioHangChiTiets
                        .Where(ct => selectedIds.Contains(ct.BienTheId))
                        .ToList();
                }

                // 5️⃣ Tối ưu view model (chỉ gửi ra View dữ liệu cần hiển thị)
                var viewModel = new
                {
                    GioHangId = gioHang.GioHangId,
                    ChiTiets = gioHang.GioHangChiTiets.Select(ct => new
                    {
                        ct.BienTheId,
                        SanPham = ct.BienTheSanPham.SanPham.TenSanPham,
                        ct.SoLuong,
                        ct.DonGia,
                        ct.ThanhTien,
                        Image = ct.BienTheSanPham.SanPham.AnhSanPhams.FirstOrDefault(a => a.MacDinh == true)?.Url
                    }).ToList()
                };

                return View("ThanhToan", viewModel);
            }

            // ====== KHÁCH CHƯA ĐĂNG NHẬP ======
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống.";
                return RedirectToAction("Index");
            }

            if (selectedIds != null)
                cart = cart.Where(c => selectedIds.Contains(c.BienTheId)).ToList();

            return View("ThanhToan", cart);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DatHang(FormCollection form)
        {
            try
            {
                string hoTen = form["HoTen"];
                string sdt = form["DienThoai"];
                string diaChi = form["DiaChi"];
                string tinh = form["Tinh"];
                string huyen = form["Huyen"];
                string xa = form["Xa"];
                string ghiChu = form["GhiChu"];
                string email = form["Email"];
                int phuongThucThanhToanId = int.Parse(form["PhuongThucThanhToanId"]);

                string selectedIdsStr = form["SelectedIds"];
                List<int> selectedIds = new List<int>();
                if (!string.IsNullOrEmpty(selectedIdsStr))
                    selectedIds = selectedIdsStr.Split(',').Select(int.Parse).ToList();

                decimal tongTien = 0;
                DonHang donHang = null;
                List<DonHangChiTiet> chiTiets = new List<DonHangChiTiet>();

                // ========== ĐÃ ĐĂNG NHẬP ==========
                if (User.Identity.IsAuthenticated)
                {
                    Guid nguoiDungId = Guid.Parse(User.Identity.GetUserId());
                    var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == nguoiDungId.ToString());
                    if (user == null)
                    {
                        TempData["Error"] = "Không tìm thấy tài khoản.";
                        return RedirectToAction("Index");
                    }

                    // Kiểm tra nếu tick dùng địa chỉ cũ
                    // ✅ Kiểm tra nếu tick dùng địa chỉ cũ
                    bool useOldAddress = !string.IsNullOrEmpty(form["DiaChiId"]);
                    KhachHang khachHang = null;
                    DiaChiGiaoHang diaChiGiaoHang = null;

                    if (useOldAddress)
                    {
                        // 👉 Lấy lại đúng địa chỉ cũ đã chọn
                        int diaChiId = int.Parse(form["DiaChiId"]);
                        diaChiGiaoHang = db.DiaChiGiaoHangs.AsNoTracking().FirstOrDefault(d => d.DiaChiId == diaChiId);

                        if (diaChiGiaoHang == null)
                        {
                            TempData["Error"] = "Không tìm thấy địa chỉ giao hàng cũ.";
                            return RedirectToAction("Index");
                        }

                        // 👉 Dùng lại khách hàng cũ từ địa chỉ đó
                        khachHang = db.KhachHangs.FirstOrDefault(k => k.KhachHangId == diaChiGiaoHang.KhachHangId);
                        if (khachHang == null)
                        {
                            TempData["Error"] = "Không tìm thấy thông tin khách hàng.";
                            return RedirectToAction("Index");
                        }

                        // 👉 KHÔNG tạo mới khách hàng hay địa chỉ
                    }
                    else
                    {
                        // 👉 Không tick => tạo mới khách hàng + địa chỉ
                        khachHang = new KhachHang
                        {
                            NgaySinh = null,
                            GioiTinh = null,
                            TongDiemTichLuy = 0
                        };
                        db.KhachHangs.Add(khachHang);
                        db.SaveChanges();

                        diaChiGiaoHang = new DiaChiGiaoHang
                        {
                            KhachHangId = khachHang.KhachHangId,
                            HoTen = hoTen,
                            DienThoai = sdt,
                            Email = email,
                            Tinh = tinh,
                            Huyen = huyen,
                            Xa = xa,
                            DiaChiChiTiet = diaChi,
                            MacDinh = true
                        };
                        db.DiaChiGiaoHangs.Add(diaChiGiaoHang);
                        db.SaveChanges();
                    }


                    db.SaveChanges(); // Lưu 1 lần sau khi thêm địa chỉ

                    // Lấy giỏ hàng
                    var gioHang = db.GioHangs
                        .Include("GioHangChiTiets.BienTheSanPham.SanPham")
                        .FirstOrDefault(x => x.NguoiDungId == nguoiDungId && x.TrangThai == 0);

                    if (gioHang == null)
                    {
                        TempData["Error"] = "Không tìm thấy giỏ hàng.";
                        return RedirectToAction("Index");
                    }

                    var chiTietChon = gioHang.GioHangChiTiets
                        .Where(ct => selectedIds == null || selectedIds.Contains(ct.BienTheId))
                        .ToList();

                    if (!chiTietChon.Any())
                    {
                        TempData["Error"] = "Không có sản phẩm nào được chọn.";
                        return RedirectToAction("Index");
                    }

                    tongTien = chiTietChon.Sum(x => x.ThanhTien ?? 0);

                    // Tạo đơn hàng
                    donHang = new DonHang
                    {
                        MaDonHang = "DH" + DateTime.Now.Ticks,
                        NgayTao = DateTime.Now,
                        TrangThai = 0,
                        TongTien = tongTien,
                        PhuongThucThanhToanId = phuongThucThanhToanId,
                        GhiChu = ghiChu,
                        KhachHangId = khachHang.KhachHangId,
                        DiaChiGiaoHangId = diaChiGiaoHang.DiaChiId
                    };
                    db.DonHangs.Add(donHang);
                    db.SaveChanges();

                    // Thêm chi tiết đơn hàng
                    chiTiets = chiTietChon.Select(item => new DonHangChiTiet
                    {
                        DonHangId = donHang.DonHangId,
                        BienTheId = item.BienTheId,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia,
                        ThanhTien = item.ThanhTien
                    }).ToList();

                    db.DonHangChiTiets.AddRange(chiTiets);

                    // Đánh dấu giỏ hàng đã đặt
                    gioHang.TrangThai = 1;
                    db.SaveChanges();
                    // ✅ Cập nhật tồn kho sau khi tạo đơn hàng
                    var bienTheIds = chiTiets.Select(ct => ct.BienTheId).ToList();
                    var tonKhoList = db.TonKhoes.Where(t => bienTheIds.Contains(t.BienTheId)).ToList();
                    foreach (var item in chiTiets)
                    {
                        var tonKho = tonKhoList.FirstOrDefault(t => t.BienTheId == item.BienTheId);
                        if (tonKho != null)
                        {
                            tonKho.SoLuong = (tonKho.SoLuong ?? 0) - item.SoLuong;
                            tonKho.NgayCapNhat = DateTime.Now;

                            // Ghi log lịch sử
                            db.LichSuTonKhoes.Add(new LichSuTonKho
                            {
                                BienTheId = item.BienTheId,
                                KhoId = tonKho.KhoId,
                                SoThayDoi = -item.SoLuong,
                                GhiChu = $"Giảm tồn kho khi đặt đơn hàng #{donHang.MaDonHang}",
                                NgayThucHien = DateTime.Now,
                                NguoiThucHien = User.Identity.IsAuthenticated ? Guid.Parse(User.Identity.GetUserId()) : (Guid?)null
                            });
                        }
                    }
                    db.SaveChanges();


                }

                // ========== KHÁCH CHƯA ĐĂNG NHẬP ==========
                else
                {
                    var cart = Session["Cart"] as List<CartItem>;
                    if (cart == null || !cart.Any())
                        return RedirectToAction("Index");

                    tongTien = cart.Sum(x => x.DonGia * x.SoLuong);

                    // Tạo khách vãng lai
                    var kh = new KhachHang
                    {
                        GioiTinh = null,
                        NgaySinh = null,
                        TongDiemTichLuy = 0
                    };
                    db.KhachHangs.Add(kh);
                    db.SaveChanges();

                    // Tạo địa chỉ giao hàng
                    //var email = form["Email"];
                    var diaChiGiaoHang = new DiaChiGiaoHang
                    {
                        KhachHangId = kh.KhachHangId,
                        HoTen = hoTen,
                        DienThoai = sdt,
                        Email = email,
                        DiaChiChiTiet = diaChi,
                        Tinh = "",
                        Huyen = "",
                        Xa = "",
                        MacDinh = true
                    };
                    db.DiaChiGiaoHangs.Add(diaChiGiaoHang);
                    db.SaveChanges();

                    // Tạo đơn hàng
                    donHang = new DonHang
                    {
                        MaDonHang = "DH" + DateTime.Now.Ticks,
                        NgayTao = DateTime.Now,
                        TrangThai = 0,
                        TongTien = tongTien,
                        PhuongThucThanhToanId = phuongThucThanhToanId,
                        GhiChu = ghiChu,
                        KhachHangId = kh.KhachHangId,
                        DiaChiGiaoHangId = diaChiGiaoHang.DiaChiId
                    };
                    db.DonHangs.Add(donHang);
                    db.SaveChanges();

                    // Thêm chi tiết đơn hàng
                    foreach (var item in cart)
                    {
                        chiTiets.Add(new DonHangChiTiet
                        {
                            DonHangId = donHang.DonHangId,
                            BienTheId = item.BienTheId,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            ThanhTien = item.SoLuong * item.DonGia
                        });
                    }
                    db.DonHangChiTiets.AddRange(chiTiets);
                    db.SaveChanges();
                    // ✅ Cập nhật tồn kho sau khi tạo đơn hàng
                    var bienTheIds = chiTiets.Select(ct => ct.BienTheId).ToList();
                    var tonKhoList = db.TonKhoes.Where(t => bienTheIds.Contains(t.BienTheId)).ToList();
                    foreach (var item in chiTiets)
                    {
                        var tonKho = tonKhoList.FirstOrDefault(t => t.BienTheId == item.BienTheId);
                        if (tonKho != null)
                        {
                            tonKho.SoLuong = (tonKho.SoLuong ?? 0) - item.SoLuong;
                            tonKho.NgayCapNhat = DateTime.Now;

                            // Ghi log lịch sử
                            db.LichSuTonKhoes.Add(new LichSuTonKho
                            {
                                BienTheId = item.BienTheId,
                                KhoId = tonKho.KhoId,
                                SoThayDoi = -item.SoLuong,
                                GhiChu = $"Giảm tồn kho khi đặt đơn hàng #{donHang.MaDonHang}",
                                NgayThucHien = DateTime.Now,
                                NguoiThucHien = User.Identity.IsAuthenticated ? Guid.Parse(User.Identity.GetUserId()) : (Guid?)null
                            });
                        }
                    }
                    db.SaveChanges();

                    Session["Cart"] = null;
                }

                // ===== Sau khi tạo đơn hàng & chi tiết thành công =====

                // Lấy danh sách chi tiết có thông tin sản phẩm để hiển thị trong email
                var chiTietDonHang = db.DonHangChiTiets
                    .Include(ct => ct.BienTheSanPham.SanPham)
                    .Where(ct => ct.DonHangId == donHang.DonHangId)
                    .ToList();

                // Xây dựng bảng chi tiết sản phẩm HTML
                StringBuilder chiTietHtml = new StringBuilder();
                chiTietHtml.Append("<table border='1' cellspacing='0' cellpadding='6' style='border-collapse: collapse; width:100%; font-family: Arial, sans-serif;'>");
                chiTietHtml.Append("<tr style='background-color:#f2f2f2; text-align:center; font-weight:bold;'>");
                chiTietHtml.Append("<th>Tên sản phẩm</th><th>Số lượng</th><th>Đơn giá</th><th>Thành tiền</th></tr>");

                foreach (var ct in chiTietDonHang)
                {
                    string tenSp = ct.BienTheSanPham?.SanPham?.TenSanPham ?? "Sản phẩm";
                    string mau = ct.BienTheSanPham?.GiaTriThuocTinhs?
                        .FirstOrDefault(g => g.ThuocTinh.TenThuocTinh == "Màu sắc")?.TenGiaTri;
                    string size = ct.BienTheSanPham?.GiaTriThuocTinhs?
                        .FirstOrDefault(g => g.ThuocTinh.TenThuocTinh == "Size")?.TenGiaTri;

                    string bienThe = "";
                    if (!string.IsNullOrEmpty(mau)) bienThe += $"Màu: {mau} ";
                    if (!string.IsNullOrEmpty(size)) bienThe += $" - Size: {size}";

                    chiTietHtml.Append("<tr>");
                    chiTietHtml.Append($"<td>{tenSp}<br/><small>{bienThe}</small></td>");
                    chiTietHtml.Append($"<td align='center'>{ct.SoLuong}</td>");
                    chiTietHtml.Append($"<td align='right'>{ct.DonGia:N0} đ</td>");
                    chiTietHtml.Append($"<td align='right'>{ct.ThanhTien:N0} đ</td>");
                    chiTietHtml.Append("</tr>");
                }

                chiTietHtml.Append("</table>");

                // Tạo nội dung email
                string subject = $"Xác nhận đơn hàng #{donHang.MaDonHang} - KRIK Shop";

                string body = $@"
                    <div style='font-family: Arial, sans-serif; font-size:14px; color:#333;'>
                        <h2 style='color:#d32f2f;'>Cảm ơn bạn đã đặt hàng tại KRIK SHOP!</h2>
                        <p>Xin chào <strong>{hoTen}</strong>,</p>
                        <p>Đơn hàng của bạn đã được ghi nhận thành công với thông tin như sau:</p>
                        <p><strong>Mã đơn hàng:</strong> {donHang.MaDonHang}<br/>
                           <strong>Ngày đặt:</strong> {donHang.NgayTao:dd/MM/yyyy HH:mm}<br/>
                           <strong>Tổng tiền:</strong> {donHang.TongTien:N0} đ<br/>
                           <strong>Phương thức thanh toán:</strong> {(phuongThucThanhToanId == 1 ? "Thanh toán khi nhận hàng" : phuongThucThanhToanId == 2 ? "Thanh toán qua VNPAY" : "Chuyển khoản ngân hàng")}
                        </p>
                
                        <h3>Chi tiết đơn hàng</h3>
                        {chiTietHtml}
                
                        <p style='margin-top:20px;'>Địa chỉ giao hàng: <br/>
                           <strong>{diaChi}, {xa}, {huyen}, {tinh}</strong>
                        </p>
                
                        <p style='margin-top:20px;'>Chúng tôi sẽ sớm liên hệ để xác nhận đơn hàng và giao hàng cho bạn.</p>
                
                        <p style='margin-top:30px;'>
                            Trân trọng,<br/>
                            <strong>KRIK SHOP</strong><br/>
                            <a href='https://krikshop.vn' style='color:#d32f2f;'>https://krikshop.vn</a>
                        </p>
                    </div>
                ";

                try
                {
                    if (!string.IsNullOrEmpty(email))
                    {
                        SendMail(email, subject, body);
                    }
                }
                catch (Exception mailEx)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Gửi mail xác nhận thất bại: " + mailEx.Message);
                }


                // ========== XỬ LÝ SAU CÙNG ==========
                if (phuongThucThanhToanId == 2)
                    return RedirectToAction("ThanhToanVnPay", "ThanhToan", new { donHangId = donHang.DonHangId });
                else if (phuongThucThanhToanId == 3)
                    return RedirectToAction("ChuyenKhoan", "ThanhToan", new { donHangId = donHang.DonHangId });


                return RedirectToAction("ThanhCong", new { id = donHang.DonHangId });
            }
            catch (Exception ex)
            {
                // TODO: Ghi log lỗi
                System.Diagnostics.Debug.WriteLine("Lỗi khi đặt hàng: " + ex.Message);
                TempData["Error"] = "Đặt hàng thất bại, vui lòng thử lại.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult ThanhCong(long id)
        {
            var donHang = db.DonHangs
                .AsNoTracking()
                .Include(d => d.DiaChiGiaoHang)
                .Include(d => d.DonHangChiTiets)
                .FirstOrDefault(d => d.DonHangId == id);

            if (donHang == null)
                return HttpNotFound();

            // 🔹 ĐÁNH DẤU QUYỀN XEM ĐƠN HÀNG CHO KHÁCH VÃNG LAI
            var list = (List<long>)Session["GuestOrderIds"] ?? new List<long>();
            if (!list.Contains(id)) list.Add(id);
            Session["GuestOrderIds"] = list;

            // Load thủ công để đảm bảo có dữ liệu
            foreach (var ct in donHang.DonHangChiTiets)
            {
                ct.BienTheSanPham = db.BienTheSanPhams.AsNoTracking()
                    .Include(b => b.SanPham)
                    .Include(b => b.GiaTriThuocTinhs.Select(gt => gt.ThuocTinh))
                    .FirstOrDefault(b => b.BienTheId == ct.BienTheId);
            }

            return View(donHang);
        }

        // ========================
        // GỬI EMAIL XÁC NHẬN ĐƠN HÀNG
        // ========================
        private void SendMail(string toEmail, string subject, string body)
        {
            try
            {
                var fromEmail = "krikshop.dacn.n2@gmail.com"; // ✅ Email cửa hàng
                var fromPassword = "cafl dmri cnuq zdqb";      // ✅ App password Gmail (KHÔNG phải mật khẩu thật)

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(fromEmail, "KRIK SHOP");
                    message.To.Add(toEmail);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential(fromEmail, fromPassword);
                        smtp.Send(message);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ Lỗi gửi mail: " + ex.Message);
            }
        }

    }
}