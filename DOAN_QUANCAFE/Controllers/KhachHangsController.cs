using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DOAN_QUANCAFE; // Namespace chứa Models

namespace DOAN_QUANCAFE.Controllers
{
    public class KhachHangsController : Controller
    {
        private QL_QuanCafeEntities db = new QL_QuanCafeEntities();

        // GET: Admin/KhachHangs
        public ActionResult Index()
        {
            return View(db.tblKhachHangs.ToList());
        }

        // GET: Admin/KhachHangs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            tblKhachHang tblKhachHang = db.tblKhachHangs.Find(id);
            if (tblKhachHang == null) return HttpNotFound();
            return View(tblKhachHang);
        }

        // GET: Admin/KhachHangs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/KhachHangs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaKH,TenKH,MatKhau,GioiTinh,NamSinh,Avarta,DienThoai,Email,DiaChi")] tblKhachHang tblKhachHang, HttpPostedFileBase uploadHinh)
        {
            if (ModelState.IsValid)
            {
                // Xử lý Upload Avatar
                if (uploadHinh != null && uploadHinh.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(uploadHinh.FileName);
                    // Lưu vào thư mục riêng cho khách hàng
                    string path = Path.Combine(Server.MapPath("~/Content/Images/Customers/"), fileName);

                    // Kiểm tra nếu thư mục chưa tồn tại thì tạo mới
                    if (!Directory.Exists(Server.MapPath("~/Content/Images/Customers/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/Images/Customers/"));
                    }

                    uploadHinh.SaveAs(path);
                    tblKhachHang.Avarta = fileName;
                }
                else
                {
                    tblKhachHang.Avarta = "default-user.jpg";
                }

                db.tblKhachHangs.Add(tblKhachHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblKhachHang);
        }

        // GET: Admin/KhachHangs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            tblKhachHang tblKhachHang = db.tblKhachHangs.Find(id);
            if (tblKhachHang == null) return HttpNotFound();
            return View(tblKhachHang);
        }

        // POST: Admin/KhachHangs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaKH,TenKH,MatKhau,GioiTinh,NamSinh,Avarta,DienThoai,Email,DiaChi")] tblKhachHang tblKhachHang, HttpPostedFileBase uploadHinh)
        {
            if (ModelState.IsValid)
            {
                var khachHangCu = db.tblKhachHangs.AsNoTracking().FirstOrDefault(x => x.MaKH == tblKhachHang.MaKH);

                if (uploadHinh != null && uploadHinh.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(uploadHinh.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Images/Customers/"), fileName);
                    uploadHinh.SaveAs(path);
                    tblKhachHang.Avarta = fileName;
                }
                else
                {
                    // Giữ ảnh cũ
                    tblKhachHang.Avarta = khachHangCu.Avarta;
                }

                db.Entry(tblKhachHang).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblKhachHang);
        }

        // GET: Admin/KhachHangs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            tblKhachHang tblKhachHang = db.tblKhachHangs.Find(id);
            if (tblKhachHang == null) return HttpNotFound();
            return View(tblKhachHang);
        }

        // POST: Admin/KhachHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblKhachHang tblKhachHang = db.tblKhachHangs.Find(id);

            // Nếu khách hàng đã có đơn hàng -> Không cho xóa (hoặc phải xóa đơn hàng trước)
            // Ở đây ta chọn giải pháp an toàn: Giữ lại data để tránh lỗi
            if (tblKhachHang.tblHoaDons.Any())
            {
                TempData["Error"] = "Không thể xóa khách hàng này vì họ đã có lịch sử mua hàng!";
                return RedirectToAction("Index");
            }

            db.tblKhachHangs.Remove(tblKhachHang);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult ThongTinTaiKhoan()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int maKH = (int)Session["UserId"];
            var kh = db.tblKhachHangs.Find(maKH);
            if (kh == null)
                return HttpNotFound();

            return View(kh);
        }

        // GET: KhachHangs/EditProfile (khách tự sửa chính mình)
        public ActionResult EditProfile()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int maKH = (int)Session["UserId"];
            var kh = db.tblKhachHangs.Find(maKH);
            if (kh == null)
                return HttpNotFound();

            return View("EditProfile", kh);
        }

        // POST: KhachHangs/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(tblKhachHang model, HttpPostedFileBase file)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int maKH = (int)Session["UserId"];
            var kh = db.tblKhachHangs.Find(maKH);
            if (kh == null)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                kh.TenKH = model.TenKH;
                kh.GioiTinh = model.GioiTinh;
                kh.NamSinh = model.NamSinh;
                kh.Email = model.Email;
                kh.DienThoai = model.DienThoai;
                kh.DiaChi = model.DiaChi;

                // Nếu có file ảnh mới
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Images/avt"), fileName);
                    file.SaveAs(path);
                    kh.Avarta = fileName;
                }

                db.SaveChanges();
                TempData["Success"] = "Cập nhật thành công!";
                return RedirectToAction("ThongTinTaiKhoan");
            }

            return View("EditProfile", kh);
        }


    }
}