using DOAN_QUANCAFE;
using Newtonsoft.Json;
using DOAN_QUANCAFE.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DOAN_QUANCAFE.Controllers
{
    public class PaymentController : Controller
    {
        private QL_QuanCafeEntities db = new QL_QuanCafeEntities();

        [HttpPost]
        public async Task<ActionResult> CreatePayment(int maHD)
        {
            var hoaDon = db.tblHoaDons.FirstOrDefault(x => x.MaHD == maHD);
            if (hoaDon == null) return HttpNotFound();

            // === TỰ ĐỘNG LẤY LINK NGROK TẠI ĐÂY ===
            string baseNgrokUrl = await NgrokService.GetNgrokPublicUrl();

            // Thông tin cấu hình MoMo
            string endpoint = "https://payment.momo.vn/v2/gateway/api/create";
            string partnerCode = "MOMOAHMJ20251221";
            string accessKey = "3YfrHSt6bQEaJAol";
            string secretKey = "eBRv9Z954duLyf3ZQKqVRGZjimCMbWdR";

            string orderInfo = "Thanh toán đơn hàng #" + hoaDon.MaHD;

            // Tự động nối link Ngrok vào các URL phản hồi
            string redirectUrl = baseNgrokUrl + "/Payment/ConfirmPaymentClient";
            string ipnUrl = baseNgrokUrl + "/Payment/SavePayment";

            string amount = ((int)hoaDon.TongTien).ToString();
            string orderId = hoaDon.MaHD.ToString() + "_" + DateTime.Now.Ticks.ToString();
            string requestId = Guid.NewGuid().ToString();
            string requestType = "captureWallet";
            string extraData = "";

            // Tạo chữ ký Signature
            string rawHash = "accessKey=" + accessKey +
                "&amount=" + amount +
                "&extraData=" + extraData +
                "&ipnUrl=" + ipnUrl +
                "&orderId=" + orderId +
                "&orderInfo=" + orderInfo +
                "&partnerCode=" + partnerCode +
                "&redirectUrl=" + redirectUrl +
                "&requestId=" + requestId +
                "&requestType=" + requestType;

            MomoSecurity crypto = new MomoSecurity();
            string signature = crypto.signSHA256(rawHash, secretKey);

            var message = new
            {
                partnerCode = partnerCode,
                partnerName = "Test",
                storeId = "MomoTestStore",
                requestId = requestId,
                amount = amount,
                orderId = orderId,
                orderInfo = orderInfo,
                redirectUrl = redirectUrl,
                ipnUrl = ipnUrl,
                lang = "vi",
                extraData = extraData,
                requestType = requestType,
                signature = signature
            };

            using (HttpClient client = new HttpClient())
            {
                var requestContent = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(endpoint, requestContent);
                var resContent = await response.Content.ReadAsStringAsync();

                var momoResponse = JsonConvert.DeserializeObject<MomoResponse>(resContent);

                // Nếu MoMo trả về URL thành công, Redirect sang trang đó để hiện mã QR
                if (momoResponse != null && !string.IsNullOrEmpty(momoResponse.payUrl))
                {
                    // Trong hàm CreatePayment cũ, thay dòng return Redirect(momoResponse.payUrl); bằng:
                    TempData["PayUrl"] = momoResponse.payUrl;
                    TempData["MaHD"] = maHD;
                    return RedirectToAction("WaitingPayment");
                }
                else
                {
                    return View("Error"); // Bạn nên tạo một View báo lỗi nếu không lấy được QR
                }
            }
        }
        // Action này dùng để hiển thị trang chờ thanh toán
        public ActionResult WaitingPayment(string payUrl, int maHD)
        {
            ViewBag.PayUrl = payUrl;
            ViewBag.MaHD = maHD;
            return View();
        }
        [HttpPost]
        public ActionResult SavePayment(MomoResponse response)
        {
            // MoMo sẽ tự động gọi vào hàm này qua đường link Ngrok
            // resultCode = 0 là thanh toán thành công
            if (response.resultCode == 0)
            {
                // Lấy MaHD từ orderId (Ví dụ "1_638xxx" lấy được số 1)
                int maHD = int.Parse(response.orderId.Split('_')[0]);

                // Tìm hóa đơn trong SQL Server của bạn
                var hoaDon = db.tblHoaDons.Find(maHD);
                if (hoaDon != null)
                {
                    hoaDon.DaThanhToan = true; // Cập nhật cột bit trong file SQL bạn gửi
                    db.SaveChanges(); // Lưu thay đổi vào Database
                }
            }

            // Trả về kết quả trống để MoMo biết bạn đã nhận được tin
            return Content("");
        }
        [HttpGet]
        public JsonResult CheckStatus(int maHD)
        {
            var hoaDon = db.tblHoaDons.Find(maHD);
            return Json(new { isPaid = (hoaDon != null && hoaDon.DaThanhToan == true) }, JsonRequestBehavior.AllowGet);
        }
    }
}