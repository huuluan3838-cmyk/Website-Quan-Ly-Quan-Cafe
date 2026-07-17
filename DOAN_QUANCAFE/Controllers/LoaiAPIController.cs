using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DOAN_QUANCAFE; // Đảm bảo namespace này khớp với project của bạn

namespace DOAN_QUANCAFE.Controllers
{
    public class LoaiAPIController : Controller
    {
        private QL_QuanCafeEntities db = new QL_QuanCafeEntities();
        public ActionResult Index()
        {
            return View();
        }
        // 1. HIỂN THỊ DANH SÁCH (Get All)
        [HttpGet]
        public JsonResult GetAll()
        {
            var data = db.tblLoaiSanPhams.Select(x => new {
                x.MaLoai,
                x.TenLoai,
                x.GhiChu,
                SoLuongMon = x.tblSanPhams.Count
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // 2. LẤY CHI TIẾT 1 LOẠI (Để đổ dữ liệu vào ô Sửa)
        [HttpGet]
        public JsonResult GetById(int id)
        {
            var loai = db.tblLoaiSanPhams.Where(x => x.MaLoai == id).Select(x => new {
                x.MaLoai,
                x.TenLoai,
                x.GhiChu
            }).FirstOrDefault();
            return Json(loai, JsonRequestBehavior.AllowGet);
        }

        // 3. THÊM HOẶC SỬA (Lưu dữ liệu)
        [HttpPost]
        public JsonResult Save(int id, string tenLoai, string ghiChu)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenLoai))
                {
                    return Json(new { success = false, message = "Tên loại không được để trống!" });
                }

                // Chuẩn hóa tên (trim + lowercase)
                string tenLoaiCheck = tenLoai.Trim().ToLower();

                // KIỂM TRA TRÙNG TÊN
                bool isDuplicate = db.tblLoaiSanPhams
                    .Any(x => x.TenLoai.Trim().ToLower() == tenLoaiCheck
                           && x.MaLoai != id); // loại trừ chính nó khi sửa

                if (isDuplicate)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Tên loại sản phẩm đã tồn tại!"
                    });
                }

                if (id == 0)
                {
                    // 👉 THÊM MỚI
                    var loai = new tblLoaiSanPham
                    {
                        TenLoai = tenLoai.Trim(),
                        GhiChu = ghiChu
                    };

                    db.tblLoaiSanPhams.Add(loai);
                    db.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        message = "Thêm loại sản phẩm thành công!"
                    });
                }
                else
                {
                    // 👉 CẬP NHẬT
                    var loai = db.tblLoaiSanPhams.Find(id);
                    if (loai == null)
                        return Json(new { success = false, message = "Không tìm thấy dữ liệu!" });

                    loai.TenLoai = tenLoai.Trim();
                    loai.GhiChu = ghiChu;
                    db.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        message = "Cập nhật loại sản phẩm thành công!"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Lỗi hệ thống: " + ex.Message
                });
            }
        }


        // 4. XÓA (Delete)
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var loai = db.tblLoaiSanPhams.Find(id);
            if (loai == null) return Json(new { success = false, message = "Dữ liệu không tồn tại!" });

            // Kiểm tra ràng buộc khóa ngoại trước khi xóa
            var hasProduct = db.tblSanPhams.Any(x => x.LoaiSP == id);
            if (hasProduct)
                return Json(new { success = false, message = "Không thể xóa vì đang có sản phẩm thuộc loại này!" });

            db.tblLoaiSanPhams.Remove(loai);
            db.SaveChanges();
            return Json(new { success = true, message = "Xóa loại sản phẩm thành công!" });
        }
    }
}