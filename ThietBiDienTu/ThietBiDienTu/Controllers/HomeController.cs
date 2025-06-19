using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThietBiDienTu.Models;
using PagedList;

namespace ThietBiDienTu.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        //string connectionString = @"Server=SHINICHIKUTIEN;Database=ThietBiDienTu_Moi_8386;User Id = sa; Password=123;";

        DataClasses1DataContext data = new DataClasses1DataContext();

        //public HomeController()
        //{
        //    data = new DataClasses1DataContext(connectionString);
        //}

        public ActionResult Index(int? page, string ml)
        {
            int pageSize = 8;
            int pageNumber = (page ?? 1);

            List<SanPham> dsSP = data.SanPhams.ToList();
            if (ml != null)
            {
                dsSP = data.SanPhams.Where(t => t.MaLoai == ml).ToList();
            }

            foreach (var sp in dsSP)
            {
                // Split chuỗi hình ảnh và lấy ảnh đầu tiên
                var danhSachHinhAnh = sp.HinhAnh.Split(new string[] { ", " }, StringSplitOptions.None).ToList();
                sp.HinhAnh = danhSachHinhAnh.FirstOrDefault();  // Lấy ảnh đầu tiên
            }

            return View(dsSP.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult hienThidanhMuc()
        {
            List<LoaiSanPham> dsDM = data.LoaiSanPhams.ToList();
            return PartialView(dsDM);        
        }

        public ActionResult chiTietSanPham(string msp)
        {
            var sp = data.SanPhams.FirstOrDefault(t => t.MaSP == msp);

            if (sp == null)
            {
                return HttpNotFound();
            }

            List<string> danhSachHinhAnh = sp.HinhAnh.Split(new string[] { ", " }, StringSplitOptions.None).ToList();
            ViewBag.DanhSachHinhAnh = danhSachHinhAnh;

            var dsCungLoai = data.SanPhams.Where(t => t.MaLoai == sp.MaLoai && t.MaSP != msp).Take(5).ToList();

            ViewBag.spLienQuan = dsCungLoai;

            foreach (var sp1 in dsCungLoai)
            {
                var danhSachHinhAnh1 = sp1.HinhAnh.Split(new string[] { ", " }, StringSplitOptions.None).ToList();
                sp1.HinhAnh = danhSachHinhAnh1.FirstOrDefault();  // Lấy ảnh đầu tiên
            }

            var dsDanhGia = (from dg in data.DanhGias
                             join kh in data.KhachHangs on dg.MaKH equals kh.MaKH
                             where dg.MaSP == sp.MaSP
                             select new ChiTietDanhGIa
                             {
                                 MaDanhGia = dg.MaDanhGia,
                                 TenKH = kh.HoTen,
                                 NgayDanhGia = dg.NgayDanhGia.ToString(),
                                 NoiDung = dg.NoiDung,
                                 MaKH = kh.MaKH
                             }).ToList();

            ViewBag.dsDanhGia = dsDanhGia;
            return View(sp);
        }

        public ActionResult trangSanPham(int? page, string searchText, string locSapXep)
        {
            int pageSize = 12;
            int pageNumber = (page ?? 1);

            var dsSP = data.SanPhams.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                var searchTerms = searchText.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                           .Select(term => term.Trim().ToLower())
                                           .ToList();

                dsSP = dsSP.ToList().AsQueryable()
                           .Where(s => searchTerms.Any(term => s.TenSP.ToLower().Contains(term)));
            }

            if (!dsSP.Any())
            {
                ViewBag.Message = "Không tìm thấy sản phẩm nào phù hợp với từ khóa tìm kiếm!";
            }

            foreach (var sp in dsSP)
            {
                var danhSachHinhAnh = sp.HinhAnh.Split(new string[] { ", " }, StringSplitOptions.None).ToList();
                sp.HinhAnh = danhSachHinhAnh.FirstOrDefault();
            }

            switch (locSapXep)
            {
                case "name_AtoZ":
                    dsSP = dsSP.OrderBy(s => s.TenSP);
                    break;
                case "name_ZtoA":
                    dsSP = dsSP.OrderByDescending(s => s.TenSP);
                    break;
                case "price_Tang":
                    dsSP = dsSP.OrderBy(s => s.GiaBan);
                    break;
                case "price_Giam":
                    dsSP = dsSP.OrderByDescending(s => s.GiaBan);
                    break;
                default:
                    dsSP = dsSP.OrderBy(s => s.TenSP);
                    break;
            }

            return View(dsSP.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult danhGiaSanPham(FormCollection form, string msp)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("DangNhap", "Home");
            }

            string madg = "DG" + DateTime.Now.ToString("ddMMyyyy");
            var danhGiaCuoi = data.DanhGias
                                 .Where(dg => dg.MaDanhGia.StartsWith(madg))
                                 .OrderByDescending(dg => dg.MaDanhGia)
                                 .Select(dg => dg.MaDanhGia)
                                 .FirstOrDefault();

            if (danhGiaCuoi == null)
            {
                madg += "001";
            }
            else
            {
                int currentNum = int.Parse(danhGiaCuoi.Substring(10));
                madg += (currentNum + 1).ToString("D3");
            }

            string makh = Session["UserID"].ToString();
            string noiDung = form["NoiDung"];

            if (string.IsNullOrEmpty(noiDung))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập nội dung đánh giá!";
                return RedirectToAction("chiTietSanPham", new { msp });
            }

            DanhGia danhGiaMoi = new DanhGia
            {
                MaDanhGia = madg,
                MaKH = makh,
                MaSP = msp,
                NgayDanhGia = DateTime.Now,
                NoiDung = noiDung
            };

            data.DanhGias.InsertOnSubmit(danhGiaMoi);
            data.SubmitChanges();

            TempData["Message"] = HttpUtility.HtmlDecode("Đánh giá sản phẩm thành công!");
            return RedirectToAction("chiTietSanPham", new { msp });
        }


        public ActionResult XoaDanhGia(string madg)
        {
            var userId = Session["UserID"];
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var danhGia = data.DanhGias.FirstOrDefault(t => t.MaDanhGia == madg);

            if (danhGia.MaKH != userId.ToString())
            {
                return RedirectToAction("chiTietSanPham", "Home", new { msp = danhGia.MaSP });
            }

            data.DanhGias.DeleteOnSubmit(danhGia);
            data.SubmitChanges();

            TempData["Message"] = HttpUtility.HtmlDecode("Đánh giá đã được xóa thành công!");
            return RedirectToAction("chiTietSanPham", "Home", new { msp = danhGia.MaSP });
        }

    }
}
