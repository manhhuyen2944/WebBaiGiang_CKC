using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebBaiGiang_CKC.Extension;
using WebBaiGiang_CKC.Data;
using Microsoft.EntityFrameworkCore;
using BaiGiang.Models;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;

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
