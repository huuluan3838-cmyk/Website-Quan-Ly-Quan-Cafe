using System;
using System.Linq;
using System.Web.Mvc;
using DOAN_QUANCAFE;

namespace DOAN_QUANCAFE.Controllers
{
    [PhanQuyenAdmin] // Giữ nguyên phân quyền của bạn
    public class AdminController : Controller
    {
        private QL_QuanCafeEntities db = new QL_QuanCafeEntities();

        // GET: Admin Dashboard
        public ActionResult Index()
        {
            // 1. Tổng số khách hàng
            ViewBag.SoLuongNguoiDung = db.tblKhachHangs.Count();

            // 2. Tổng số sản phẩm
            ViewBag.SoLuongSanPham = db.tblSanPhams.Count();

            // 3. Tổng số đơn hàng (Trừ đơn đã hủy để số liệu đẹp hơn, hoặc giữ nguyên tùy bạn)
            ViewBag.SoLuongDonHang = db.tblHoaDons.Count();
            // 4. Tổng doanh thu từ các đơn đã thanh toán (Trừ đơn hủy)
            decimal tongDoanhThu = db.tblHoaDons
                .Where(n => n.DaThanhToan == true && n.TinhTrang != 3)
                .Sum(n => (decimal?)n.TongTien) ?? 0;

            ViewBag.TongDoanhThu = tongDoanhThu;

            return View();
        }

        // ==================================================================
        // THÊM HÀM HỦY ĐƠN HÀNG
        // Hàm này dùng để gọi từ trang Quản lý đơn hàng khi bấm nút "Hủy"
        // ==================================================================
        public ActionResult HuyDon(int id)
        {
            // Tìm đơn hàng theo ID
            var dh = db.tblHoaDons.Find(id);
            if (dh != null)
            {
                // Cập nhật trạng thái thành 3 (Đã hủy)
                dh.TinhTrang = 3;

                db.SaveChanges();
            }

            return RedirectToAction("DonHang");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}