using DOAN_QUANCAFE;
using Newtonsoft.Json; // Cần thư viện này để đọc JSON từ Google
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DOAN_QUANCAFE.Controllers
{
    public class AccountController : Controller
    {
        QL_QuanCafeEntities db = new QL_QuanCafeEntities();

        // ==========================================
        // CẤU HÌNH GOOGLE LOGIN (Thay đổi ở đây)
        // ==========================================
        private readonly string GoogleClientId = "156762013462-udegtn37akejkivnj62esjkvaq55o314.apps.googleusercontent.com";
        private readonly string GoogleClientSecret = "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";
        private readonly string GoogleRedirectUrl = "https://localhost:44359/Account/LoginGoogleCallback"; // Sửa port 44300 theo máy bạn

        // =========================
        // 1. ĐĂNG KÝ (GIỮ NGUYÊN)
        // =========================
        [HttpGet]
        public ActionResult Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(tblKhachHang kh, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var check = db.tblKhachHangs.FirstOrDefault(x => x.DienThoai == kh.DienThoai);
                if (check == null)
                {
                    kh.Avarta = "default-user.jpg"; // Lưu ý: DB bạn ghi sai chính tả là Avarta
                    db.tblKhachHangs.Add(kh);
                    db.SaveChanges();

                    SetSession(kh.MaKH, kh.TenKH, "Customer", kh);

                    if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.Error = "Số điện thoại đã được đăng ký";
            }
            return View(kh);
        }

        // =========================
        // 2. ĐĂNG NHẬP (ĐÃ SỬA THEO CÁCH 1)
        // =========================
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string UserPhone, string Password, string returnUrl)
        {
            // 2.1. Kiểm tra Admin (Đăng nhập bằng Tên NV vì DB không có user/phone)
            var admin = db.tblNhanViens.FirstOrDefault(x => x.TenNV == UserPhone && x.MatKhau == Password);
            if (admin != null)
            {
                SetSession(admin.MaNV, admin.TenNV, "Admin", admin);
                return RedirectToAction("Index", "Admin");
            }

            // 2.2. Kiểm tra Khách hàng (Đăng nhập bằng SĐT)
            var kh = db.tblKhachHangs.FirstOrDefault(x => x.DienThoai == UserPhone && x.MatKhau == Password);
            if (kh != null)
            {
                SetSession(kh.MaKH, kh.TenKH, "Customer", kh);

                if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // =========================
        // 3. XỬ LÝ LOGIN GOOGLE
        // =========================
        public ActionResult LoginGoogle()
        {
            var url = $"https://accounts.google.com/o/oauth2/auth?client_id={GoogleClientId}&redirect_uri={GoogleRedirectUrl}&scope=email profile&response_type=code";
            return Redirect(url);
        }

        public async Task<ActionResult> LoginGoogleCallback(string code)
        {
            if (string.IsNullOrEmpty(code)) return RedirectToAction("Login");

            // 1. Lấy Access Token từ Code
            var tokenUrl = "https://accounts.google.com/o/oauth2/token";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", GoogleClientId),
                new KeyValuePair<string, string>("client_secret", GoogleClientSecret),
                new KeyValuePair<string, string>("redirect_uri", GoogleRedirectUrl),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            });

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(tokenUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic tokenData = JsonConvert.DeserializeObject(responseString);
                string accessToken = tokenData.access_token;

                if (!string.IsNullOrEmpty(accessToken))
                {
                    // 2. Lấy thông tin User từ Google
                    var userInfoUrl = $"https://www.googleapis.com/oauth2/v2/userinfo?access_token={accessToken}";
                    var userInfoResponse = await client.GetStringAsync(userInfoUrl);
                    dynamic userInfo = JsonConvert.DeserializeObject(userInfoResponse);

                    string email = userInfo.email;
                    string name = userInfo.name;
                    string picture = userInfo.picture;

                    // 3. Xử lý lưu vào Database
                    var kh = db.tblKhachHangs.FirstOrDefault(x => x.Email == email);
                    if (kh == null)
                    {
                        // Nếu chưa có thì tạo mới
                        kh = new tblKhachHang
                        {
                            TenKH = name,
                            Email = email,
                            Avarta = picture, // Lưu link ảnh từ Google
                            MatKhau = "", // Không cần mật khẩu
                            DienThoai = "", // Google không trả về SĐT mặc định
                            GioiTinh = "Nam", // Mặc định
                            NamSinh = 2000    // Mặc định
                        };
                        db.tblKhachHangs.Add(kh);
                        db.SaveChanges();
                    }

                    // 4. Set Session và đăng nhập
                    SetSession(kh.MaKH, kh.TenKH, "Customer", kh);
                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("Login");
        }

        // =========================
        // HÀM PHỤ TRỢ
        // =========================
        public ActionResult LogOff()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private void SetSession(int id, string name, string role, object userObj)
        {
            Session["UserID"] = id;
            Session["UserName"] = name;
            Session["Role"] = role;
            Session["User"] = userObj;
        }

        // 1. Giao diện nhập Email
        [HttpGet]
        public ActionResult ForgotPassword() { return View(); }

        // 2. Xử lý gửi mã OTP qua Email
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            var user = db.tblKhachHangs.FirstOrDefault(x => x.Email == email);
            if (user == null)
            {
                ViewBag.Error = "Email này không tồn tại trong hệ thống!";
                return View();
            }

            // Tạo mã OTP 6 số ngẫu nhiên và lưu vào Session
            string otp = new Random().Next(100000, 999999).ToString();
            Session["ResetOTP"] = otp;
            Session["ResetEmail"] = email;

            try
            {
                var mail = new MailMessage();
                mail.From = new MailAddress("quan.cafe.house@gmail.com", "CAFE HOUSE Support");
                mail.To.Add(email);
                mail.Subject = "Mã xác nhận khôi phục mật khẩu";
                mail.Body = $"Mã xác nhận của bạn là: <b style='font-size:20px'>{otp}</b>. Vui lòng không cung cấp mã này cho ai.";
                mail.IsBodyHtml = true;

                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("quan.cafe.house@gmail.com", "uvli ygow kywl ngnj"),
                    EnableSsl = true
                };
                smtp.Send(mail);
                return RedirectToAction("VerifyOTP"); // Chuyển sang trang nhập OTP
            }
            catch
            {
                ViewBag.Error = "Không thể gửi mail, vui lòng kiểm tra kết nối!";
                return View();
            }
        }

        // 3. Giao diện nhập mã OTP và Mật khẩu mới
        [HttpGet]
        public ActionResult VerifyOTP()
        {
            if (Session["ResetOTP"] == null) return RedirectToAction("ForgotPassword");
            return View();
        }

        // 4. Xác nhận OTP và cập nhật Mật khẩu vào DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyOTP(string otp, string newPassword, string confirmPassword)
        {
            if (otp != Session["ResetOTP"]?.ToString())
            {
                ViewBag.Error = "Mã OTP không chính xác!";
                return View();
            }
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            string email = Session["ResetEmail"]?.ToString();
            var user = db.tblKhachHangs.FirstOrDefault(x => x.Email == email);
            if (user != null)
            {
                user.MatKhau = newPassword; // Cập nhật mật khẩu mới
                db.SaveChanges();

                // Xóa thông tin tạm trong Session
                Session.Remove("ResetOTP");
                Session.Remove("ResetEmail");

                TempData["Success"] = "Đổi mật khẩu thành công! Hãy đăng nhập lại.";
                return RedirectToAction("Login");
            }
            return RedirectToAction("ForgotPassword");
        }
    }
}