--drop database ThietBiDienTu

Create database Web_ThietBiDienTu
GO

use Web_ThietBiDienTu
GO

CREATE TABLE KhachHang (
    MaKH CHAR(5) NOT NULL PRIMARY KEY,
    HoTen NVARCHAR(100),
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    DienThoai NVARCHAR(15),
    MatKhau NVARCHAR(50),
    Email NVARCHAR(100),
	DiaChi NVARCHAR(255)
);
GO

CREATE TABLE NhanVien (
    MaNV CHAR(5) NOT NULL PRIMARY KEY,
    HoTen NVARCHAR(100),
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    DienThoai NVARCHAR(15),
    Email NVARCHAR(100),
    ChucVu NVARCHAR(50),
    MatKhau NVARCHAR(50)
);

CREATE TABLE LoaiSanPham (
    MaLoai CHAR(5) NOT NULL PRIMARY KEY,
    TenLoai NVARCHAR(100)
);

CREATE TABLE NhaCungCap (
    MaNCC CHAR(5) NOT NULL PRIMARY KEY,
    TenNCC NVARCHAR(100),
    DiaChi NVARCHAR(200),
    DienThoai NVARCHAR(15)
);
GO

CREATE TABLE SanPham (
    MaSP CHAR(5) NOT NULL PRIMARY KEY,
    TenSP NVARCHAR(100),
    GiaBan DECIMAL(18,0),
    MoTa NVARCHAR(MAX),
    NgayCapNhat DATE,
    HinhAnh NVARCHAR(255),
    SoLuongTon INT,
    MaLoai CHAR(5) NOT NULL,
    MaNCC CHAR(5) NOT NULL,
    FOREIGN KEY (MaLoai) REFERENCES LoaiSanPham(MaLoai),
    FOREIGN KEY (MaNCC) REFERENCES NhaCungCap(MaNCC)
);
GO

CREATE TABLE DonHang (
    MaDonHang VARCHAR(20) NOT NULL PRIMARY KEY,
    NgayDat DATE NOT NULL DEFAULT GETDATE(),
    TongTien DECIMAL,
    TinhTrangGiaoHang NVARCHAR(50) NOT NULL DEFAULT N'Đang xử lý', 
    MaKH CHAR(5) NOT NULL,
    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
);

CREATE TABLE ChiTietDonHang (
    MaDonHang VARCHAR(20) NOT NULL,
    MaSP CHAR(5) NOT NULL,
    SoLuong INT,
    DonGia DECIMAL(18,0),
    PRIMARY KEY (MaDonHang, MaSP),
    FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang),
    FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);
GO

CREATE TABLE ThongTinDatHang (
    MaDonHang VARCHAR(20) NOT NULL PRIMARY KEY,
    TenNguoiNhan NVARCHAR(100),
    DiaChiNhanHang NVARCHAR(200),
    SoDienThoai NVARCHAR(15),
    GhiChu NVARCHAR(MAX),
    FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang)
);
GO

CREATE TABLE GioHang (
    MaGioHang VARCHAR(20) NOT NULL PRIMARY KEY,
    MaKH CHAR(5) NOT NULL,
    NgayTao DATE NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
);

CREATE TABLE ChiTietGioHang (
    MaGioHang VARCHAR(20) NOT NULL,
    MaSP CHAR(5) NOT NULL,
    SoLuong INT,
    PRIMARY KEY (MaGioHang, MaSP),
    FOREIGN KEY (MaGioHang) REFERENCES GioHang(MaGioHang),
    FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);
GO

CREATE TABLE NhapHang (
    MaNhap VARCHAR(20) NOT NULL PRIMARY KEY,
    MaNCC CHAR(5) NOT NULL,
    MaNV CHAR(5) NOT NULL,
    NgayNhap DATE,
    TongTien DECIMAL(18,0),
    FOREIGN KEY (MaNCC) REFERENCES NhaCungCap(MaNCC),
    FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
);
GO

CREATE TABLE ChiTietNhapHang (
    MaNhap VARCHAR(20) NOT NULL,
    MaSP CHAR(5) NOT NULL,
    SoLuong INT,
    DonGia DECIMAL(18,0),
    PRIMARY KEY (MaNhap, MaSP),
    FOREIGN KEY (MaNhap) REFERENCES NhapHang(MaNhap),
    FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);
GO

CREATE TABLE DanhGia (
    MaDanhGia VARCHAR(20) NOT NULL PRIMARY KEY,
    MaKH CHAR(5) NOT NULL,
    MaSP CHAR(5) NOT NULL,
    NgayDanhGia DATE,
    NoiDung NVARCHAR(MAX),
    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH),
    FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);
GO


set dateformat dmy
-- Thêm dữ liệu bảng NhanVien
INSERT INTO NhanVien (MaNV, HoTen, NgaySinh, GioiTinh, DienThoai, Email, ChucVu, MatKhau) VALUES
('NV01', N'Nguyễn Văn A', '15-03-1985', N'Nam', '0123456789', 'nva@example.com', N'Quản lý', '123456'),
('NV02', N'Trần Thị B', '20-07-1990', N'Nữ', '0987654321', 'ntb@example.com', N'Nhân viên', '123456');
GO

-- Thêm dữ liệu bảng KhachHang
INSERT INTO KhachHang (MaKH, HoTen, NgaySinh, GioiTinh, DienThoai, MatKhau, Email) VALUES
('KH01', N'Lê Văn C', '10-01-1995', N'Nam', '0123456780', '123456', 'lvc@example.com'),
('KH02', N'Nguyễn Thị D', '25-05-1988', N'Nữ', '0987654322', '123456', 'ntd@example.com'),
('KH03', N'Phạm Văn E', '30-11-1992', N'Nam', '0123456781', '123456', 'pve@example.com');
GO

-- Thêm dữ liệu bảng LoaiSanPham
INSERT INTO LoaiSanPham (MaLoai, TenLoai) VALUES
('D01', N'Thiết bị gia dụng'),
('D02', N'Thiết bị điện tử'),
('D03', N'Thiết bị văn phòng'),
('D04', N'Thiết bị giải trí'),
('D05', N'Thiết bị công nghệ');
GO

-- Thêm dữ liệu bảng NhaCungCap
INSERT INTO NhaCungCap (MaNCC, TenNCC, DiaChi, DienThoai) VALUES
('NCC01', N'Tập đoàn Điện tử Samsung', N'Số 1, Đường Samsung, TP. Hồ Chí Minh', '0901234567'),
('NCC02', N'Công ty TNHH LG Electronics', N'Số 10, Đường LG, Hà Nội', '0912345678'),
('NCC03', N'Công ty Cổ phần Điện tử Sony', N'Số 20, Đường Sony, Đà Nẵng', '0923456789'),
('NCC04', N'Công ty TNHH Panasonic Việt Nam', N'Số 30, Đường Panasonic, Bình Dương', '0934567890'),
('NCC05', N'Tập đoàn Intel Việt Nam', N'Số 40, Đường Intel, Hải Phòng', '0945678901');
GO

-- Thêm dữ liệu bảng SanPham
INSERT INTO SanPham (MaSP, TenSP, GiaBan, MoTa, NgayCapNhat, HinhAnh, SoLuongTon, MaLoai, MaNCC) VALUES
-- Thiết bị gia dụng
('SP01', N'Tủ lạnh Samsung RT38K5531S8', 11990000, N'Tủ lạnh Samsung 500L', GETDATE(), 'url_image_1, url_image_1_1, url_image_1_2', 10, 'D01', 'NCC01'),
('SP02', N'Máy giặt LG T2385VS2M', 7390000, N'Máy giặt LG 8kg', GETDATE(), 'url_image_2, url_image_2_1', 15, 'D01', 'NCC02'),
('SP03', N'Máy hút bụi Electrolux ZB3314', 2799000, N'Máy hút bụi Electrolux', GETDATE(), 'url_image_3, url_image_3_1, url_image_3_2', 20, 'D01', 'NCC03'),
('SP04', N'Nồi cơm điện Panasonic SR-CEZ18', 1399000, N'Nồi cơm điện Panasonic', GETDATE(), 'url_image_4, url_image_4_1', 25, 'D01', 'NCC04'),
('SP05', N'Tivi Sony 55 inch 4K', 22990000, N'Tivi Sony 55 inch', GETDATE(), 'url_image_5, url_image_5_1', 5, 'D01', 'NCC05'),
('SP06', N'Quạt điện Mitsubishi RL40', 1200000, N'Quạt điện Mitsubishi', GETDATE(), 'url_image_6, url_image_6_1, url_image_6_2', 30, 'D01', 'NCC01'),
-- Thiết bị điện tử
('SP07', N'Điện thoại iPhone 13', 22990000, N'Điện thoại iPhone 13 128GB', GETDATE(), 'url_image_7, url_image_7_1, url_image_7_2, url_image_7_3', 12, 'D02', 'NCC02'),
('SP08', N'Laptop Dell XPS 13', 26990000, N'Máy tính xách tay Dell XPS', GETDATE(), 'url_image_8', 8, 'D02', 'NCC03'),
('SP09', N'Máy ảnh Canon EOS 200D', 16490000, N'Máy ảnh Canon EOS', GETDATE(), 'url_image_9', 7, 'D02', 'NCC04'),
('SP10', N'Loa Bluetooth JBL Charge 4', 3490000, N'Loa JBL Charge 4', GETDATE(), 'url_image_10', 15, 'D02', 'NCC05'),
('SP11', N'Máy chiếu BenQ MS550', 9490000, N'Máy chiếu BenQ', GETDATE(), 'url_image_11', 6, 'D02', 'NCC01'),
('SP12', N'Tai nghe Sony WH-1000XM4', 7990000, N'Tai nghe Sony WH-1000XM4', GETDATE(), 'url_image_12', 20, 'D02', 'NCC02'),
-- Thiết bị văn phòng
('SP13', N'Máy in HP LaserJet Pro M15w', 2199000, N'Máy in HP LaserJet', GETDATE(), 'url_image_13', 5, 'D03', 'NCC03'),
('SP14', N'Máy tính để bàn Lenovo IdeaCentre 310S', 7990000, N'Máy tính để bàn Lenovo', GETDATE(), 'url_image_14', 10, 'D03', 'NCC04'),
('SP15', N'Điện thoại Samsung Galaxy S21', 20990000, N'Điện thoại Samsung Galaxy S21', GETDATE(), 'url_image_15', 12, 'D03', 'NCC05'),
('SP16', N'Ghế văn phòng Ergonomic', 2500000, N'Ghế văn phòng Ergonomic', GETDATE(), 'url_image_16', 15, 'D03', 'NCC01'),
('SP17', N'Màn hình LED Acer R240HY', 4990000, N'Màn hình LED Acer 24 inch', GETDATE(), 'url_image_17', 8, 'D03', 'NCC02'),
('SP18', N'Tai nghe Logitech H390', 800000, N'Tai nghe Logitech H390', GETDATE(), 'url_image_18', 20, 'D03', 'NCC03'),
-- Thiết bị giải trí
('SP19', N'Console PlayStation 5', 16990000, N'Console PlayStation 5 mới nhất', GETDATE(), 'url_image_19', 10, 'D04', 'NCC04'),
('SP20', N'Console Xbox Series X', 14990000, N'Console Xbox Series X', GETDATE(), 'url_image_20', 7, 'D04', 'NCC05'),
('SP21', N'Dàn âm thanh Sony HT-G700', 9490000, N'Dàn âm thanh Sony HT-G700', GETDATE(), 'url_image_21', 5, 'D04', 'NCC01'),
('SP22', N'Máy chơi game Nintendo Switch', 8990000, N'Máy chơi game Nintendo Switch', GETDATE(), 'url_image_22', 12, 'D04', 'NCC02'),
('SP23', N'Máy chiếu ViewSonic M1', 8000000, N'Máy chiếu ViewSonic M1', GETDATE(), 'url_image_23', 6, 'D04', 'NCC03'),
('SP24', N'Tai nghe gaming Razer Kraken', 3000000, N'Tai nghe gaming Razer Kraken', GETDATE(), 'url_image_24', 15, 'D04', 'NCC04'),
-- Thiết bị công nghệ
('SP25', N'Robot hút bụi Xiaomi', 4000000, N'Robot hút bụi Xiaomi', GETDATE(), 'url_image_25', 8, 'D05', 'NCC05'),
('SP26', N'Điện thoại thông minh OnePlus 9', 16990000, N'Điện thoại OnePlus 9', GETDATE(), 'url_image_26', 5, 'D05', 'NCC01'),
('SP27', N'Tablet Lenovo Tab P11', 7000000, N'Tablet Lenovo Tab P11', GETDATE(), 'url_image_27', 10, 'D05', 'NCC02'),
('SP28', N'Máy lọc không khí Xiaomi', 3500000, N'Máy lọc không khí Xiaomi', GETDATE(), 'url_image_28', 12, 'D05', 'NCC03'),
('SP29', N'Camera hành trình GoPro Hero9', 8000000, N'Camera hành trình GoPro Hero9', GETDATE(), 'url_image_29', 4, 'D05', 'NCC04'),
('SP30', N'Smartwatch Samsung Galaxy Watch 4', 6000000, N'Smartwatch Samsung Galaxy Watch 4', GETDATE(), 'url_image_30', 8, 'D05', 'NCC05');
GO

-- Thêm dữ liệu DonHang
INSERT INTO DonHang (MaDonHang, NgayDat, MaKH) VALUES
('DH01102024001', '01-10-2024', 'KH01'),
('DH02102024002', '02-10-2024', 'KH02'),
('DH03102024003', '03-10-2024', 'KH03');
GO

-- Thêm dữ liệu ChiTietDonHang
INSERT INTO ChiTietDonHang (MaDonHang, MaSP, SoLuong, DonGia) VALUES
-- Đơn hàng DH01102024001
('DH01102024001', 'SP01', 2, 11990000),
('DH01102024001', 'SP02', 1, 7390000),
('DH01102024001', 'SP03', 1, 2799000),
-- Đơn hàng DH02102024002
('DH02102024002', 'SP04', 3, 1399000),
('DH02102024002', 'SP05', 2, 22990000),
-- Đơn hàng DH03102024003
('DH03102024003', 'SP06', 1, 1200000),
('DH03102024003', 'SP07', 2, 22990000),
('DH03102024003', 'SP08', 1, 26990000);

-- Thêm dữ liệu NhapHang
INSERT INTO NhapHang (MaNhap, MaNCC, MaNV, NgayNhap, TongTien) VALUES
('NH07112024001', 'NCC01', 'NV01', '07-11-2024',  53500000), -- Tổng tiền cho đơn hàng 1 (5 * 9500000 + 3 * 6000000)
('NH07112024002', 'NCC02', 'NV02', '07-11-2024',  5000000);   -- Tổng tiền cho đơn hàng 2 (2 * 2500000)

INSERT INTO ChiTietNhapHang (MaNhap, MaSP, SoLuong, DonGia) VALUES
('NH07112024001', 'SP01', 5, 9500000),  -- Giá nhập cho SP01
('NH07112024001', 'SP02', 3, 6000000),  -- Giá nhập cho SP02
('NH07112024002', 'SP03', 2, 2500000);  -- Giá nhập cho SP03
GO

-- Thêm dữ liệu vào bảng DanhGia
INSERT INTO DanhGia (MaDanhGia, MaKH, MaSP, NgayDanhGia, NoiDung) VALUES
('DG06112024001', 'KH01', 'SP01', '06-11-2024', N'Sản phẩm tuyệt vời, tôi rất hài lòng!'),
('DG06112024002', 'KH02', 'SP03', '06-11-2024', N'Chất lượng tốt, nhưng giao hàng hơi lâu.'),
('DG06112024003', 'KH03', 'SP02', '06-11-2024', N'Giá cả hợp lý, đáng mua.');
GO