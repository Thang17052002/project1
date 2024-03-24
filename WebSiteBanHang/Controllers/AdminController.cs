using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc;
using System.Web.Security;

namespace WebSiteBanHang.Controllers
{
    public class AdminController : Controller
    {
        private QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        public ActionResult Index()
        {
            // Kiểm tra đăng nhập trước khi truy cập trang admin
            if (Session["TaiKhoan"] == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["TaiKhoan"] != null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        public class LoginViewModel
        {
            public string TxtTenDangNhap { get; set; }
            public string TxtMatKhau { get; set; }
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string txtTenDangNhap = model.TxtTenDangNhap;
                string txtMatKhau = model.TxtMatKhau;

                ThanhVien tv = db.ThanhViens.SingleOrDefault(n => n.TaiKhoan == txtTenDangNhap && n.MatKhau == txtMatKhau);
                if (tv != null)
                {
                    var lstQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV == tv.MaLoaiTV).Select(n => n.Quyen.MaQuyen);

                    if (lstQuyen.Any())
                    {
                        string Quyen = string.Join(",", lstQuyen);
                        PhanQuyen(tv.TaiKhoan.ToString(), Quyen);
                        Session["TaiKhoan"] = tv.HoTen.ToString();
                        return RedirectToAction("Index");
                    }
                }

                return Content("Tài khoản hoặc mật khẩu không đúng!");
            }

            return Content("Vui lòng nhập đầy đủ thông tin đăng nhập!");
        }

        public void PhanQuyen(string TaiKhoan, string Quyen)
        {
            FormsAuthentication.Initialize();
            var ticket = new FormsAuthenticationTicket(1,
                                          TaiKhoan,
                                          DateTime.Now,
                                          DateTime.Now.AddHours(3),
                                          false,
                                          Quyen,
                                          FormsAuthentication.FormsCookiePath);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
            if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;
            Response.Cookies.Add(cookie);
        }
    }
}