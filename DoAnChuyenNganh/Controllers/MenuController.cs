using DoAnChuyenNganh.Models;
using DoAnChuyenNganh.Models.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnChuyenNganh.Controllers
{
    public class MenuController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MenuTop()
        {
            // Lấy tất cả danh mục cha (DanhMucChaId = null)
            var danhMucCha = db.DanhMucs
                       .Where(dm => dm.DanhMucChaId == null && dm.TrangThai == true)
                       .Include(dm=>dm.DanhMuc1)//load con
                       .OrderBy(dm => dm.TenDanhMuc)
                       .ToList();

         

            return PartialView("_MenuTop", danhMucCha);
            //var items = db.DanhMucs.OrderBy(x => x.DanhMucId).ToList();
            //return PartialView("_MenuTop", items);
        }

        public ActionResult MenuDanhMucSanPham()
        {
            // Lấy tất cả danh mục cha (DanhMucChaId = null)
            var danhMucCha = db.DanhMucs
                       .Where(dm => dm.DanhMucChaId == null && dm.TrangThai == true)
                       .Include(dm => dm.DanhMuc1)//load con
                       .OrderBy(dm => dm.TenDanhMuc)
                       .ToList();
            return PartialView("_MenuDanhMucSanPham", danhMucCha);
        }
        public ActionResult MenuLeft(string slug, int? id)
        {
            List<DanhMuc> danhMucs = new List<DanhMuc>();
            if( id != null)
            {
                ViewBag.DanhMucId = id;
            }    
            if (!string.IsNullOrEmpty(slug))
            {
                // Tìm danh mục hiện tại
                var currentCate = db.DanhMucs.FirstOrDefault(x => x.Slug == slug && x.TrangThai == true);

                if (currentCate != null)
                {
                    // Nếu là danh mục con → lấy danh mục cha
                    var parentId = currentCate.DanhMucChaId ?? currentCate.DanhMucId;

                    // Lấy danh mục con của danh mục cha
                    danhMucs = db.DanhMucs
                                 .Where(x => x.DanhMucChaId == parentId && x.TrangThai == true)
                                 .OrderBy(x => x.TenDanhMuc)
                                 .ToList();
                }
            }

            // Nếu slug rỗng → hiển thị danh mục cha mặc định
            if (danhMucs == null || !danhMucs.Any())
            {
                danhMucs = db.DanhMucs
                             .Where(x => x.DanhMucChaId == null && x.TrangThai == true)
                             .OrderBy(x => x.TenDanhMuc)
                             .ToList();
            }

            // Truyền thêm slug để đánh dấu danh mục đang chọn
            ViewBag.CurrentSlug = slug;

            return PartialView("_MenuLeft", danhMucs);
        }
        public ActionResult MenuArrivals()
        {

            // Lấy tất cả danh mục cha (DanhMucChaId = null)
            var danhMucCha = db.DanhMucs
                       .Where(dm => dm.DanhMucChaId == null && dm.TrangThai == true)
                       .Include(dm => dm.DanhMuc1)//load con
                       .OrderBy(dm => dm.TenDanhMuc)
                       .ToList();
            return PartialView("_MenuArrivals", danhMucCha);
        }
    }
}