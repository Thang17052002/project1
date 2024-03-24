using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Controllers
{
    [Authorize(Roles = "QuanTri,QuanLySanPham")]
    public class QuanLyDonHangController : Controller
    {
        private QuanLyBanHangEntities db = new QuanLyBanHangEntities();

        public ActionResult ChuaThanhToan()
        {
            var lst = db.DonDatHangs
     .Where(n => n.DaThanhToan != null && n.DaThanhToan.ToString() == "SomeValue")
     .OrderBy(n => n.NgayDat)
     .ToList();



            return View(lst);
        }

        public ActionResult ChuaGiao()
        {
            var lstDSDHCG = db.DonDatHangs.Where(n => n.TinhTrangGiaoHang == false && n.DaThanhToan == true).OrderBy(n => n.NgayGiao).ToList();

            return View(lstDSDHCG);
        }

        public ActionResult DaGiaoDaThanhToan()
        {
            var lstDSDHCG = db.DonDatHangs
            .Where(n => (n.TinhTrangGiaoHang ?? false) && (n.DaThanhToan ?? false))
            .OrderBy(n => n.NgayGiao)
            .ToList();

            return View(lstDSDHCG);
        }

        [HttpGet]
        public ActionResult DuyetDonHang(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DonDatHang model = db.DonDatHangs.SingleOrDefault(n => n.MaDDH == id);
            if (model == null)
            {
                return HttpNotFound();
            }

            var lstChiTietDH = db.ChiTietDonDatHangs.Where(n => n.MaDDH == id).ToList();
            ViewBag.ListChiTietDH = lstChiTietDH;
            return View(model);
        }

        [HttpPost]
        public ActionResult DuyetDonHang(DonDatHang ddh)
        {
            DonDatHang ddhUpdate = db.DonDatHangs.SingleOrDefault(n => n.MaDDH == ddh.MaDDH);
            if (ddhUpdate == null)
            {
                return HttpNotFound();
            }

            ddhUpdate.DaThanhToan = ddh.DaThanhToan;
            ddhUpdate.TinhTrangGiaoHang = ddh.TinhTrangGiaoHang;

            try
            {
                db.SaveChanges();
                SendConfirmationEmail("Xác nhận đơn hàng của hệ thống", "fanmuchanchinh@gmail.com", "ksshop.com.vn@gmail.com", "google123456", "Đơn hàng của bạn đã được đặt thành công!");
            }
            catch (Exception)
            {
                ViewBag.ThongBao = "Không thể cập nhật đơn hàng hoặc gửi email.";
                // Log the exception for further investigation
            }

            var lstChiTietDH = db.ChiTietDonDatHangs.Where(n => n.MaDDH == ddh.MaDDH).ToList();
            ViewBag.ListChiTietDH = lstChiTietDH;
            return View(ddhUpdate);
        }

        private void SendConfirmationEmail(string title, string toEmail, string fromEmail, string password, string content)
        {
            using (MailMessage mail = new MailMessage())
            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                mail.To.Add(toEmail);
                mail.From = new MailAddress(fromEmail);
                mail.Subject = title;
                mail.Body = content;
                mail.IsBodyHtml = true;

                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(fromEmail, password);
                smtp.EnableSsl = true;

                smtp.Send(mail);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
