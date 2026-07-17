using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOAN_QUANCAFE.Controllers
{
    public class DanhMucController : Controller
    {
        // GET: DanhMuc
        QL_QuanCafeEntities db = new QL_QuanCafeEntities();
        public ActionResult _DanhMuc()
        {
            return PartialView();
        }
        public ActionResult _DanhMucTL()
        {
            List<tblLoaiSanPham>   lstLoaiSP = db.tblLoaiSanPhams.ToList();
            return PartialView(lstLoaiSP);
        }
        public ActionResult _DanhMucNCC()
        {
            List<tblNhaCungCap> lstLoaiNCC = db.tblNhaCungCaps.ToList();
            return PartialView(lstLoaiNCC);
        }
        public ActionResult _TimKiemNC()
        {
            List<tblLoaiSanPham> lstLoaiSP = db.tblLoaiSanPhams.ToList();
            return PartialView(lstLoaiSP);
        }
    }
}