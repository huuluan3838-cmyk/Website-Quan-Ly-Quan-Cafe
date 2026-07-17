using System;
using System.Web.Mvc;

namespace DOAN_QUANCAFE.Controllers
{
    public class LienHeController : Controller
    {
        private QL_QuanCafeEntities db = new QL_QuanCafeEntities();

        // GET: LienHe
        public ActionResult Index()
        {
            if (Session["UserId"] != null)
            {
                int maKH = (int)Session["UserId"];
                ViewBag.KhachHang = db.tblKhachHangs.Find(maKH);
            }

            return View();
        }



        // POST: LienHe/SendContact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendContact(string Message)
        {
            if (Session["UserId"] == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để gửi liên hệ.";
                return RedirectToAction("Index");
            }

            int maKH = (int)Session["UserId"];
            var kh = db.tblKhachHangs.Find(maKH);

            if (string.IsNullOrWhiteSpace(Message))
            {
                TempData["Error"] = "Vui lòng nhập nội dung.";
                return RedirectToAction("Index");
            }

            var lh = new LienHe
            {
                HoTen = kh.TenKH,
                EmailOrPhone = kh.Email ?? kh.DienThoai,
                NoiDung = Message,
                NgayGui = DateTime.Now
            };

            db.LienHes.Add(lh);
            db.SaveChanges();

            TempData["Success"] = "✔ Gửi liên hệ thành công!";
            return RedirectToAction("Index");
        }

    }
}
