using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThietBiDienTu.Models;

namespace ThietBiDienTu.Controllers
{
    public class DatHangController : Controller
    {

        //string connectionString = @"Server=SHINICHIKUTIEN;Database=ThietBiDienTu_Moi_8386;User Id = sa; Password=123;";

        DataClasses1DataContext data = new DataClasses1DataContext();

        //public DatHangController()
        //{
        //    data = new DataClasses1DataContext(connectionString);
        //}

        public ActionResult ThemMatHang(string msp, string maKH)
        {
            if(Session["UserID"] == null)
            {
                return RedirectToAction("DangNhap", "Home");
            }

            var gioHang = data.GioHangs.FirstOrDefault(gh => gh.MaKH == maKH);

            if (gioHang == null)
            {
                gioHang = new GioHang
                {
                    MaKH = maKH.Trim(),
                    NgayTao = DateTime.Now,
                    MaGioHang = "GH" + maKH
                };
                data.GioHangs.InsertOnSubmit(gioHang);
                data.SubmitChanges(); // Lưu giỏ hàng mới vào cơ sở dữ liệu
            }

            var chiTiet = data.ChiTietGioHangs.FirstOrDefault(ct => ct.MaGioHang == gioHang.MaGioHang && ct.MaSP == msp);

            if (chiTiet == null)
            {
                chiTiet = new ChiTietGioHang
                {
                    MaGioHang = gioHang.MaGioHang,
                    MaSP = msp,
                    SoLuong = 1 // Thêm 1 sản phẩm vào giỏ hàng
                };
                data.ChiTietGioHangs.InsertOnSubmit(chiTiet);
            }
            else
            {
                chiTiet.SoLuong++;
            }

            data.SubmitChanges();

            return Json(new { success = true, message = "Đã thêm vào giỏ hàng!" });
        }

        public ActionResult XemGioHang(string maKH)
        {
            var gioHang = data.GioHangs.FirstOrDefault(gh => gh.MaKH == maKH);
            if (gioHang == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.MaKH = maKH;

            var chiTietGioHang = data.ChiTietGioHangs
                                      .Where(ct => ct.MaGioHang == gioHang.MaGioHang)
                                      .ToList();

            foreach (var item in chiTietGioHang)
            {
                var sanPham = data.SanPhams.FirstOrDefault(sp => sp.MaSP == item.MaSP);
                if (sanPham != null)
                {
                    var danhSachHinhAnh = sanPham.HinhAnh.Split(new string[] { ", " }, StringSplitOptions.None);
                    sanPham.HinhAnh = danhSachHinhAnh.FirstOrDefault(); // Lấy ảnh đầu tiên
                    item.SanPham = sanPham;

                }
            }

            var tongGia = chiTietGioHang.Sum(item => item.SoLuong * item.SanPham.GiaBan);

            ViewBag.TongGia = tongGia;

            return View(chiTietGioHang);
        }

        public ActionResult ChinhSuaGioHang(string maKH)
        {
            var gioHang = data.GioHangs.FirstOrDefault(gh => gh.MaKH == maKH);
            if (gioHang == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.MaKH = maKH;

            var chiTietGioHang = data.ChiTietGioHangs
                                      .Where(ct => ct.MaGioHang == gioHang.MaGioHang)
                                      .ToList();

            foreach (var item in chiTietGioHang)
            {
                var sanPham = data.SanPhams.FirstOrDefault(sp => sp.MaSP == item.MaSP);
                if (sanPham != null)
                {
                    var danhSachHinhAnh = sanPham.HinhAnh.Split(new string[] { ", " }, StringSplitOptions.None);
                    sanPham.HinhAnh = danhSachHinhAnh.FirstOrDefault();
                    item.SanPham = sanPham;
                }
            }

            return View(chiTietGioHang);
        }

       [HttpPost]
    public ActionResult CapNhatGioHang(string maKH, List<ChiTietGioHang> chiTietCapNhat)
    {
        if (chiTietCapNhat == null || !chiTietCapNhat.Any())
        {
            return RedirectToAction("ChinhSuaGioHang", new { maKH });
        }

        var gioHang = data.GioHangs.FirstOrDefault(gh => gh.MaKH == maKH);
        if (gioHang == null)
        {
            return RedirectToAction("Index", "Home");
        }

        List<string> danhSachSanPhamCapNhat = new List<string>();

        foreach (var chiTiet in chiTietCapNhat)
        {
            var item = data.ChiTietGioHangs.FirstOrDefault(ct => ct.MaGioHang == gioHang.MaGioHang && ct.MaSP == chiTiet.MaSP);
            if (item != null)
            {
                if (chiTiet.SoLuong > 0)
                {
                    if (item.SoLuong != chiTiet.SoLuong)
                    {
                        item.SoLuong = chiTiet.SoLuong; // Cập nhật số lượng sản phẩm
                        danhSachSanPhamCapNhat.Add(item.SanPham.TenSP);
                    }
                }
                else
                {
                    data.ChiTietGioHangs.DeleteOnSubmit(item); // Xóa sản phẩm khỏi giỏ hàng nếu số lượng bằng 0
                    danhSachSanPhamCapNhat.Add("Xóa " + item.SanPham.TenSP);
                }
            }
        }

        data.SubmitChanges();

        if (danhSachSanPhamCapNhat.Any())
        {
            TempData["Message"] = "Đã cập nhật giỏ hàng: " + string.Join(", ", danhSachSanPhamCapNhat);
        }

        return RedirectToAction("XemGioHang", new { maKH });
    }
        
        public ActionResult DatHang()
{
    string maKH = Session["UserID"].ToString();
    if (string.IsNullOrEmpty(maKH))
    {
        return RedirectToAction("DangNhap", "DNDK"); // Nếu chưa đăng nhập, chuyển đến trang đăng nhập
    }

    var gioHang = data.GioHangs.FirstOrDefault(gh => gh.MaKH == maKH);
    if (gioHang == null || !data.ChiTietGioHangs.Any(ct => ct.MaGioHang == gioHang.MaGioHang))
    {
        TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm trước khi đặt hàng!";
        return RedirectToAction("XemGioHang", "DatHang", new { maKH }); // Chuyển thẳng đến giỏ hàng
    }

    KhachHang kh = data.KhachHangs.FirstOrDefault(t => t.MaKH == maKH);
    return View(kh);
}

        [HttpPost]
    public ActionResult DatHang(FormCollection c)
    {
        decimal tongTien = 0;
        string maKH = c["maKH"];

        var gioHang = data.GioHangs.FirstOrDefault(gh => gh.MaKH == maKH);
        if (gioHang == null)
        {
            TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm trước khi đặt hàng!";
            return RedirectToAction("XemGioHang", "DatHang", new { maKH });
        }

        var chiTietDonHang = data.ChiTietGioHangs.Where(ct => ct.MaGioHang == gioHang.MaGioHang).ToList();

        if (!chiTietDonHang.Any())
        {
            TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm trước khi đặt hàng!";
            return RedirectToAction("XemGioHang", "DatHang", new { maKH });
        }

        string diaChiNhanHang = c["diaChiNhanHang"];
        if (string.IsNullOrEmpty(diaChiNhanHang))
        {
            TempData["ErrorMessage"] = "Vui lòng nhập địa chỉ nhận hàng trước khi đặt hàng!";
            return RedirectToAction("DatHang");
        }

        // Tiếp tục tạo đơn hàng nếu có sản phẩm trong giỏ
        string mpn = "DH" + DateTime.Now.ToString("ddMMyyyy");
        var maphieucuoicung = data.DonHangs
            .Where(d => d.MaDonHang.StartsWith(mpn))
            .OrderByDescending(d => d.MaDonHang)
            .Select(d => d.MaDonHang)
            .FirstOrDefault();

        if (string.IsNullOrEmpty(maphieucuoicung))
        {
            mpn += "001";
        }
        else
        {
            string stt = maphieucuoicung.Substring(10);
            int sttInt = int.Parse(stt) + 1;
            mpn += sttInt.ToString("D3");
        }

        var donHang = new DonHang
        {
            MaDonHang = mpn,
            NgayDat = DateTime.Now,
            TinhTrangGiaoHang = "Đang xử lý",
            MaKH = maKH
        };

        data.DonHangs.InsertOnSubmit(donHang);

        foreach (var item in chiTietDonHang)
        {
            var sanPham = data.SanPhams.FirstOrDefault(sp => sp.MaSP == item.MaSP);
            if (sanPham != null)
            {
                if (sanPham.SoLuongTon >= item.SoLuong)
                {
                    tongTien += decimal.Parse((item.SoLuong * sanPham.GiaBan).ToString());
                    sanPham.SoLuongTon -= item.SoLuong;

                    var chiTietDonHangItem = new ChiTietDonHang
                    {
                        MaDonHang = mpn,
                        MaSP = item.MaSP,
                        SoLuong = item.SoLuong,
                        DonGia = sanPham.GiaBan
                    };

                    data.ChiTietDonHangs.InsertOnSubmit(chiTietDonHangItem);
                }
                else
                {
                    TempData["ErrorMessage"] = "Sản phẩm " + sanPham.TenSP + " không đủ số lượng tồn trong kho.";
                    return RedirectToAction("XemGioHang", "DatHang", new { maKH });
                }
            }
        }

        var thongTinDatHang = new ThongTinDatHang
        {
            TenNguoiNhan = c["tenNguoiNhan"],
            DiaChiNhanHang = c["diaChiNhanHang"],
            SoDienThoai = c["soDienThoai"],
            GhiChu = c["ghiChu"],
            MaDonHang = mpn
        };

        donHang.TongTien = tongTien;

        data.ThongTinDatHangs.InsertOnSubmit(thongTinDatHang);
        data.SubmitChanges();

        TempData["Message"] = "Đơn hàng của bạn đã được đặt thành công!";
        return RedirectToAction("ThanhToan");
    }

        public ActionResult ThanhToan()
        {
            return View();
        }

        public ActionResult XacNhanThanhToan()
        {
            var maKH = Session["UserID"].ToString();
            var gioHang = data.GioHangs.FirstOrDefault(gh => gh.MaKH == maKH);
            if (gioHang != null)
            {
                var chiTietGioHang = data.ChiTietGioHangs
                                          .Where(ct => ct.MaGioHang == gioHang.MaGioHang)
                                          .ToList();
                data.ChiTietGioHangs.DeleteAllOnSubmit(chiTietGioHang);

                data.GioHangs.DeleteOnSubmit(gioHang);

                data.SubmitChanges();
            }
            return View();
        }

        public ActionResult LichSuDatHang()
        {
            if(Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var maKH = Session["UserID"].ToString();

            List<DonHang> danhSachDonHang = data.DonHangs.Where(dh => dh.MaKH == maKH).ToList();

            foreach (var donHang in danhSachDonHang)
            {
                var chiTietDonHang = data.ChiTietDonHangs.Where(ctdh => ctdh.MaDonHang == donHang.MaDonHang).ToList();

                donHang.TongTien = chiTietDonHang.Sum(ctdh => ctdh.SoLuong * ctdh.DonGia);
            }

            return View(danhSachDonHang);
        }

        public ActionResult ChiTietLichSuDatHang(string maDH)
        {
            var chiTietDonHang = data.ChiTietDonHangs.Where(ctdh => ctdh.MaDonHang == maDH).ToList();

            foreach (var item in chiTietDonHang)
            {
                var danhSachHinhAnh = item.SanPham.HinhAnh.Split(new string[] { ", " }, StringSplitOptions.None);
                item.SanPham.HinhAnh = danhSachHinhAnh.FirstOrDefault();
            }

            return View(chiTietDonHang);
        }

        public ActionResult HuyDon(string madh)
        {
            var chiTietDonHang = data.ChiTietDonHangs.Where(ct => ct.MaDonHang == madh);
            var thongTinDatHang = data.ThongTinDatHangs.Where(t => t.MaDonHang == madh);
            var donHang = data.DonHangs.Where(dh => dh.MaDonHang == madh);

            data.ChiTietDonHangs.DeleteAllOnSubmit(chiTietDonHang);
            data.ThongTinDatHangs.DeleteAllOnSubmit(thongTinDatHang);
            data.DonHangs.DeleteAllOnSubmit(donHang);

            data.SubmitChanges();
            return RedirectToAction("LichSuDatHang");
        }
    }
}