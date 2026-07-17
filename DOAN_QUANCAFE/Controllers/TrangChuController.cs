using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOAN_QUANCAFE.Controllers
{
    public class TrangChuController : Controller
    {
        // GET: TrangChu
        QL_QuanCafeEntities db = new QL_QuanCafeEntities();
        public ActionResult Index(int? id) // id này nhận từ URL như /TrangChu/Index/1
        {
            if (id != null)
            {
                // Thực hiện lọc sản phẩm theo mã loại
                List<tblSanPham> lstSP = db.tblSanPhams.Where(s => s.LoaiSP == id).ToList();

                // Lấy tên loại để hiển thị tiêu đề trang cho pro
                var loai = db.tblLoaiSanPhams.Find(id);
                ViewBag.TenLoai = loai != null ? loai.TenLoai : "Sản phẩm";

                return View(lstSP);
            }

            // Nếu id trống, hiện tất cả sản phẩm
            ViewBag.TenLoai = "Tất cả sản phẩm";
            return View(db.tblSanPhams.ToList());
        }
        public ActionResult ChiTietSP(int masp)
        {
            
            var sanpham = db.tblSanPhams.FirstOrDefault(s => s.MaSP == masp);
            List<tblSanPham> lienquan = db.tblSanPhams.Where(s => s.LoaiSP == sanpham.LoaiSP && s.MaSP != masp).ToList();
            ViewBag.LQ = lienquan;
            //sản phẩm cung nxb
            List<tblSanPham> ncc = db.tblSanPhams.Where(s => s.NhaCungCap == sanpham.NhaCungCap && s.MaSP != masp).ToList();
            ViewBag.NCC = ncc;
            List<BinhLuan> binhluan = db.BinhLuans
                 .Where(b => b.MaSP == masp)
                 .OrderByDescending(b => b.Ngay)
                 .ToList();
            ViewBag.BinhLuan = binhluan;
            return View(sanpham);
        }
        public ActionResult LocSP(int idloc, int type)
        {
            List<tblSanPham> lienquan = new List<tblSanPham>();
            if (type == 1) lienquan = db.tblSanPhams.Where(s => s.LoaiSP == idloc).ToList();
            else if (type == 2) lienquan = db.tblSanPhams.Where(s => s.NhaCungCap == idloc).ToList();
            return View("Index", lienquan);
        }
        [HttpPost]
        public ActionResult GuiBinhLuan(int maSP, string noiDung, int soSao = 5)
        {
            // 1. Kiểm tra đăng nhập
            if (Session["UserName"] == null)
            {
                // Nếu chưa đăng nhập, trả về trang đăng nhập hoặc thông báo lỗi
                return RedirectToAction("Login", "Account");
            }

            // 2. Kiểm tra nội dung rỗng
            if (!string.IsNullOrEmpty(noiDung))
            {
                BinhLuan bl = new BinhLuan();
                bl.MaSP = maSP;
                bl.NoiDung = noiDung;
                bl.SoSao = soSao;
                bl.Ngay = DateTime.Now;

                // Lấy tên từ Session (Lúc này chắc chắn đã có vì đã check ở trên)
                bl.HoTen = Session["UserName"].ToString();

                db.BinhLuans.Add(bl);
                db.SaveChanges();
            }

            return RedirectToAction("ChiTietSP", new { masp = maSP });
        }
    }
}