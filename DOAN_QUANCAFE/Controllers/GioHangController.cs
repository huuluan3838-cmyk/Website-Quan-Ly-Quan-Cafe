using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using DOAN_QUANCAFE.Models;

namespace DOAN_QUANCAFE.Controllers
{
    public class GioHangController : Controller
    {
        QL_QuanCafeEntities db = new QL_QuanCafeEntities();

        // Cấu hình ngân hàng
        private const string BANK_ID = "MBBank";
        private const string ACCOUNT_NO = "210524456";
        private const string TEMPLATE = "compact";

        // Shipping
        private const decimal DEFAULT_SHIPPING_FEE = 20000m;
        private const decimal FREE_SHIP_THRESHOLD = 120000m;

        // Coupon session keys
        private const string SESSION_COUPON_CODE = "CouponCode";
        private const string SESSION_DISCOUNT_AMOUNT = "DiscountAmount";

        private List<GioHangItem> GetCart()
        {
            if (Session["Cart"] == null)
                Session["Cart"] = new List<GioHangItem>();
            return (List<GioHangItem>)Session["Cart"];
        }

        private decimal GetShippingFee(decimal subTotal)
        {
            return (subTotal >= FREE_SHIP_THRESHOLD) ? 0m : DEFAULT_SHIPPING_FEE;
        }

        private string GetCouponCode()
        {
            return (Session[SESSION_COUPON_CODE] as string) ?? "";
        }

        private decimal GetDiscountAmount()
        {
            return (Session[SESSION_DISCOUNT_AMOUNT] is decimal d) ? d : 0m;
        }

        private void ClearCoupon()
        {
            Session[SESSION_COUPON_CODE] = null;
            Session[SESSION_DISCOUNT_AMOUNT] = null;
        }

        private string NormalizeCoupon(string code)
        {
            return (code ?? "").Trim().ToUpperInvariant();
        }

        // SALE10: giảm 10%, đơn tối thiểu 100.000, tối đa 50.000
        // SALE25: giảm 25%, đơn tối thiểu 200.000 tối đa 100.000
        // FREESHIP: không dùng nữa (ship tự động miễn phí nếu đơn >= 120.000)
        private bool TryCalcDiscount(string couponCode, decimal subTotal, out decimal discount, out string error)
        {
            discount = 0m;
            error = "";

            string code = NormalizeCoupon(couponCode);
            if (string.IsNullOrWhiteSpace(code))
            {
                error = "Mã ưu đãi không hợp lệ.";
                return false;
            }

            if (subTotal <= 0m)
            {
                error = "Giỏ hàng trống.";
                return false;
            }

            if (code == "SALE10")
            {
                if (subTotal < 100000m)
                {
                    error = "Đơn tối thiểu 100.000 để dùng SALE10.";
                    return false;
                }

                discount = Math.Floor(subTotal * 0.10m);
                if (discount > 50000m) discount = 50000m;
                return discount > 0m;
            }

            if (code == "SALE25")
            {
                if (subTotal < 200000m)
                {
                    error = "Đơn tối thiểu 200.000 để dùng SALE25.";
                    return false;
                }

                discount = Math.Floor(subTotal * 0.25m);
                if (discount > 100000m) discount = 100000m;
                return discount > 0m;
            }

            error = "Mã ưu đãi không đúng.";
            return false;
        }

        private decimal CalcGrandTotal(decimal subTotal, decimal shippingFee, decimal discount)
        {
            if (discount < 0m) discount = 0m;

            decimal total = subTotal + shippingFee - discount;
            if (total < 0m) total = 0m;

            return total;
        }

        // =========================
        // CART
        // =========================
        public ActionResult Index()
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index", "TrangChu");
            return View(cart);
        }

        public ActionResult AddToCart(int masp)
        {
            var cart = GetCart();
            var sp = db.tblSanPhams.Find(masp);
            if (sp == null) return RedirectToAction("Index", "TrangChu");

            var item = cart.FirstOrDefault(x => x.MaSP == masp);
            if (item != null)
                item.SoLuong++;
            else
                cart.Add(new GioHangItem
                {
                    MaSP = masp,
                    TenSP = sp.TenSP,
                    HinhAnh = sp.HinhAnh,
                    GiaBan = sp.GiaBan ?? 0,
                    SoLuong = 1
                });

            return RedirectToAction("Index", "TrangChu");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MaSP == id);
            if (item != null && quantity > 0)
                item.SoLuong = quantity;
            return RedirectToAction("Index");
        }

        public ActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MaSP == id);
            if (item != null) cart.Remove(item);
            return RedirectToAction("Index");
        }

        // =========================
        // COUPON APPLY / REMOVE
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyCoupon(string couponCode)
        {
            var cart = GetCart();
            if (cart == null || !cart.Any())
            {
                ClearCoupon();
                TempData["Error"] = "Giỏ hàng trống.";
                return RedirectToAction("DatHang");
            }

            decimal subTotal = cart.Sum(x => x.ThanhTien);

            if (TryCalcDiscount(couponCode, subTotal, out decimal discount, out string err))
            {
                Session[SESSION_COUPON_CODE] = NormalizeCoupon(couponCode);
                Session[SESSION_DISCOUNT_AMOUNT] = discount;
                TempData["Success"] = "Áp dụng mã ưu đãi thành công.";
            }
            else
            {
                ClearCoupon();
                TempData["Error"] = err;
            }

            return RedirectToAction("DatHang");
        }

        public ActionResult RemoveCoupon()
        {
            ClearCoupon();
            return RedirectToAction("DatHang");
        }

        // ==========================================
        // 1. GET: DatHang
        // ==========================================
        public ActionResult DatHang()
        {
            if (Session["User"] == null || Session["Role"].ToString() != "Customer")
            {
                TempData["Error"] = "Bạn cần đăng nhập để tiếp tục đặt hàng.";
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("DatHang", "GioHang") });
            }

            var cart = GetCart();
            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "TrangChu");

            decimal subTotal = cart.Sum(x => x.ThanhTien);

            // Ship tự động: >=120k thì 0, ngược lại 20k
            decimal shippingFee = GetShippingFee(subTotal);

            // Validate coupon đang lưu trong session theo subtotal hiện tại
            string code = GetCouponCode();
            decimal discount = GetDiscountAmount();

            if (!string.IsNullOrWhiteSpace(code))
            {
                if (TryCalcDiscount(code, subTotal, out decimal d2, out _))
                {
                    discount = d2;
                    Session[SESSION_DISCOUNT_AMOUNT] = discount;
                }
                else
                {
                    ClearCoupon();
                    code = "";
                    discount = 0m;
                }
            }

            ViewBag.SubTotal = subTotal;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.CouponCode = code;
            ViewBag.DiscountAmount = discount;
            ViewBag.GrandTotal = CalcGrandTotal(subTotal, shippingFee, discount);

            return View(cart);
        }

        // ==========================================
        // 2. POST: DatHang
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DatHang(string hoten, string email, string diachi, string sdt, string ghichu, string phuongthuc)
        {
            if (Session["User"] == null || Session["Role"].ToString() != "Customer")
                return RedirectToAction("Login", "Account");

            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index", "TrangChu");

            var user = (tblKhachHang)Session["User"];

            decimal subTotal = cart.Sum(x => x.ThanhTien);

            // Ship tự động: >=120k thì 0, ngược lại 20k
            decimal shippingFee = GetShippingFee(subTotal);

            // Coupon
            string code = GetCouponCode();
            decimal discount = 0m;

            if (!string.IsNullOrWhiteSpace(code))
            {
                if (TryCalcDiscount(code, subTotal, out decimal d2, out _))
                {
                    discount = d2;
                    Session[SESSION_DISCOUNT_AMOUNT] = discount;
                }
                else
                {
                    ClearCoupon();
                    discount = 0m;
                }
            }

            decimal grandTotal = CalcGrandTotal(subTotal, shippingFee, discount);

            // Tạo hóa đơn mới
            var hd = new tblHoaDon
            {
                MaKH = user.MaKH,
                NgayLap = DateTime.Now,
                TinhTrang = 1, // Chờ xử lý
                DaThanhToan = false,
                DiaChiGiaoHang = diachi,
                TongTien = grandTotal
            };
            db.tblHoaDons.Add(hd);
            db.SaveChanges();

            // Lưu chi tiết hóa đơn
            foreach (var item in cart)
            {
                db.tblChiTietHoaDons.Add(new tblChiTietHoaDon
                {
                    MaHD = hd.MaHD,
                    MaSP = item.MaSP,
                    SoLuong = item.SoLuong,
                    GiaBan = item.GiaBan
                });
            }
            db.SaveChanges();

            if (phuongthuc == "2")
            {
                // Chuyển khoản -> QR
                return RedirectToAction("Payment", new { id = hd.MaHD });
            }

            // Tiền mặt
            SendOrderConfirmationEmail(
                email, hoten, sdt, diachi, cart,
                "Tiền mặt (COD)",
                shippingFee, discount, grandTotal, code
            );

            Session["Cart"] = null;
            ClearCoupon();
            return View("DatHangThanhCong");
        }

        // ==========================================
        // TRANG HIỂN THỊ MÃ QR
        // ==========================================
        public ActionResult Payment(int id)
        {
            var hd = db.tblHoaDons.Find(id);
            if (hd == null) return HttpNotFound();

            string noiDungCK = "THANHTOAN HD" + id;
            var qrUrl = $"https://img.vietqr.io/image/{BANK_ID}-{ACCOUNT_NO}-{TEMPLATE}.png?amount={hd.TongTien}&addInfo={noiDungCK}";

            ViewBag.QrImage = qrUrl;
            ViewBag.OrderInfo = hd;
            return View();
        }

        // ==========================================
        // XÁC NHẬN ĐÃ THANH TOÁN
        // ==========================================
        public ActionResult ConfirmPayment(int id)
        {
            var hd = db.tblHoaDons.Find(id);
            if (hd == null) return HttpNotFound();

            hd.DaThanhToan = true;
            hd.TinhTrang = 1;
            db.SaveChanges();

            var kh = db.tblKhachHangs.Find(hd.MaKH);

            var details = db.tblChiTietHoaDons.Where(x => x.MaHD == id).ToList();
            List<GioHangItem> cartFake = new List<GioHangItem>();
            foreach (var d in details)
            {
                var sp = db.tblSanPhams.Find(d.MaSP);
                cartFake.Add(new GioHangItem
                {
                    TenSP = sp != null ? sp.TenSP : "",
                    GiaBan = d.GiaBan ?? 0,
                    SoLuong = d.SoLuong ?? 0
                });
            }

            // Không đủ dữ liệu coupon/ship/discount từ DB hiện tại (bạn chỉ lưu TongTien) -> gửi tổng tiền
            if (kh != null)
            {
                SendOrderConfirmationEmail(
                    kh.Email, kh.TenKH, kh.DienThoai, hd.DiaChiGiaoHang, cartFake,
                    "Chuyển khoản Ngân hàng (Đã thanh toán)",
                    0m, 0m, (decimal)(hd.TongTien ?? 0m), ""
                );
            }

            Session["Cart"] = null;
            ClearCoupon();
            return View("DatHangThanhCong");
        }

        private void SendOrderConfirmationEmail(
            string toEmail,
            string tenNguoiNhan,
            string sdt,
            string diachi,
            List<GioHangItem> cart,
            string phuongthuc,
            decimal shippingFee,
            decimal discount,
            decimal grandTotal,
            string couponCode
        )
        {
            try
            {
                decimal subTotal = (cart != null && cart.Any()) ? cart.Sum(x => x.ThanhTien) : 0m;

                var sb = new StringBuilder();
                sb.Append("<h2>CAFE HOUSE - XÁC NHẬN ĐƠN HÀNG</h2>");
                sb.Append("<p><b>Người đặt:</b> " + tenNguoiNhan + "</p>");
                sb.Append("<p><b>SĐT:</b> " + sdt + "</p>");
                sb.Append("<p><b>Địa chỉ:</b> " + diachi + "</p>");
                sb.Append("<p><b>Thanh toán:</b> " + phuongthuc + "</p>");

                if (!string.IsNullOrWhiteSpace(couponCode))
                    sb.Append("<p><b>Mã ưu đãi:</b> " + couponCode + "</p>");

                sb.Append("<table border='1' cellpadding='6' cellspacing='0'>");
                sb.Append("<tr><th>Sản phẩm</th><th>Giá</th><th>SL</th><th>Thành tiền</th></tr>");

                foreach (var i in cart)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + i.TenSP + "</td>");
                    sb.Append("<td>" + i.GiaBan.ToString("#,##0") + "</td>");
                    sb.Append("<td>" + i.SoLuong + "</td>");
                    sb.Append("<td>" + i.ThanhTien.ToString("#,##0") + "</td>");
                    sb.Append("</tr>");
                }

                sb.Append("</table>");
                sb.Append("<p><b>Tạm tính:</b> " + subTotal.ToString("#,##0") + " đ</p>");

                // Ship tự động: nếu 0 thì ghi miễn phí
                sb.Append("<p><b>Phí vận chuyển:</b> " + (shippingFee == 0m ? "Miễn phí" : shippingFee.ToString("#,##0") + " đ") + "</p>");

                if (discount > 0m) sb.Append("<p><b>Giảm giá:</b> -" + discount.ToString("#,##0") + " đ</p>");
                sb.Append("<p><b>Tổng cộng:</b> " + grandTotal.ToString("#,##0") + " đ</p>");
                sb.Append("<p>Cảm ơn quý khách đã ủng hộ!</p>");

                var mail = new MailMessage();
                mail.From = new MailAddress("quan.cafe.house@gmail.com", "CAFE HOUSE");
                mail.To.Add(toEmail);
                mail.Subject = "Xác nhận đơn hàng #" + DateTime.Now.Ticks.ToString().Substring(10) + " - CAFE HOUSE";
                mail.Body = sb.ToString();
                mail.IsBodyHtml = true;

                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(
                        "quan.cafe.house@gmail.com",
                        "uvli ygow kywl ngnj"
                    ),
                    EnableSsl = true
                };

                smtp.Send(mail);
            }
            catch
            {
            }
        }

        public ActionResult LichSuDatHang()
        {
            if (Session["User"] == null || Session["Role"].ToString() != "Customer")
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("LichSuDatHang", "GioHang") });

            int makh = ((tblKhachHang)Session["User"]).MaKH;
            var donHangs = db.tblHoaDons
                .Where(h => h.MaKH == makh)
                .OrderByDescending(h => h.NgayLap)
                .ToList();

            return View(donHangs);
        }

        public ActionResult ChiTietDonHang(int id)
        {
            if (Session["User"] == null || Session["Role"].ToString() != "Customer")
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("ChiTietDonHang", "GioHang", new { id }) });

            var user = (tblKhachHang)Session["User"];
            var hd = db.tblHoaDons.FirstOrDefault(h => h.MaHD == id && h.MaKH == user.MaKH);
            if (hd == null) return HttpNotFound();

            var ct = db.tblChiTietHoaDons.Where(c => c.MaHD == id).ToList();
            ViewBag.ChiTiet = ct;
            return View(hd);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HuyDonHang(int id)
        {
            if (Session["User"] == null || Session["Role"].ToString() != "Customer")
                return RedirectToAction("Login", "Account");

            var user = (tblKhachHang)Session["User"];
            var hd = db.tblHoaDons.FirstOrDefault(h => h.MaHD == id && h.MaKH == user.MaKH);
            if (hd == null || hd.TinhTrang != 1)
                return RedirectToAction("LichSuDatHang");

            hd.TinhTrang = 3; // Đã hủy
            db.SaveChanges();

            return RedirectToAction("LichSuDatHang");
        }
    }
}
