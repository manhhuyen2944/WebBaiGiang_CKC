using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebBaiGiang_CKC.Data;
using WebBaiGiang_CKC.Models;

namespace WebBaiGiang_CKC.Controllers
{
    public class HomeController : Controller
    {


        protected WebBaiGiangContext _context;

        protected readonly IWebHostEnvironment _environment;

        public INotyfService _notyfService { get; }
        public HomeController(WebBaiGiangContext context, IWebHostEnvironment environment, INotyfService notyfService)
        {
            _context = context;
            _environment = environment;
            _notyfService = notyfService;
        }


        public IActionResult Index()
        {
            var mssvclaim = User.Claims.FirstOrDefault(c => c.Type == "MSSV");
            var mssv_ = "";
            if (mssvclaim != null)
            {
                mssv_ = mssvclaim.Value;
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var lstMon = _context.MonHoc.Include(a => a.Chuongs).ThenInclude(x => x.Bais).AsNoTracking().ToList();
            ViewData["lstSubject"] = lstMon;
            var mssvclaim = User.Claims.FirstOrDefault(c => c.Type == "MSSV");
            var mssv_ = "";
            if (mssvclaim != null)
            {
                mssv_ = mssvclaim.Value;
            }

            var kikiemtra = _context.DanhSachThi.Include(t => t.KyKiemTra).Include(x => x.TaiKhoan).ToList();
            var kiemtra_ = 0;
            CauHoi_BaiLam diemsv = null;
            kiemtra_ = _context.KyKiemTra.Include(x => x.DanhSachThi).Count(x => x.DanhSachThi.First().TaiKhoan.MSSV == mssv_ && x.DanhSachThi.First().TrangThai == false  && DateTime.UtcNow.AddHours(7) >= x.ThoiGianBatDau && DateTime.UtcNow.AddHours(7) < x.ThoiGianKetThuc);
            foreach (var kiemtra in kikiemtra)
            {
                diemsv = _context.CauHoi_BaiLam.Include(x => x.BaiLam).Include(x => x.CauHoi_De).ThenInclude(x => x.De).FirstOrDefault(x=>x.CauHoi_De.De.KyKiemTraId == kiemtra.KyKiemTraId && x.BaiLam.MSSV == mssv_);
            }
            var giaovien = _context.GiaoVien.ToList();
            ViewBag.Giaovien = giaovien;
            ViewBag.diemsv = diemsv;
            ViewBag.kikiemtra = kiemtra_;
            base.OnActionExecuting(filterContext);
        }

    }
}