# ☕ Website Quản Lý Quán Cà Phê (Q_COFFEE)

## 📌 Giới thiệu dự án
**Q_COFFEE** là một nền tảng quản lý và thương mại điện tử hoàn chỉnh (End-to-End) được xây dựng phục vụ cho các mô hình kinh doanh quán cà phê hiện đại[cite: 3]. 

Hệ thống giải quyết bài toán tối ưu hóa quy trình vận hành từ thủ công sang công nghệ số: cho phép khách hàng đặt món trực tuyến thông qua cổng thanh toán ví điện tử, đồng thời cung cấp hệ thống quản trị (Admin Dashboard) mạnh mẽ giúp doanh nghiệp kiểm soát chặt chẽ sản phẩm, phân quyền nhân viên, quản lý đơn hàng và theo dõi báo cáo thống kê doanh thu[cite: 3].

## 🛠 Công nghệ sử dụng
- **Nền tảng & Ngôn ngữ Backend:** ASP.NET Core 8 MVC / Ngôn ngữ C# / Visual Studio[cite: 3]
- **Tương tác Cơ sở dữ liệu:** Entity Framework Core (EF Core) theo phương pháp **Code First** & **Migration**[cite: 3]
- **Cơ sở dữ liệu (Database):** Microsoft SQL Server[cite: 3]
- **Giao diện (Frontend):** HTML5, CSS3, Bootstrap 5, thư viện jQuery[cite: 3]
- **Xử lý bất đồng bộ:** Gọi Ajax kết hợp jQuery để kiểm tra form và tối ưu hiệu suất tải trang[cite: 3]
- **Tích hợp cổng dịch vụ:** Cổng thanh toán trực tuyến MoMo API[cite: 3]

## ⚡ Các chức năng chính (Theo cấu trúc phân quyền hệ thống)

### 1. Phân hệ dành cho Khách hàng (Client Website)
- **Xem & Lọc sản phẩm:** Khám phá thực đơn đồ uống/bánh ngọt được phân loại chi tiết theo từng danh mục, hỗ trợ bộ lọc nhanh theo mức giá và nhà cung cấp[cite: 3].
- **Hệ thống tài khoản cá nhân:** Đăng ký tài khoản, đăng nhập hệ thống, khôi phục mật khẩu và cập nhật thông tin hồ sơ cá nhân[cite: 3].
- **Quản lý mua hàng:** Thao tác thêm/xóa/sửa số lượng món ăn trong giỏ hàng, nạp mã ưu đãi giảm giá và tiến hành đặt hàng[cite: 3].
- **Thanh toán trực tuyến:** Tích hợp cổng thanh toán ví điện tử MoMo API, xử lý luồng giao dịch trực tuyến an toàn và mượt mà[cite: 3].
- **Lịch sử đơn hàng:** Theo dõi chi tiết danh sách, thời gian, tổng tiền và trạng thái xử lý các đơn đặt hàng cũ[cite: 3].
- **Tương tác & Phản hồi:** Gửi biểu mẫu liên hệ, viết bình luận góp ý và đánh giá số sao trực tiếp trên từng sản phẩm[cite: 3].

### 2. Phân hệ dành cho Admin & Nhân viên (Admin Dashboard)
- **Phân quyền truy cập dựa trên vai trò (RBAC):** Kiểm soát bảo mật và phân chia giới hạn quyền hạn nghiêm ngặt giữa Quản trị viên (Admin) và Nhân viên hệ thống[cite: 3].
- **Thống kê & Báo cáo dữ liệu:** Màn hình Dashboard trực quan hiển thị tổng quan các chỉ số kinh doanh chính: tổng doanh thu, tổng đơn hàng, tổng sản phẩm và số lượng tài khoản khách hàng[cite: 3].
- **Quản lý danh mục & Sản phẩm:** Thực hiện các thao tác CRUD (Thêm, sửa, xóa, xem chi tiết) danh mục món ăn, quản lý kích thước (Size S/M/L) và hình ảnh sản phẩm[cite: 3].
- **Quản lý đơn hàng:** Theo dõi danh sách hóa đơn, cập nhật trạng thái xử lý đơn hàng (Đang xử lý, Đang giao, Đã giao, Đã hủy) và trạng thái thanh toán[cite: 3].
- **Quản lý người dùng & Yêu cầu:** Quản lý thông tin tài khoản khách hàng, kiểm soát danh sách liên hệ phản hồi và hỗ trợ reset mật khẩu khi có yêu cầu[cite: 3].

## 📂 Cấu trúc mã nguồn nổi bật
- `Controllers/` : Chứa các bộ điều hướng xử lý logic nghiệp vụ chính của hệ thống (Xử lý tài khoản, giỏ hàng, thanh toán MoMo, nghiệp vụ quản trị...)[cite: 3].
- `Models/` : Định nghĩa cấu trúc dữ liệu và các thực thể quan hệ (`NguoiDung`, `DiaDiem`, `LichTrinh`, `BaiViet`, `BinhLuan`, `DanhGia`...) kết nối thông qua EF Core Code First[cite: 3, 4].
- `Views/` : Hệ thống giao diện hiển thị cho người dùng, được tổ chức phân tầng mã nguồn và quản lý thông qua các Layout dùng chung[cite: 3].
- `database/` : Chứa file script `.sql` khởi tạo cấu trúc cơ sở dữ liệu quan hệ chặt chẽ và nạp dữ liệu mẫu ban đầu vào SQL Server[cite: 3, 4].

## 🚀 Hướng dẫn cài đặt và chạy thử (Installation & Setup)

### 1. Cấu hình Cơ sở dữ liệu (Database)
1. Mở phần mềm **SQL Server Management Studio (SSMS)**.
2. Tạo một cơ sở dữ liệu mới trên máy cục bộ của bạn.
3. Mở và thực thi (Execute) file script `.sql` nằm trong thư mục `database/` để tự động khởi tạo cấu trúc các bảng quan hệ và nạp dữ liệu mẫu[cite: 3, 4].

### 2. Cấu hình và Khởi chạy Website
1. Mở thư mục mã nguồn dự án Website bằng **Visual Studio**.
2. Tìm đến file cấu hình hệ thống, cập nhật lại chuỗi `ConnectionStrings` trỏ chính xác về tên SQL Server và Cơ sở dữ liệu bạn vừa import ở bước trên[cite: 3].
3. Tiến hành Build dự án để đồng bộ hóa cấu trúc thực thể qua Entity Framework Core[cite: 3].
4. Nhấn **F5** (hoặc nút **Start**) để khởi chạy website trên trình duyệt web cục bộ (Localhost)[cite: 3].
