using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO; // Thư viện để xử lý file ảnh
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DOAN_QUANCAFE; // Đảm bảo đúng namespace

namespace DOAN_QUANCAFE.Controllers
{
    [PhanQuyenAdmin]
    public class SanPhamsController : Controller
    {
        private QL_QuanCafeEntities db = new QL_QuanCafeEntities();

        // GET: Admin/SanPhams
        public ActionResult Index()
        {
            var tblSanPhams = db.tblSanPhams.Include(t => t.tblLoaiSanPham).Include(t => t.tblNhaCungCap);
            return View(tblSanPhams.ToList());
        }

        // GET: Admin/SanPhams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            tblSanPham tblSanPham = db.tblSanPhams.Find(id);
            if (tblSanPham == null) return HttpNotFound();
            return View(tblSanPham);
        }

        // GET: Admin/SanPhams/Create
        public ActionResult Create()
        {
            ViewBag.LoaiSP = new SelectList(db.tblLoaiSanPhams, "MaLoai", "TenLoai");
            ViewBag.NhaCungCap = new SelectList(db.tblNhaCungCaps, "MaNCC", "TenNCC");
            return View();
        }

        // POST: Admin/SanPhams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Thêm tham số uploadHinh để nhận file từ View
        public ActionResult Create([Bind(Include = "MaSP,TenSP,KichThuoc,GiaBan,MoTa,LoaiSP,NhaCungCap")] tblSanPham tblSanPham, HttpPostedFileBase uploadHinh)
        {
            if (ModelState.IsValid)
            {
                // Xử lý Upload Hình Ảnh
                if (uploadHinh != null && uploadHinh.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(uploadHinh.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Images/Products/"), fileName);
                    uploadHinh.SaveAs(path);

                    // Lưu tên file vào Database
                    tblSanPham.HinhAnh = fileName;
                }
                else
                {
                    // Ảnh mặc định nếu không upload
                    tblSanPham.HinhAnh = "placeholder.jpg";
                }

                db.tblSanPhams.Add(tblSanPham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LoaiSP = new SelectList(db.tblLoaiSanPhams, "MaLoai", "TenLoai", tblSanPham.LoaiSP);
            ViewBag.NhaCungCap = new SelectList(db.tblNhaCungCaps, "MaNCC", "TenNCC", tblSanPham.NhaCungCap);
            return View(tblSanPham);
        }

        // GET: Admin/SanPhams/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            tblSanPham tblSanPham = db.tblSanPhams.Find(id);
            if (tblSanPham == null) return HttpNotFound();

            ViewBag.LoaiSP = new SelectList(db.tblLoaiSanPhams, "MaLoai", "TenLoai", tblSanPham.LoaiSP);
            ViewBag.NhaCungCap = new SelectList(db.tblNhaCungCaps, "MaNCC", "TenNCC", tblSanPham.NhaCungCap);
            return View(tblSanPham);
        }

        // POST: Admin/SanPhams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaSP,TenSP,KichThuoc,GiaBan,MoTa,HinhAnh,LoaiSP,NhaCungCap")] tblSanPham tblSanPham, HttpPostedFileBase uploadHinh)
        {
            if (ModelState.IsValid)
            {
                // Tìm sản phẩm cũ để kiểm tra
                var sanPhamCu = db.tblSanPhams.AsNoTracking().FirstOrDefault(x => x.MaSP == tblSanPham.MaSP);

                if (uploadHinh != null && uploadHinh.ContentLength > 0)
                {
                    // Nếu có chọn ảnh mới -> Lưu ảnh mới
                    string fileName = Path.GetFileName(uploadHinh.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Images/Products/"), fileName);
                    uploadHinh.SaveAs(path);
                    tblSanPham.HinhAnh = fileName;
                }
                else
                {
                    // Nếu không chọn ảnh mới -> Giữ nguyên ảnh cũ
                    tblSanPham.HinhAnh = sanPhamCu.HinhAnh;
                }

                db.Entry(tblSanPham).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LoaiSP = new SelectList(db.tblLoaiSanPhams, "MaLoai", "TenLoai", tblSanPham.LoaiSP);
            ViewBag.NhaCungCap = new SelectList(db.tblNhaCungCaps, "MaNCC", "TenNCC", tblSanPham.NhaCungCap);
            return View(tblSanPham);
        }

        // GET: Admin/SanPhams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            tblSanPham tblSanPham = db.tblSanPhams.Find(id);
            if (tblSanPham == null) return HttpNotFound();
            return View(tblSanPham);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = db.tblSanPhams.Find(id);
            if (product == null) return RedirectToAction("Index");

            bool daCoDonHang = db.tblChiTietHoaDons.Any(ct => ct.MaSP == id);
            if (daCoDonHang)
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm này vì đã có đơn hàng liên quan!";
                return RedirectToAction("Delete", new { id });
            }

            try
            {
                var imgs = db.tblHinhAnhs.Where(x => x.MaSP == id).ToList();
                if (imgs.Any()) db.tblHinhAnhs.RemoveRange(imgs);

                var cmts = db.BinhLuans.Where(x => x.MaSP == id).ToList();
                if (cmts.Any()) db.BinhLuans.RemoveRange(cmts);

                db.tblSanPhams.Remove(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (System.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                TempData["ErrorMessage"] = "Không thể xóa vì còn dữ liệu liên quan (hình ảnh/bình luận/chi tiết hóa đơn).";
                return RedirectToAction("Delete", new { id });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BulkDelete(int[] selectedIds)
        {
            if (selectedIds == null || selectedIds.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một sản phẩm để xóa!";
                return RedirectToAction("Index");
            }

            int deletedCount = 0;
            int errorCount = 0;

            foreach (var id in selectedIds)
            {
                var product = db.tblSanPhams.Find(id);
                if (product == null) continue;

                // ĐÚNG: sản phẩm có trong chi tiết hóa đơn thì không xóa
                bool daCoDonHang = db.tblChiTietHoaDons.Any(ct => ct.MaSP == id);
                if (daCoDonHang)
                {
                    errorCount++;
                    continue;
                }

                try
                {
                    // XÓA BẢNG CON TRƯỚC (FK MaSP)
                    var imgs = db.tblHinhAnhs.Where(x => x.MaSP == id).ToList();
                    if (imgs.Any()) db.tblHinhAnhs.RemoveRange(imgs);

                    // Nếu có bảng bình luận tham chiếu MaSP thì xóa luôn
                    var cmts = db.BinhLuans.Where(x => x.MaSP == id).ToList();
                    if (cmts.Any()) db.BinhLuans.RemoveRange(cmts);

                    db.tblSanPhams.Remove(product);
                    db.SaveChanges(); // lưu từng sản phẩm để dễ xác định cái nào lỗi

                    deletedCount++;
                }
                catch (System.Data.SqlClient.SqlException ex) when (ex.Number == 547) // FK constraint
                {
                    errorCount++;
                    // bỏ thay đổi đang track cho entity này để tránh lỗi dây chuyền
                    db.Entry(product).State = System.Data.Entity.EntityState.Unchanged;
                }
            }

            TempData["ErrorMessage"] = $"Đã xóa {deletedCount} sản phẩm. {errorCount} sản phẩm không thể xóa do có ràng buộc dữ liệu!";
            return RedirectToAction("Index");
        }
    }
}