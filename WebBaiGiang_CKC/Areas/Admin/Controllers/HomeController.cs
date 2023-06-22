using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using WebBaiGiang_CKC.Data;
using WebBaiGiang_CKC.Models;

namespace WebBaiGiang_CKC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly WebBaiGiangContext _context;
        public HomeController(WebBaiGiangContext context)
        {
            _context = context;

        }

        //public IActionResult Index(int? kyKiemTraId)
        //{
        //    // Lấy danh sách các kỳ kiểm tra từ cột KyKiemTra trong bảng BaiLam
        //    var kyKiemTraList = _context.KyKiemTra.ToList();

        //    // Nếu không có kỳ kiểm tra được chọn, lấy kỳ kiểm tra đầu tiên
        //    if (kyKiemTraId == null)
        //    {
        //        kyKiemTraId = kyKiemTraList.FirstOrDefault()?.KyKiemTraId;
        //    }

        //    // Lấy thông tin kỳ kiểm tra được chọn
        //    var kyKiemTra = _context.KyKiemTra.FirstOrDefault(x => x.KyKiemTraId == kyKiemTraId);

        //    // Lấy danh sách bài làm của kỳ kiểm tra được chọn
        //    var baiLamList = _context.BaiLam
        //        .Include(x => x.CauHoi_BaiLam)
        //        .ThenInclude(x => x.CauHoi_De)
        //        .ThenInclude(x => x.De)
        //        .Where(x => x.CauHoi_BaiLam.FirstOrDefault().CauHoi_De.De.KyKiemTraId == kyKiemTraId)
        //        .ToList();

        //    // Tính toán số lượng sinh viên và tỷ lệ phần trăm sinh viên trong mỗi khoảng điểm
        //    var count0to5 = baiLamList.Count(s => s.Diem >= 0 && s.Diem < 5);
        //    var count5to7 = baiLamList.Count(s => s.Diem >= 5 && s.Diem < 7);
        //    var count7to8 = baiLamList.Count(s => s.Diem >= 7 && s.Diem < 8);
        //    var count8to10 = baiLamList.Count(s => s.Diem >= 8 && s.Diem <= 10);

        //    var total = baiLamList.Count;

        //    var percent0to5 = Math.Round((double)count0to5 / total * 100, 2);
        //    var percent5to7 = Math.Round((double)count5to7 / total * 100, 2);
        //    var percent7to8 = Math.Round((double)count7to8 / total * 100, 2);
        //    var percent8to10 = Math.Round((double)count8to10 / total * 100, 2);

        //    // Lưu trữ dữ liệu của kỳ kiểm tra vào ViewBag để truyền vào view
        //    ViewBag.KyKiemTraList = kyKiemTraList;
        //    ViewBag.KyKiemTraId = kyKiemTraId;
        //    ViewBag.KyKiemTraTen = kyKiemTra?.TenKyKiemTra;
        //    ViewBag.Percent0to5 = percent0to5;
        //    ViewBag.Percent5to7 = percent5to7;
        //    ViewBag.Percent7to8 = percent7to8;
        //    ViewBag.Percent8to10 = percent8to10;
        //    ViewBag.Diem0to5 = count0to5;
        //    ViewBag.Diem5to7 = count5to7;
        //    ViewBag.Diem7to8 = count7to8;
        //    ViewBag.Diem8to10 = count8to10;

        //    return View();
        //}

        public IActionResult Index()
        {
            var kyKiemTraList = GetKyKiemTraList();
            ViewBag.KyKiemTraList = kyKiemTraList;

            var kyKiemTra = kyKiemTraList.FirstOrDefault();
            var kyKiemTraId = kyKiemTra?.KyKiemTraId ?? 0;
            var baiLamList = GetBaiLamList(kyKiemTraId);
            var studentCount = GetStudentCount(baiLamList);
            var percent0to5 = GetPercent0to5(kyKiemTraId, baiLamList, studentCount);
            var percent5to7 = GetPercent5to7(kyKiemTraId, baiLamList, studentCount);
            var percent7to8 = GetPercent7to8(kyKiemTraId, baiLamList, studentCount);
            var percent8to10 = GetPercent8to10(kyKiemTraId, baiLamList, studentCount);

            ViewBag.Percent0to5 = percent0to5;
            ViewBag.Percent5to7 = percent5to7;
            ViewBag.Percent7to8 = percent7to8;
            ViewBag.Percent8to10 = percent8to10;

            return View();
        }

        private List<BaiLam> GetBaiLamList(int kyKiemTraId)
        {
            return _context.BaiLam
                .Include(x => x.CauHoi_BaiLam)
                .ThenInclude(x => x.CauHoi_De)
                .ThenInclude(x => x.De)
                .Where(x => x.CauHoi_BaiLam.FirstOrDefault().CauHoi_De.De.KyKiemTraId == kyKiemTraId)
                .ToList();
        }
        private List<KyKiemTra> GetKyKiemTraList()
        {
            return _context.KyKiemTra.ToList();
        }

        private int GetStudentCount(List<BaiLam> baiLamList)
        {
            return baiLamList.Select(x => x.MSSV).Distinct().Count();
        }

        private double GetPercent0to5(int kyKiemTraId, List<BaiLam> baiLamList, int studentCount)
        {
            return (double)baiLamList.Count(x => x.Diem >= 0 && x.Diem < 5 && x.CauHoi_BaiLam.FirstOrDefault().CauHoi_De.De.KyKiemTraId == kyKiemTraId) / studentCount * 100;
        }

        private double GetPercent5to7(int kyKiemTraId, List<BaiLam> baiLamList, int studentCount)
        {
            return (double)baiLamList.Count(x => x.Diem >= 5 && x.Diem < 7 && x.CauHoi_BaiLam.FirstOrDefault().CauHoi_De.De.KyKiemTraId == kyKiemTraId) / studentCount * 100;
        }

        private double GetPercent7to8(int kyKiemTraId, List<BaiLam> baiLamList, int studentCount)
        {
            return (double)baiLamList.Count(x => x.Diem >= 7 && x.Diem < 8 && x.CauHoi_BaiLam.FirstOrDefault().CauHoi_De.De.KyKiemTraId == kyKiemTraId) / studentCount * 100;
        }

        private double GetPercent8to10(int kyKiemTraId, List<BaiLam> baiLamList, int studentCount)
        {
            return (double)baiLamList.Count(x => x.Diem >= 8 && x.Diem <= 10 && x.CauHoi_BaiLam.FirstOrDefault().CauHoi_De.De.KyKiemTraId == kyKiemTraId) / studentCount * 100;
        }


    }
}
