using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebBaiGiang_CKC.Extension;
using WebBaiGiang_CKC.Data;
using Microsoft.EntityFrameworkCore;

namespace WebBaiGiang_CKC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DangNhapController : Controller
    {
        private readonly WebBaiGiangContext _context;
        public INotyfService _notyfService { get; } 
        public static string image;
        public DangNhapController(WebBaiGiangContext context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Đăng nhập
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnURL = returnUrl;
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Login(string tendangnhap, string pass)
        {
            if (ModelState.IsValid)
            {
                var password = pass.ToMD5();
                //var anh = _context.GiaoVien.ToList();
                // Kiểm tra tên đăng nhập và mật khẩu
                var user = await _context.GiaoVien.FirstOrDefaultAsync(u => u.TenDangNhap == tendangnhap && u.MatKhau == password);
                if (user == null)
                {
                    // Tên đăng nhập hoặc mật khẩu không đúng
                    _notyfService.Error("Thông tin đăng nhập không chính xác");
                    return RedirectToAction("Index", "Home");
                }
                if (user.TenDangNhap != tendangnhap)
                {
                    // Tên đăng nhập hoặc mật khẩu không đúng
                    _notyfService.Error("Tài khoản không chính xác");
                    return RedirectToAction("Index", "Home");
                }
                if (user.MatKhau != password)
                {
                    // Tên đăng nhập hoặc mật khẩu không đúng
                    _notyfService.Error("Mật khẩu không chính xác");
                    return RedirectToAction("Index", "Home");
                }
                if (user.TrangThai == false)
                {
                    _notyfService.Error("Tài khoản đã bị khóa");
                    return RedirectToAction("Index", "Home");
                }
                if (user != null)
                {
                    // Lưu thông tin người dùng vào cookie xác thực
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, user.HoTen),
                        new Claim(ClaimTypes.Role, "Admin"),
                         new Claim("AnhDaiDien", "/contents/Images/GiaoVien/" + user.AnhDaiDien) // Thêm đường dẫn đến ảnh đại diện vào claims
                    };
                    //   Response.Cookies.Append("AnhDaiDien", "Images/GiaoVien/" + user.AnhDaiDien);
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    _notyfService.Success("Đăng nhập thành công");
                    // Chuyển hướng đến trang chủ
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _notyfService.Warning("Tên đăng nhập hoặc mật khẩu không đúng");
                }
            }

            // Nếu có lỗi xảy ra, hiển thị thông báo lỗi bằng NotyfService
            _notyfService.Warning("Tên đăng nhập hoặc mật khẩu không đúng");

            // Chuyển hướng đến trang Login
            return View("Login", "Home");
        }
        #endregion
        #region Đăng xuất

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _notyfService.Success("Đăng xuất thành công");
            return RedirectToAction("Login", "DangNhap");
        }
        #endregion



    }
}
