-- =============================================================================
-- DATABASE: QL_QuanCafe
-- Mô tả: Script tạo database quản lý quán cà phê
-- =============================================================================

USE master;
GO

-- Kiểm tra và xóa database cũ nếu có
IF DB_ID('QL_QuanCafe') IS NOT NULL
BEGIN
    -- Ngắt kết nối tất cả user khỏi database
    ALTER DATABASE QL_QuanCafe SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    
    -- Xóa database
    DROP DATABASE QL_QuanCafe;
END
GO

-- =============================================================================
-- 1. TẠO DATABASE
-- =============================================================================
CREATE DATABASE QL_QuanCafe;
GO

USE [QL_QuanCafe]
GO

-- =============================================================================
-- 2. TẠO CÁC BẢNG
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 2.1 Bảng tblLoaiSanPham - Loại sản phẩm
-- -----------------------------------------------------------------------------
CREATE TABLE [dbo].[tblLoaiSanPham](
    [MaLoai] [int] IDENTITY(1,1) NOT NULL,
    [TenLoai] [nvarchar](100) NULL,
    [GhiChu] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED ([MaLoai] ASC)
) ON [PRIMARY]
GO

-- -----------------------------------------------------------------------------
-- 2.2 Bảng tblNhaCungCap - Nhà cung cấp
-- -----------------------------------------------------------------------------
CREATE TABLE [dbo].[tblNhaCungCap](
    [MaNCC] [int] IDENTITY(1,1) NOT NULL,
    [TenNCC] [nvarchar](100) NULL,
    [DiaChi] [nvarchar](255) NULL,
    [DienThoai] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED ([MaNCC] ASC)
) ON [PRIMARY]
GO

-- -----------------------------------------------------------------------------
-- 2.3 Bảng tblSanPham - Sản phẩm
-- -----------------------------------------------------------------------------
CREATE TABLE [dbo].[tblSanPham](
    [MaSP] [int] IDENTITY(1,1) NOT NULL,
    [TenSP] [nvarchar](200) NULL,
    [KichThuoc] [nvarchar](50) NULL,
    [GiaBan] [decimal](18, 2) NULL,
    [MoTa] [nvarchar](max) NULL,
    [HinhAnh] [nvarchar](255) NULL, -- Ảnh đại diện
    [LoaiSP] [int] NULL,
    [NhaCungCap] [int] NULL,
PRIMARY KEY CLUSTERED ([MaSP] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- -----------------------------------------------------------------------------
-- 2.4 Bảng tblHinhAnh - Hình ảnh chi tiết sản phẩm
-- -----------------------------------------------------------------------------
CREATE TABLE [dbo].[tblHinhAnh](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [MaSP] [int] NULL,
    [HinhAnh] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED ([ID] ASC)
) ON [PRIMARY]
GO

-- -----------------------------------------------------------------------------
-- 2.5 Bảng BinhLuan - Bình luận sản phẩm
-- -----------------------------------------------------------------------------
CREATE TABLE [dbo].[BinhLuan](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [MaSP] [int] NOT NULL,
    [HoTen] [nvarchar](100) NOT NULL,
    [NoiDung] [nvarchar](500) NOT NULL,
    [SoSao] [int] NOT NULL,
    [Ngay] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

-- -----------------------------------------------------------------------------
-- 2.6 Bảng tblVaiTro - Vai trò nhân viên
-- -----------------------------------------------------------------------------
CREATE TABLE [dbo].[tblVaiTro](
    [IDVaiTro] [int] IDENTITY(1,1) NOT NULL,
    [TenVaiTro] [nvarchar](50) NULL,
    [MoTa] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED ([IDVaiTro] ASC)
) ON [PRIMARY]
GO

-- -----------------------------------------------------------------------------
-- 2.7 Bảng tblNhanVien - Nhân viên
-- -----------------------------------------------------------------------------
CREATE TABLE [dbo].[tblNhanVien](
    [MaNV] [int] IDENTITY(1,1) NOT NULL,
    [MatKhau] [nvarchar](100) NULL,
    [TenNV] [nvarchar](100) NULL,
    [GioiTinh] [nvarchar](10) NULL,
    [NamSinh] [int] NULL,
    [VaiTro] [int] NULL,
PRIMARY KEY CLUSTERED ([MaNV] ASC)
) ON [PRIMARY]
GO

-- -----------------------------------------------------------------------------
-- 2.8 Bảng tblKhachHang - Khách hàng
-- -----------------------------------------------------------------------------
CREATE TABLE [dbo].[tblKhachHang](
    [MaKH] [int] IDENTITY(1,1) NOT NULL,
    [TenKH] [nvarchar](100) NULL,
    [MatKhau] [nvarchar](100) NULL,
    [GioiTinh] [nvarchar](10) NULL,
    [NamSinh] [int] NULL,
    [Avarta] [nvarchar](255) NULL,
    [DienThoai] [nvarchar](20) NULL,
    [Email] [nvarchar](100) NULL,
    [DiaChi] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED ([MaKH] ASC)
) ON [PRIMARY]
GO

-- -----------------------------------------------------------------------------
-- 2.9 Bảng tblTinhTrang - Tình trạng hóa đơn
-- -----------------------------------------------------------------------------
CREATE TABLE [dbo].[tblTinhTrang](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [TinhTrangHoaDon] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED ([ID] ASC)
) ON [PRIMARY]
GO

-- -----------------------------------------------------------------------------
-- 2.10 Bảng tblHoaDon - Hóa đơn
-- -----------------------------------------------------------------------------
CREATE TABLE [dbo].[tblHoaDon](
    [MaHD] [int] IDENTITY(1,1) NOT NULL,
    [MaKH] [int] NULL,
    [MaNV] [int] NULL,
    [NgayLap] [datetime] NULL,
    [TongTien] [decimal](18, 2) NULL,
    [TinhTrang] [int] NULL,
    [DiaChiGiaoHang] [nvarchar](255) NULL,
    [DaThanhToan] [bit] NULL,
PRIMARY KEY CLUSTERED ([MaHD] ASC)
) ON [PRIMARY]
GO

-- -----------------------------------------------------------------------------
-- 2.11 Bảng tblChiTietHoaDon - Chi tiết hóa đơn
-- -----------------------------------------------------------------------------
CREATE TABLE [dbo].[tblChiTietHoaDon](
    [MaHD] [int] NOT NULL,
    [MaSP] [int] NOT NULL,
    [SoLuong] [int] NULL,
    [GiaBan] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED ([MaHD] ASC, [MaSP] ASC)
) ON [PRIMARY]
GO

-- -----------------------------------------------------------------------------
-- 2.12 Bảng LienHe - Liên hệ
-- -----------------------------------------------------------------------------
CREATE TABLE [dbo].[LienHe] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [HoTen] NVARCHAR(100) NOT NULL,
    [EmailOrPhone] NVARCHAR(100) NOT NULL,
    [NoiDung] NVARCHAR(MAX) NOT NULL,
    [NgayGui] DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- =============================================================================
-- 3. TẠO KHÓA NGOẠI (FOREIGN KEYS)
-- =============================================================================

-- FK: tblSanPham -> tblLoaiSanPham
ALTER TABLE [dbo].[tblSanPham] ADD FOREIGN KEY([LoaiSP]) REFERENCES [dbo].[tblLoaiSanPham] ([MaLoai])
GO

-- FK: tblSanPham -> tblNhaCungCap
ALTER TABLE [dbo].[tblSanPham] ADD FOREIGN KEY([NhaCungCap]) REFERENCES [dbo].[tblNhaCungCap] ([MaNCC])
GO

-- FK: tblHinhAnh -> tblSanPham
ALTER TABLE [dbo].[tblHinhAnh] ADD FOREIGN KEY([MaSP]) REFERENCES [dbo].[tblSanPham] ([MaSP])
GO

-- FK: BinhLuan -> tblSanPham
ALTER TABLE [dbo].[BinhLuan] ADD CONSTRAINT [FK_BinhLuan_tblSanPham] FOREIGN KEY([MaSP]) REFERENCES [dbo].[tblSanPham] ([MaSP])
GO

-- FK: tblNhanVien -> tblVaiTro
ALTER TABLE [dbo].[tblNhanVien] ADD FOREIGN KEY([VaiTro]) REFERENCES [dbo].[tblVaiTro] ([IDVaiTro])
GO

-- FK: tblHoaDon -> tblKhachHang
ALTER TABLE [dbo].[tblHoaDon] ADD FOREIGN KEY([MaKH]) REFERENCES [dbo].[tblKhachHang] ([MaKH])
GO

-- FK: tblHoaDon -> tblNhanVien
ALTER TABLE [dbo].[tblHoaDon] ADD FOREIGN KEY([MaNV]) REFERENCES [dbo].[tblNhanVien] ([MaNV])
GO

-- FK: tblHoaDon -> tblTinhTrang
ALTER TABLE [dbo].[tblHoaDon] ADD FOREIGN KEY([TinhTrang]) REFERENCES [dbo].[tblTinhTrang] ([ID])
GO

-- FK: tblChiTietHoaDon -> tblSanPham
ALTER TABLE [dbo].[tblChiTietHoaDon] ADD FOREIGN KEY([MaSP]) REFERENCES [dbo].[tblSanPham] ([MaSP])
GO

-- FK: tblChiTietHoaDon -> tblHoaDon
ALTER TABLE [dbo].[tblChiTietHoaDon] ADD FOREIGN KEY([MaHD]) REFERENCES [dbo].[tblHoaDon] ([MaHD])
GO

-- =============================================================================
-- 4. CHÈN DỮ LIỆU MẪU
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 4.1 Dữ liệu tblLoaiSanPham
-- -----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[tblLoaiSanPham] ON 
INSERT [dbo].[tblLoaiSanPham] ([MaLoai], [TenLoai], [GhiChu]) VALUES 
(1, N'Cà phê', N'Các loại cà phê'),
(2, N'Trà - Trà sữa', N'Trà và trà sữa các loại'),
(3, N'Sinh tố - Nước ép', N'Đồ uống từ trái cây'),
(4, N'Bánh ngọt', N'Bánh và đồ ăn nhẹ')
SET IDENTITY_INSERT [dbo].[tblLoaiSanPham] OFF
GO

-- -----------------------------------------------------------------------------
-- 4.2 Dữ liệu tblNhaCungCap
-- -----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[tblNhaCungCap] ON 
INSERT [dbo].[tblNhaCungCap] ([MaNCC], [TenNCC], [DiaChi], [DienThoai]) VALUES 
(1, N'Trung Nguyên Legend', N'TP. Hồ Chí Minh', N'02838227490'),
(2, N'Highlands Coffee', N'Hà Nội', N'02438515320'),
(3, N'Vinamilk', N'TP. Hồ Chí Minh', N'02838225340')
SET IDENTITY_INSERT [dbo].[tblNhaCungCap] OFF
GO

-- -----------------------------------------------------------------------------
-- 4.3 Dữ liệu tblSanPham
-- -----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[tblSanPham] ON 
INSERT [dbo].[tblSanPham] ([MaSP], [TenSP], [KichThuoc], [GiaBan], [MoTa], [HinhAnh], [LoaiSP], [NhaCungCap]) VALUES 
(1, N'Cappuccino', N'M', CAST(45000.00 AS Decimal(18, 2)), N'Cà phê Espresso pha với sữa nóng và bọt sữa mịn màng', N'Hinh1.jpeg', 1, 2),
(2, N'Espresso', N'S', CAST(35000.00 AS Decimal(18, 2)), N'Cà phê nguyên chất đậm đà, hương vị mạnh mẽ', N'Hinh2.jpeg', 1, 1),
(3, N'Latte', N'L', CAST(50000.00 AS Decimal(18, 2)), N'Cà phê Espresso kết hợp với sữa tươi thơm ngon', N'Hinh3.jpeg', 1, 2),
(4, N'Trà sữa trân châu', N'L', CAST(48000.00 AS Decimal(18, 2)), N'Trà sữa béo ngậy kèm trân châu dai ngon', N'Hinh4.jpeg', 2, 3),
(5, N'Sinh tố bơ', N'M', CAST(55000.00 AS Decimal(18, 2)), N'Sinh tố bơ sánh mịn, bổ dưỡng', N'Hinh5.jpeg', 3, 3),
(6, N'Bánh Tiramisu', N'1 miếng', CAST(42000.00 AS Decimal(18, 2)), N'Bánh Tiramisu kem cheese thơm ngon', N'Hinh6.jpeg', 4, 2),
(7, N'Bạc Xỉu', N'M', CAST(40000.00 AS Decimal(18, 2)), N'Cà phê ít sữa, vị nhẹ, phù hợp cho người mới uống.', N'Hinh7.jpeg', 1, 2),
(8, N'Trà Sữa Khoai Môn', N'L', CAST(50000.00 AS Decimal(18, 2)), N'Trà sữa hương khoai môn béo ngậy.', N'Hinh8.jpeg', 2, 3),
(9, N'Nước Chanh Tươi', N'M', CAST(30000.00 AS Decimal(18, 2)), N'Nước chanh tươi giải khát.', N'Hinh9.jpeg', 3, 1),
(10, N'Bánh Su Kem', N'1 cái', CAST(35000.00 AS Decimal(18, 2)), N'Bánh su kem nhân béo mặn.', N'Hinh10.jpeg', 4, 2),
(11, N'Cold Brew', N'S', CAST(60000.00 AS Decimal(18, 2)), N'Cà phê ủ lạnh, hương vị mượt mà, ít chua.', N'Hinh11.jpeg', 1, 1),
(12, N'Trà Atiso Đỏ', N'M', CAST(40000.00 AS Decimal(18, 2)), N'Trà Atiso đỏ (Hibiscus) chua nhẹ, giải nhiệt.', N'Hinh12.jpeg', 2, 3),
(13, N'Sinh Tố Dâu Tằm', N'L', CAST(55000.00 AS Decimal(18, 2)), N'Sinh tố dâu tằm tươi, có vị chua ngọt đặc trưng.', N'Hinh13.jpeg', 3, 2),
(14, N'Bánh Brownie', N'1 miếng', CAST(45000.00 AS Decimal(18, 2)), N'Bánh Brownie socola ẩm, đậm vị.', N'Hinh14.jpeg', 4, 2),
(15, N'Espresso Macchiato', N'S', CAST(48000.00 AS Decimal(18, 2)), N'Espresso với một lớp bọt sữa nhỏ phía trên.', N'Hinh15.jpeg', 1, 1)
SET IDENTITY_INSERT [dbo].[tblSanPham] OFF
GO

-- -----------------------------------------------------------------------------
-- 4.4 Dữ liệu tblHinhAnh
-- -----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[tblHinhAnh] ON 
INSERT [dbo].[tblHinhAnh] ([ID], [MaSP], [HinhAnh]) VALUES 
-- Ảnh chi tiết cho MaSP 1-6
(1, 1, N'Hinh7.jpeg'),
(2, 1, N'Hinh6.jpeg'),
(3, 2, N'Hinh5.jpeg'),
(4, 3, N'Hinh4.jpeg'),
(5, 4, N'Hinh3.jpeg'),
(6, 5, N'Hinh2.jpeg'),
(7, 6, N'Hinh1.jpeg'),
(8, 6, N'Hinh8.jpeg'),
-- Ảnh chi tiết cho MaSP 7-15
(9, 7, N'Hinh9.jpeg'),
(10, 7, N'Hinh10.jpeg'),
(11, 8, N'Hinh11.jpeg'),
(12, 9, N'Hinh12.jpeg'),
(13, 10, N'Hinh13.jpeg'),
(14, 10, N'Hinh14.jpeg'),
(15, 11, N'Hinh15.jpeg'),
(16, 12, N'Hinh16.jpeg'),
(17, 13, N'Hinh17.jpeg'),
(18, 13, N'Hinh18.jpeg'),
(19, 14, N'Hinh19.jpeg'),
(20, 15, N'Hinh20.jpeg')
SET IDENTITY_INSERT [dbo].[tblHinhAnh] OFF
GO

-- -----------------------------------------------------------------------------
-- 4.5 Dữ liệu tblVaiTro
-- -----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[tblVaiTro] ON 
INSERT [dbo].[tblVaiTro] ([IDVaiTro], [TenVaiTro], [MoTa]) VALUES 
(1, N'Admin', N'Quản lý hệ thống'),
(2, N'Nhân viên', N'Phục vụ khách hàng')
SET IDENTITY_INSERT [dbo].[tblVaiTro] OFF
GO

-- -----------------------------------------------------------------------------
-- 4.6 Dữ liệu tblNhanVien
-- -----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[tblNhanVien] ON 
INSERT [dbo].[tblNhanVien] ([MaNV], [MatKhau], [TenNV], [GioiTinh], [NamSinh], [VaiTro]) VALUES 
(1, N'admin123', N'Phạm Văn C', N'Nam', 1990, 1),
(2, N'nv123', N'Trần Thị D', N'Nữ', 1992, 2),
(3, N'nv456', N'Đỗ Hoài E', N'Nữ', 1995, 2)
SET IDENTITY_INSERT [dbo].[tblNhanVien] OFF
GO

-- -----------------------------------------------------------------------------
-- 4.7 Dữ liệu tblKhachHang
-- -----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[tblKhachHang] ON 
INSERT [dbo].[tblKhachHang] ([MaKH], [TenKH], [MatKhau], [GioiTinh], [NamSinh], [Avarta], [DienThoai], [Email], [DiaChi]) VALUES 
(1, N'Nguyễn Văn A', N'123456', N'Nam', 1995, N'avt1.jpg', N'0909123456', N'a@gmail.com', N'200 Nguyễn Xí, HCM'),
(2, N'Lê Thị B', N'abcdef', N'Nữ', 1998, N'avt2.jpg', N'0911222333', N'b@gmail.com', N'15 Cầu Giấy, HN'),
(3, N'Trần Minh K', N'kieuhoa', N'Nam', 2000, N'avt3.jpg', N'0987654321', N'k@yahoo.com', N'100 Phan Văn Trị, Bình Thạnh')
SET IDENTITY_INSERT [dbo].[tblKhachHang] OFF
GO

-- -----------------------------------------------------------------------------
-- 4.8 Dữ liệu tblTinhTrang
-- -----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[tblTinhTrang] ON 
INSERT [dbo].[tblTinhTrang] ([ID], [TinhTrangHoaDon]) VALUES 
(1, N'Đang chờ xử lý'),
(2, N'Đã giao hàng'),
(3, N'Đã hủy')
SET IDENTITY_INSERT [dbo].[tblTinhTrang] OFF
GO

-- -----------------------------------------------------------------------------
-- 4.9 Dữ liệu tblHoaDon
-- -----------------------------------------------------------------------------
SET IDENTITY_INSERT [dbo].[tblHoaDon] ON 
INSERT [dbo].[tblHoaDon] ([MaHD], [MaKH], [MaNV], [NgayLap], [TongTien], [TinhTrang], [DiaChiGiaoHang], [DaThanhToan]) VALUES 
(1, 1, 1, CAST(N'2024-12-01T10:30:00.000' AS DateTime), CAST(90000.00 AS Decimal(18, 2)), 2, N'200 Nguyễn Xí, HCM', 1),
(2, 2, 2, CAST(N'2024-12-02T14:00:00.000' AS DateTime), CAST(35000.00 AS Decimal(18, 2)), 2, N'15 Cầu Giấy, HN', 1),
(3, 3, 2, CAST(N'2024-12-05T08:15:00.000' AS DateTime), CAST(105000.00 AS Decimal(18, 2)), 1, N'100 Phan Văn Trị, Bình Thạnh', 0),
(4, 1, 1, CAST(N'2024-12-10T11:45:00.000' AS DateTime), CAST(96000.00 AS Decimal(18, 2)), 3, N'200 Nguyễn Xí, HCM', 0)
SET IDENTITY_INSERT [dbo].[tblHoaDon] OFF
GO

-- -----------------------------------------------------------------------------
-- 4.10 Dữ liệu tblChiTietHoaDon
-- -----------------------------------------------------------------------------
INSERT [dbo].[tblChiTietHoaDon] ([MaHD], [MaSP], [SoLuong], [GiaBan]) VALUES 
(1, 1, 2, CAST(45000.00 AS Decimal(18, 2))),
(2, 2, 1, CAST(35000.00 AS Decimal(18, 2))),
(3, 3, 1, CAST(50000.00 AS Decimal(18, 2))),
(3, 5, 1, CAST(55000.00 AS Decimal(18, 2))),
(4, 4, 2, CAST(48000.00 AS Decimal(18, 2)))
GO

-- -----------------------------------------------------------------------------
-- 4.11 Dữ liệu BinhLuan
-- -----------------------------------------------------------------------------
INSERT INTO [dbo].[BinhLuan] ([MaSP], [HoTen], [NoiDung], [SoSao], [Ngay]) VALUES 
-- Bình luận cho sản phẩm 1-6
(1, N'Nguyễn Văn A', N'Cà phê rất thơm, vị đậm đà đúng gu mình.', 5, GETDATE()),
(1, N'Lê Thị B', N'Không gian quán đẹp nhưng cà phê hơi ngọt so với mình.', 4, GETDATE()),
(2, N'Trần Minh K', N'Espresso nguyên chất rất tuyệt vời!', 5, GETDATE()),
-- Bình luận cho sản phẩm 7-15
(7, N'Lan Vy', N'Bạc xỉu vừa miệng, phục vụ nhanh.', 5, GETDATE()),
(7, N'Lan Vy', N'Bạc xỉu vừa miệng, không quá ngọt.', 4, CAST(N'2025-10-01T10:00:00.000' AS DateTime)),
(8, N'Đức Tài', N'Trà sữa khoai môn thơm béo, trân châu ngon.', 5, CAST(N'2025-10-02T11:00:00.000' AS DateTime)),
(9, N'Thanh Nga', N'Nước chanh tươi mát, giải khát rất đã.', 5, CAST(N'2025-10-03T12:00:00.000' AS DateTime)),
(10, N'Minh Quân', N'Bánh su kem mềm và nhân nhiều.', 4, CAST(N'2025-10-04T13:00:00.000' AS DateTime)),
(11, N'Quốc Hùng', N'Cold Brew rất chất lượng, đậm vị cà phê.', 5, CAST(N'2025-10-05T14:00:00.000' AS DateTime)),
(12, N'Ngọc Ánh', N'Trà Atiso đỏ chua ngọt nhẹ, màu đẹp.', 4, CAST(N'2025-10-06T15:00:00.000' AS DateTime)),
(13, N'Hoàng Nam', N'Sinh tố dâu tằm tươi, giá hơi cao nhưng xứng đáng.', 5, CAST(N'2025-10-07T16:00:00.000' AS DateTime)),
(14, N'Thu Thảo', N'Bánh Brownie ẩm, vị socola đậm đà.', 5, CAST(N'2025-10-08T17:00:00.000' AS DateTime)),
(15, N'Việt Anh', N'Espresso Macchiato chuẩn vị, lớp bọt sữa hoàn hảo.', 4, CAST(N'2025-10-09T18:00:00.000' AS DateTime))
GO

-- =============================================================================
-- KẾT THÚC SCRIPT
-- =============================================================================
