#  Website Quản Lý Quán Cà Phê (Q_COFFEE)

## Giới thiệu dự án
Dự án được xây dựng nhằm cung cấp giải pháp quản lý toàn diện cho các quán cà phê, từ việc hiển thị thực đơn, gọi món trực tuyến cho khách hàng, đến các tính năng quản trị hệ thống, phân quyền dành cho admin và nhân viên.

## 🛠 Công nghệ sử dụng
- **Nền tảng & Ngôn ngữ:** C# / .NET Framework 4.8 / ASP.NET MVC
- **Giao diện (Frontend):** HTML5, CSS3, Bootstrap 5, jQuery
- **Cơ sở dữ liệu (Database):** SQL Server, Entity Framework (EDMX)
- **Tích hợp mở rộng:** Cổng thanh toán MoMo, dịch vụ Ngrok

## Các chức năng chính

###  Dành cho Khách hàng (Trang chủ)
- Xem danh sách sản phẩm, chi tiết món ăn/đồ uống theo phân loại danh mục.
- Đăng ký tài khoản, đăng nhập, quên mật khẩu và xác thực OTP.
- Quản lý giỏ hàng: Thêm/Xóa/Sửa số lượng món ăn.
- Tiến hành đặt hàng, xem lịch sử mua hàng và tình trạng đơn hàng.
- **Thanh toán trực tuyến:** Tích hợp cổng thanh toán điện tử MoMo.
- Gửi thông tin liên hệ và bình luận góp ý về sản phẩm.

### Dành cho Admin & Nhân viên (Trang Quản trị)
- **Phân quyền hệ thống:** Cơ chế kiểm soát quyền truy cập dành riêng cho Admin và Nhân viên (tblPhanQuyen).
- **Quản lý danh mục & sản phẩm:** Thêm, xóa, sửa thông tin món ăn, hình ảnh và nhà cung cấp.
- **Quản lý đơn hàng:** Tiếp nhận, xử lý và cập nhật trạng thái các đơn đặt hàng từ khách.
- **Quản lý khách hàng & nhân viên:** Theo dõi danh sách tài khoản, thông tin cá nhân và vai trò trong hệ thống.
- **Báo cáo & Thống kê:** Theo dõi doanh thu, lịch sử giao dịch và phản hồi từ biểu mẫu liên hệ.

## Cấu trúc mã nguồn nổi bật
- `Controllers/`: Chứa các bộ điều hướng xử lý logic chính như `AccountController` (Xử lý đăng nhập/OTP), `GioHangController`, `PaymentController` (Xử lý cổng thanh toán MoMo).
- `Models/` & `Q_COFFE.edmx`: Hệ thống ánh xạ cơ sở dữ liệu và quản lý thực thể (tblSanPham, tblHoaDon, tblKhachHang,...).
- `Views/`: Hệ thống giao diện được tổ chức rõ ràng với các Layout riêng biệt (`_LayOutAdmin`, `_LayOutHome`).
- `QL_QuanCafe.sql`: File script khởi tạo cấu trúc cơ sở dữ liệu và dữ liệu mẫu của hệ thống.
