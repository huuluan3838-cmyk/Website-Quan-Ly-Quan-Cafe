using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DOAN_QUANCAFE;

namespace DOAN_QUANCAFE.Controllers
{
    [PhanQuyenAdmin]
    public class DonHangsController : Controller
    {
        private QL_QuanCafeEntities db = new QL_QuanCafeEntities();

        // GET: DonHangs
        public ActionResult Index()
        {
            var tblHoaDons = db.tblHoaDons.Include(t => t.tblKhachHang).Include(t => t.tblNhanVien).Include(t => t.tblTinhTrang);
            return View(tblHoaDons.ToList());
        }

        // GET: DonHangs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHoaDon tblHoaDon = db.tblHoaDons.Find(id);
            if (tblHoaDon == null)
            {
                return HttpNotFound();
            }
            return View(tblHoaDon);
        }

        // GET: DonHangs/Create
        public ActionResult Create()
        {
            ViewBag.MaKH = new SelectList(db.tblKhachHangs, "MaKH", "TenKH");
            ViewBag.MaNV = new SelectList(db.tblNhanViens, "MaNV", "MatKhau");
            ViewBag.TinhTrang = new SelectList(db.tblTinhTrangs, "ID", "TinhTrangHoaDon");
            return View();
        }

        // POST: DonHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaHD,MaKH,MaNV,NgayLap,TongTien,TinhTrang,DiaChiGiaoHang,DaThanhToan")] tblHoaDon tblHoaDon)
        {
            if (ModelState.IsValid)
            {
                db.tblHoaDons.Add(tblHoaDon);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaKH = new SelectList(db.tblKhachHangs, "MaKH", "TenKH", tblHoaDon.MaKH);
            ViewBag.MaNV = new SelectList(db.tblNhanViens, "MaNV", "MatKhau", tblHoaDon.MaNV);
            ViewBag.TinhTrang = new SelectList(db.tblTinhTrangs, "ID", "TinhTrangHoaDon", tblHoaDon.TinhTrang);
            return View(tblHoaDon);
        }

        // GET: DonHangs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHoaDon tblHoaDon = db.tblHoaDons.Find(id);
            if (tblHoaDon == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaKH = new SelectList(db.tblKhachHangs, "MaKH", "TenKH", tblHoaDon.MaKH);
            ViewBag.MaNV = new SelectList(db.tblNhanViens, "MaNV", "MatKhau", tblHoaDon.MaNV);
            ViewBag.TinhTrang = new SelectList(db.tblTinhTrangs, "ID", "TinhTrangHoaDon", tblHoaDon.TinhTrang);
            return View(tblHoaDon);
        }

        // POST: DonHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaHD,MaKH,MaNV,NgayLap,TongTien,TinhTrang,DiaChiGiaoHang,DaThanhToan")] tblHoaDon tblHoaDon)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblHoaDon).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaKH = new SelectList(db.tblKhachHangs, "MaKH", "TenKH", tblHoaDon.MaKH);
            ViewBag.MaNV = new SelectList(db.tblNhanViens, "MaNV", "MatKhau", tblHoaDon.MaNV);
            ViewBag.TinhTrang = new SelectList(db.tblTinhTrangs, "ID", "TinhTrangHoaDon", tblHoaDon.TinhTrang);
            return View(tblHoaDon);
        }

        // GET: DonHangs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblHoaDon tblHoaDon = db.tblHoaDons.Find(id);
            if (tblHoaDon == null)
            {
                return HttpNotFound();
            }
            return View(tblHoaDon);
        }

        // POST: Admin/DonHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblHoaDon tblHoaDon = db.tblHoaDons.Find(id);

            // Xóa tất cả chi tiết hóa đơn của đơn này trước
            // Nếu không làm bước này, SQL sẽ báo lỗi khóa ngoại (Foreign Key)
            var chiTiet = db.tblChiTietHoaDons.Where(c => c.MaHD == id).ToList();
            db.tblChiTietHoaDons.RemoveRange(chiTiet);

            // Sau đó mới xóa hóa đơn
            db.tblHoaDons.Remove(tblHoaDon);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // Action Hủy đơn hàng
        public ActionResult HuyDon(int id)
        {
            // Tìm đơn hàng
            var dh = db.tblHoaDons.Find(id);
            if (dh != null)
            {
                // Cập nhật trạng thái thành 3 (Đã hủy)
                dh.TinhTrang = 3;

                db.SaveChanges();
            }
            return RedirectToAction("Index"); // Quay lại danh sách đơn hàng
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
