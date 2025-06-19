using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThietBiDienTu.Models;
using PagedList;
using PagedList.Mvc;

namespace ThietBiDienTu.Controllers
{
    public class NhanVienController : Controller
    {
        //
        // GET: /NhanVien/

        DataClasses1DataContext data = new DataClasses1DataContext();

        public ActionResult Index(int? page)
        {
            int pageSize = 8;
            int pageNumber = (page ?? 1);

            var danhSach = data.SanPhams.OrderBy(sp => sp.NgayCapNhat).ToPagedList(pageNumber, pageSize);

            foreach (var sp in danhSach)
            {
                var danhSachHinhAnh = sp.HinhAnh.Split(new string[] { ", " }, StringSplitOptions.None).ToList();
                sp.HinhAnh = danhSachHinhAnh.FirstOrDefault();
            }


            return View(danhSach);
        }

        public ActionResult ThemSanPham()
        {
            var lastProduct = data.SanPhams.OrderByDescending(x => x.MaSP).FirstOrDefault();

            int nextNumber = 1;
            if (lastProduct != null)
            {
                string lastNumber = lastProduct.MaSP.Substring(2);
                nextNumber = int.Parse(lastNumber) + 1;
            }

            string newMaSP = "SP" + nextNumber;

            ViewBag.MaSP = newMaSP;

            ViewBag.MaLoai = new SelectList(data.LoaiSanPhams, "MaLoai", "TenLoai");
            ViewBag.MaNCC = new SelectList(data.NhaCungCaps, "MaNCC", "TenNCC");

            return View();
        }

        [HttpPost]
        public ActionResult ThemSanPham(SanPham sp, HttpPostedFileBase HinhAnh)
        {
            if (ModelState.IsValid)
            {
                if (HinhAnh != null && HinhAnh.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(HinhAnh.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/HinhAnhSP"), fileName);
                    HinhAnh.SaveAs(path);
                    sp.HinhAnh = Path.GetFileNameWithoutExtension(fileName);
                }

                data.SanPhams.InsertOnSubmit(sp);
                data.SubmitChanges();

                return RedirectToAction("Index");
            }

            ViewBag.MaLoai = new SelectList(data.LoaiSanPhams, "MaLoai", "TenLoai", sp.MaLoai);
            ViewBag.MaNCC = new SelectList(data.NhaCungCaps, "MaNCC", "TenNCC", sp.MaNCC);
            return View(sp);
        }

        public ActionResult SuaSanPham(string masp)
        {
            var sp = data.SanPhams.SingleOrDefault(s => s.MaSP == masp);

            ViewBag.MaLoai = new SelectList(data.LoaiSanPhams, "MaLoai", "TenLoai", sp.MaLoai);
            ViewBag.MaNCC = new SelectList(data.NhaCungCaps, "MaNCC", "TenNCC", sp.MaNCC);

            return View(sp);
        }

        [HttpPost]
        public ActionResult SuaSanPham(SanPham model, HttpPostedFileBase HinhAnh)
        {
            if (ModelState.IsValid)
            {
                var sanPham = data.SanPhams.SingleOrDefault(sp => sp.MaSP == model.MaSP);

                sanPham.TenSP = model.TenSP;
                sanPham.GiaBan = model.GiaBan;
                sanPham.MoTa = model.MoTa;
                sanPham.MaLoai = model.MaLoai;
                sanPham.MaNCC = model.MaNCC;

                if (HinhAnh != null)
                {
                    var imagePath = Path.Combine(Server.MapPath("~/Images"), HinhAnh.FileName);
                    HinhAnh.SaveAs(imagePath);
                    sanPham.HinhAnh = HinhAnh.FileName;
                }

                data.SubmitChanges();

                return RedirectToAction("Index");
            }

            ViewBag.MaLoai = new SelectList(data.LoaiSanPhams, "MaLoai", "TenLoai", model.MaLoai);
            ViewBag.MaNCC = new SelectList(data.NhaCungCaps, "MaNCC", "TenNCC", model.MaNCC);

            return View(model);
        }

        [HttpPost]
        public ActionResult XoaSanPham(string masp)
        {
            var sanPham = data.SanPhams.SingleOrDefault(sp => sp.MaSP == masp);

            data.SanPhams.DeleteOnSubmit(sanPham);

            data.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult DsKhachHang()
        {
            var dsKhachHang = data.KhachHangs
                .OrderBy(kh => kh.MaKH)
                .ToList();

            return View(dsKhachHang);
        }

        public ActionResult ThemKhachHang()
        {
            var lastCustomer = data.KhachHangs.OrderByDescending(x => x.MaKH).FirstOrDefault();

            int nextNumber = 1;  // Mặc định mã khách hàng bắt đầu từ 1
            if (lastCustomer != null)
            {
                string lastNumber = lastCustomer.MaKH.Substring(2);
                nextNumber = int.Parse(lastNumber) + 1;
            }

            string newMaKH = "KH" + (nextNumber < 100 ? nextNumber.ToString("D2") : nextNumber.ToString("D3"));

            ViewBag.MaKH = newMaKH;

            return View();
        }

        [HttpPost]
        public ActionResult ThemKhachHang(KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                data.KhachHangs.InsertOnSubmit(kh);
                data.SubmitChanges();
                return RedirectToAction("DsKhachHang");
            }
            return View(kh);
        }

        public ActionResult SuaKhachHang(string makh)
        {
            var kh = data.KhachHangs.FirstOrDefault(k => k.MaKH == makh);
            return View(kh);
        }

        [HttpPost]
        public ActionResult SuaKhachHang(KhachHang kh)
        {
            var existingKh = data.KhachHangs.FirstOrDefault(k => k.MaKH == kh.MaKH);
            if (existingKh != null)
            {
                existingKh.HoTen = kh.HoTen;
                existingKh.NgaySinh = kh.NgaySinh;
                existingKh.GioiTinh = kh.GioiTinh;
                existingKh.DienThoai = kh.DienThoai;
                existingKh.Email = kh.Email;
                data.SubmitChanges();
            }
            return RedirectToAction("DsKhachHang");
        }

        public ActionResult XoaKhachHang(string makh)
        {
            var danhGia = data.DanhGias.Where(dg => dg.MaKH == makh);
            data.DanhGias.DeleteAllOnSubmit(danhGia);

            var khachHang = data.KhachHangs.FirstOrDefault(k => k.MaKH == makh);
            if (khachHang != null)
            {
                data.KhachHangs.DeleteOnSubmit(khachHang);
                data.SubmitChanges();
            }

            return RedirectToAction("DsKhachHang");
        }

        public ActionResult DanhGiaSanPham(string masp)
        {
            ViewBag.SanPhamList = data.SanPhams.ToList();

            if (string.IsNullOrEmpty(masp))
            {
                return View(new List<DanhGia>());
            }

            var danhGiaList = data.DanhGias
                .Where(dg => dg.MaSP == masp)
                .OrderByDescending(dg => dg.NgayDanhGia)
                .ThenBy(dg => dg.MaDanhGia)
                .ToList();
            return View(danhGiaList);
        }

        [HttpPost]
        public ActionResult XoaDanhGia(string maDanhGia)
        {
            var danhGia = data.DanhGias.FirstOrDefault(dg => dg.MaDanhGia == maDanhGia);
            if (danhGia != null)
            {
                data.DanhGias.DeleteOnSubmit(danhGia);
                data.SubmitChanges();
            }

            return RedirectToAction("DanhGiaSanPham");
        }

        public ActionResult DsDonHang()
        {
            List<DonHang> dsDonHang = data.DonHangs.ToList();

            if (dsDonHang == null || !dsDonHang.Any())
            {
                return View(new List<DonHang>());
            }

            foreach (DonHang dh in dsDonHang)
            {
                dh.TongTien = data.ChiTietDonHangs.Where(ct => ct.MaDonHang == dh.MaDonHang).Sum(ct => ct.SoLuong * ct.DonGia);
            }

            return View(dsDonHang);
        }

        [HttpPost]
        public ActionResult CapNhatTinhTrang(string maDonHang)
        {
            var donHang = data.DonHangs.FirstOrDefault(dh => dh.MaDonHang == maDonHang);

            if (donHang != null && donHang.TinhTrangGiaoHang == "Đang xử lý")
            {
                donHang.TinhTrangGiaoHang = "Đang giao";
                data.SubmitChanges();
            }

            return RedirectToAction("DsDonHang");
        }

        public ActionResult XemChiTiet(string maDonHang)
        {
            var donHang = data.DonHangs.FirstOrDefault(dh => dh.MaDonHang == maDonHang);

            if (donHang == null)
            {
                return HttpNotFound("Không tìm thấy đơn hàng.");
            }
            donHang.TongTien = data.ChiTietDonHangs.Where(ct => ct.MaDonHang == donHang.MaDonHang).Sum(ct => ct.SoLuong * ct.DonGia);

            var chiTietDH = data.ChiTietDonHangs.Where(ct => ct.MaDonHang == donHang.MaDonHang);
            ViewBag.ctdh = chiTietDH;

            return View(donHang);
        }

        public ActionResult NhieuAnhSanPham(string masp)
        {
            var sp = data.SanPhams.SingleOrDefault(s => s.MaSP == masp);

            var hinhAnhs = sp.HinhAnh.Split(new[] { ", " }, StringSplitOptions.None);
            ViewBag.dsHinhAnh = hinhAnhs;
            ViewBag.MaSP = masp;

            return View();
        }

        public ActionResult ThemHinhAnh(HttpPostedFileBase file, string id)
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/HinhAnhSP/"), fileName);
                file.SaveAs(path);

                var product = data.SanPhams.SingleOrDefault(p => p.MaSP == id);
                if (product != null)
                {
                    if (!string.IsNullOrEmpty(product.HinhAnh))
                    {
                        // Lấy tên file nhưng bỏ phần mở rộng ví dụ .jpg .png
                        product.HinhAnh += ", " + Path.GetFileNameWithoutExtension(fileName);
                    }
                    else
                    {
                        product.HinhAnh = Path.GetFileNameWithoutExtension(fileName);
                    }
                    data.SubmitChanges();
                }
            }

            return RedirectToAction("NhieuAnhSanPham", new { masp = id });
        }

        public ActionResult XoaHinhAnh(string id, string tenAnh)
        {
            var product = data.SanPhams.SingleOrDefault(p => p.MaSP == id);
            if (product != null)
            {
                var hinhAnhs = product.HinhAnh.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                hinhAnhs.Remove(tenAnh);

                product.HinhAnh = string.Join(", ", hinhAnhs);

                data.SubmitChanges();

                var path = Path.Combine(Server.MapPath("~/Content/HinhAnhSP/"), tenAnh + ".jpg");
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            return RedirectToAction("NhieuAnhSanPham", new { masp = id });
        }

        public ActionResult NhapHang()
        {
            var sanPhams = data.SanPhams.ToList();
            var nhaCungCaps = data.NhaCungCaps.ToList();

            ViewBag.SanPhams = sanPhams;
            ViewBag.NhaCungCap = nhaCungCaps;

            return View();
        }

        [HttpPost]
        public ActionResult NhapHang(string masp, int soluong, decimal giaNhap, string mancc)
        {
            var product = data.SanPhams.SingleOrDefault(p => p.MaSP == masp);
            var nhaCungCap = data.NhaCungCaps.SingleOrDefault(ncc => ncc.MaNCC == mancc);

            if (product != null && nhaCungCap != null)
            {
                product.SoLuongTon += soluong;

                string currentDate = DateTime.Now.ToString("ddMMyyyy");
                var lastNhapHang = data.NhapHangs
                    .Where(nh => nh.MaNhap.StartsWith("NH" + currentDate))
                    .OrderByDescending(nh => nh.MaNhap)
                    .FirstOrDefault();

                string lastNhapHangCode = string.Empty;
                if (lastNhapHang != null)
                {
                    lastNhapHangCode = lastNhapHang.MaNhap;
                }

                int nextNumber = 1;
                if (!string.IsNullOrEmpty(lastNhapHangCode))
                {
                    string lastNumber = lastNhapHangCode.Substring(8);
                    nextNumber = int.Parse(lastNumber) + 1;
                }
                string newNhapHangCode = "NH" + currentDate + nextNumber.ToString("D3");

                var nhapHang = new NhapHang
                {
                    MaNhap = newNhapHangCode,
                    MaNCC = mancc,
                    MaNV = Session["UserID"].ToString(),
                    NgayNhap = DateTime.Now,
                    TongTien = soluong * giaNhap
                };

                var chiTietNhapHang = new ChiTietNhapHang
                {
                    MaNhap = newNhapHangCode,
                    MaSP = masp,
                    SoLuong = soluong,
                    DonGia = giaNhap
                };

                data.NhapHangs.InsertOnSubmit(nhapHang);
                data.ChiTietNhapHangs.InsertOnSubmit(chiTietNhapHang);
                data.SubmitChanges();

                ViewBag.Message = "Nhập hàng thành công!";
            }
            ViewBag.SanPhams = data.SanPhams.ToList();
            ViewBag.NhaCungCap = data.NhaCungCaps.ToList();

            return RedirectToAction("Index");
        }

        public ActionResult ThongKe(DateTime? fromDate, DateTime? toDate, int? page)
        {
            decimal tongTienToanBo = 0;
            if (fromDate.HasValue && toDate.HasValue)
            {
                var thongKeSanPham = data.ChiTietDonHangs
                    .Where(ct => ct.DonHang.NgayDat >= fromDate.Value && ct.DonHang.NgayDat <= toDate.Value)
                    .GroupBy(ct => ct.MaSP)
                    .Select(g => new ThongKe
                    {
                        MaSP = g.Key,
                        TenSP = data.SanPhams.FirstOrDefault(sp => sp.MaSP == g.Key).TenSP,
                        SoLuongDat = g.Sum(ct => ct.SoLuong ?? 0),
                        TongTien = g.Sum(ct => (ct.SoLuong ?? 0) * (ct.DonGia ?? 0))
                    })
                    .OrderByDescending(sp => sp.MaSP)
                    .ToPagedList(page ?? 1, 8);
                tongTienToanBo = thongKeSanPham.Sum(sp => sp.TongTien);
                ViewBag.TongTienToanBo = tongTienToanBo;
                return View(thongKeSanPham);
            }
            return View(new PagedList<ThongKe>(new List<ThongKe>(), 1, 8));
        }

        public ActionResult DsNhapHang()
        {
            List<NhapHang> dsNhapHang = data.NhapHangs.OrderByDescending(nh => nh.NgayNhap).ToList();

            if (dsNhapHang == null || !dsNhapHang.Any())
            {
                return View(new List<DonHang>());
            }

            foreach (NhapHang nh in dsNhapHang)
            {
                nh.TongTien = data.ChiTietNhapHangs.Where(ct => ct.MaNhap == nh.MaNhap).Sum(ct => ct.SoLuong * ct.DonGia);
            }

            return View(dsNhapHang);
        }

        public ActionResult XemChiTietNhapHang(string maNhapHang)
        {
            var donNhapHang = data.NhapHangs.FirstOrDefault(nh => nh.MaNhap == maNhapHang);

            if (donNhapHang == null)
            {
                return HttpNotFound("Không tìm thấy đơn nhập hàng.");
            }
            donNhapHang.TongTien = data.ChiTietNhapHangs.Where(ct => ct.MaNhap == donNhapHang.MaNhap).Sum(ct => ct.SoLuong * ct.DonGia);

            var chiTietDNH = data.ChiTietNhapHangs.Where(ct => ct.MaNhap == donNhapHang.MaNhap);
            ViewBag.ctdnh = chiTietDNH;

            return View(donNhapHang);
        }
    }
}
