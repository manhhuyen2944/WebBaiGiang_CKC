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
        public IActionResult Index(int kyKiemTraId)
        {
            // Lấy ra danh sách sinh viên tham gia kỳ kiểm tra
            var baiLamList = _context.BaiLam
               .Include(x => x.CauHoi_BaiLam).ThenInclude(x => x.CauHoi_De.De.KyKiemTra)
               .Where(x => x.CauHoi_BaiLam.Any() && x.CauHoi_BaiLam.First().CauHoi_De.De.KyKiemTraId == kyKiemTraId)
               .ToList();

            // Tính toán thống kê
            var (percent0to5, percent5to7, percent7to8, percent8to10, totalParticipants, count0to5, count5to7, count7to8, count8to10)
                = CalculateStatistics(baiLamList);

            // Pass the percentage values to the view
            ViewBag.Percent0to5 = percent0to5;
            ViewBag.Percent5to7 = percent5to7;
            ViewBag.Percent7to8 = percent7to8;
            ViewBag.Percent8to10 = percent8to10;
            ViewBag.TotalParticipants = totalParticipants;
            ViewBag.Diem0to5 = count0to5;
            ViewBag.Diem5to7 = count5to7;
            ViewBag.Diem7to8 = count7to8;
            ViewBag.Diem8to10 = count8to10;

            // Lấy ra danh sách các kỳ kiểm tra để hiển thị lên dropdown
            var kyKiemTraList = _context.KyKiemTra.ToList();
            ViewBag.KyKiemTraList = kyKiemTraList;
          var tenkykiemtra = _context.KyKiemTra.FirstOrDefault(k => k.KyKiemTraId == kyKiemTraId)?.TenKyKiemTra;
            ViewBag.KyKiemTraName = tenkykiemtra;
            // Pass kyKiemTraId to the view
            ViewBag.KyKiemTraId = kyKiemTraId;

            return View(baiLamList);
        }

        private (double, double, double, double, int, int, int, int, int) CalculateStatistics(List<BaiLam> baiLamList)
        {
            // Đếm số lượng sinh viên tham gia thi từ bảng BaiLam
            var totalParticipants = baiLamList
                .Select(x => x.MSSV)
                .Distinct()
                .Count();

            // Count the number of students in each score range
            var count0to5 = baiLamList.Count(s => s.Diem >= 0 && s.Diem < 5);
            var count5to7 = baiLamList.Count(s => s.Diem >= 5 && s.Diem < 7);
            var count7to8 = baiLamList.Count(s => s.Diem >= 7 && s.Diem < 8);
            var count8to10 = baiLamList.Count(s => s.Diem >= 8 && s.Diem <= 10);

            // Calculate the total number of students
            var total = baiLamList.Count();

            // Calculate the percentage of students in each score range
            var percent0to5 = Math.Round((double)count0to5 / total * 100, 2);
            var percent5to7 = Math.Round((double)count5to7 / total * 100, 2);
            var percent7to8 = Math.Round((double)count7to8 / total * 100, 2);
            var percent8to10 = Math.Round((double)count8to10 / total * 100, 2);

            return (percent0to5, percent5to7, percent7to8, percent8to10, totalParticipants, count0to5, count5to7, count7to8, count8to10);
        }

    }
}
