using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOAN_QUANCAFE.Models
{
    public class GioHangItem
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public string HinhAnh { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }

        public decimal ThanhTien => SoLuong * GiaBan;
    }


}