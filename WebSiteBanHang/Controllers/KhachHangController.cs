using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Controllers
{
    [Authorize(Roles = "QuanTri,QuanLySanPham")]
    public class KhachHangController : Controller
    {
        private QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        [Authorize(Roles = "QuanLySanPham")]
        public ActionResult Index()
        {
            var lstKH = db.KhachHangs.ToList();
            return View(lstKH);
        }

        [Authorize(Roles = "QuanTri")]
        public ActionResult Index1()
        {
            var lstKH = db.KhachHangs.ToList();
            return View(lstKH);
        }

        public ActionResult TruyVan1DoiTuong()
        {
            var khachHang = db.KhachHangs.FirstOrDefault(kh => kh.MaKH == 2);
            return View(khachHang);
        }

        public ActionResult SortDuLieu()
        {
            var lstKH = db.KhachHangs.OrderByDescending(n => n.TenKH).ToList();
            return View(lstKH);
        }

        public ActionResult GroupDuLieu()
        {
            var lstTV = db.ThanhViens.OrderByDescending(n => n.TaiKhoan).ToList();
            return View(lstTV);
        }
    }
}
