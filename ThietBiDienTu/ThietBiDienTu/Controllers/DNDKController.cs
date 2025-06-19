using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Providers.Entities;
using ThietBiDienTu.Models;

namespace ThietBiDienTu.Controllers
{
    public class DNDKController : Controller
    {
        //
        // GET: /DNDK/
        // GET: /DatHang/
        //string connectionString = @"Server=SHINICHIKUTIEN;Database=ThietBiDienTu_Moi_8386;User Id = sa; Password=123;";

        DataClasses1DataContext data = new DataClasses1DataContext();

        //public DNDKController()
        //{
        //    data = new DataClasses1DataContext(connectionString);
        //}

        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(string tk, string mk)
        {
            if (string.IsNullOrEmpty(tk) || string.IsNullOrEmpty(mk))
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            var khachHang = data.KhachHangs.FirstOrDefault(kh => kh.DienThoai == tk || kh.Email == tk);
            var nhanVien = data.NhanViens.FirstOrDefault(nv => nv.DienThoai == tk || nv.Email == tk);

            if (khachHang == null && nhanVien == null)
            {
                ViewBag.Message = "Tài khoản không tồn tại trong hệ thống!";
                return View();
            }

            if ((khachHang != null && khachHang.MatKhau != mk) || (nhanVien != null && nhanVien.MatKhau != mk))
            {
                ViewBag.Message = "Mật khẩu không đúng!";
                return View();
            }

            // Đăng nhập thành công
            if (khachHang != null)
            {
                Session["UserName"] = khachHang.HoTen;
                Session["UserID"] = khachHang.MaKH;
                return RedirectToAction("Index", "Home");
            }

            if (nhanVien != null)
            {
                Session["UserName"] = nhanVien.HoTen;
                Session["UserID"] = nhanVien.MaNV;
                return nhanVien.ChucVu == "Quản lý"
                    ? RedirectToAction("Index", "QuanLy")
                    : RedirectToAction("Index", "NhanVien");
            }

            return View();
        }

        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(FormCollection c)
        {
            string hoTen = c["hoTen"];
            DateTime ngaySinh = DateTime.Parse(c["ngaySinh"]);
            string dienThoai = c["dienThoai"];
            string email = c["email"];
            string matKhau = c["matKhau"];
            string gioiTinh = c["gioiTinh"];

            if (string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(dienThoai) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matKhau))
            {
                ViewBag.Message = "Vui lòng điền đầy đủ thông tin!";
                return View();
            
            }

            if (matKhau.Length < 6)
            {
                ViewBag.Message = "Mật khẩu phải có ít nhất 6 ký tự!";
                return View();
            }

            var ktTK = data.KhachHangs.FirstOrDefault(kh => kh.DienThoai == dienThoai || kh.Email == email);
            if (ktTK != null)
            {
                ViewBag.Message = "Số điện thoại hoặc email đã tồn tại!";
                return View();
            }

            string maKhMoi = "KH01";
            KhachHang khachHangCuoi = data.KhachHangs.OrderByDescending(kh => kh.MaKH).FirstOrDefault();
            if (khachHangCuoi != null)
            {
                string maKhCuoi = khachHangCuoi.MaKH;
                string soCuoi = maKhCuoi.Substring(2);
                int soMoi = int.Parse(soCuoi) + 1;
                maKhMoi = "KH" + soMoi.ToString("D2");
            }

            var khachHang = new KhachHang
            {
                MaKH = maKhMoi,
                HoTen = hoTen,
                NgaySinh = ngaySinh,
                DienThoai = dienThoai,
                Email = email,
                MatKhau = matKhau,
                GioiTinh = gioiTinh
            };

            data.KhachHangs.InsertOnSubmit(khachHang);
            data.SubmitChanges();

            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("DangNhap");
        }

        public ActionResult DangXuat()
        {
            // Xóa thông tin người dùng khỏi Session
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult thongTinNguoiDung(string makh)
        {
            List<KhachHang> dsKH = data.KhachHangs.ToList();
            KhachHang khachHang = dsKH.FirstOrDefault(kh => kh.MaKH == makh);
            return View(khachHang);
        }

        public ActionResult chinhSuaThongTin(string makh)
        {
            List<KhachHang> dsKH = data.KhachHangs.ToList();
            KhachHang khachHang = dsKH.FirstOrDefault(kh => kh.MaKH == makh);

            return View(khachHang);
        }

        [HttpPost]
        public ActionResult chinhSuaThongTin(FormCollection c)
        {
            string maKH = c["MaKH"];
            string hoTen = c["HoTen"];
            DateTime ngaySinh = DateTime.Parse(c["ngaySinh"]);
            string dienThoai = c["DienThoai"];
            string email = c["Email"];
            string matKhau = c["MatKhau"];
            string gioiTinh = c["GioiTinh"];

            var khachHang = data.KhachHangs.FirstOrDefault(kh => kh.MaKH == maKH);

            if (khachHang != null)
            {
                khachHang.HoTen = hoTen;
                khachHang.NgaySinh = ngaySinh;
                khachHang.DienThoai = dienThoai;
                khachHang.Email = email;
                khachHang.MatKhau = matKhau;
                khachHang.GioiTinh = gioiTinh;

                data.SubmitChanges();
            }

            return RedirectToAction("thongTinNguoiDung", new { makh = maKH });
        }
    }
}
