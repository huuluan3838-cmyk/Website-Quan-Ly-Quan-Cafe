using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOAN_QUANCAFE.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        QL_QuanCafeEntities db = new QL_QuanCafeEntities();
        public ActionResult Index()
        {
            List<tblSanPham> lstSP = db.tblSanPhams.ToList();
            return View(lstSP);
        }
        // Action Xử lý Tìm kiếm Nâng cao
        [HttpGet]
        public ActionResult TimKiemNC(string sTenSP, string sTuKhoa, int? iMaLoai, string sMucGia)
        {
            // 1. Lấy tất cả sản phẩm
            var listSP = db.tblSanPhams.AsQueryable();

            // 2. Tìm theo Tên (nếu có nhập)
            if (!string.IsNullOrEmpty(sTenSP))
            {
                listSP = listSP.Where(n => n.TenSP.Contains(sTenSP));
            }

            // 3. Tìm theo Từ khóa mô tả (nếu có nhập)
            if (!string.IsNullOrEmpty(sTuKhoa))
            {
                listSP = listSP.Where(n => n.MoTa.Contains(sTuKhoa));
            }

            // 4. Tìm theo Loại (nếu có chọn)
            if (iMaLoai != null)
            {
                listSP = listSP.Where(n => n.LoaiSP == iMaLoai);
            }

            // 5. Tìm theo Mức giá (Dựa vào value 1, 2, 3, 4 ở View)
            if (!string.IsNullOrEmpty(sMucGia))
            {
                int gia = int.Parse(sMucGia);
                switch (gia)
                {
                    case 1: // 0 - 40k
                        listSP = listSP.Where(n => n.GiaBan <= 40000);
                        break;
                    case 2: // 41k - 60k
                        listSP = listSP.Where(n => n.GiaBan >= 41000 && n.GiaBan <= 60000);
                        break;
                    case 3: // 61k - 80k
                        listSP = listSP.Where(n => n.GiaBan >= 61000 && n.GiaBan <= 80000);
                        break;
                    case 4: // > 80k
                        listSP = listSP.Where(n => n.GiaBan > 80000);
                        break;
                }
            }

            // Trả về View kết quả (Tái sử dụng View SanPhamTheoLoai để hiển thị)
            ViewBag.TieuDe = "Kết quả tìm kiếm";
            return View("SanPhamTheoLoai", listSP.ToList());
        }
        public ActionResult SanPhamTheoLoai(int iMaLoai)
        {
            // 1. Lấy danh sách sản phẩm theo Mã Loại
            var listSP = db.tblSanPhams.Where(n => n.LoaiSP == iMaLoai).ToList();

            // 2. Lấy tên loại để hiển thị lên tiêu đề (Ví dụ: "Đang xem: Cà phê")
            var loai = db.tblLoaiSanPhams.Find(iMaLoai);
            ViewBag.TieuDe = loai != null ? loai.TenLoai : "Danh sách sản phẩm";

            // 3. Trả về View cùng danh sách sản phẩm
            return View(listSP);
        }
    }
}