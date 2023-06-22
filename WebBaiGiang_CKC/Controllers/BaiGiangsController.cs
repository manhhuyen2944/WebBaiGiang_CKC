﻿using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBaiGiang_CKC.Data;

namespace WebBaiGiang_CKC.Controllers
{
    public class BaiGiangsController : HomeController
    {
        public BaiGiangsController(WebBaiGiangContext context, IWebHostEnvironment environment, INotyfService notyfService)
            : base(context, environment, notyfService)
        {

        }
        public IActionResult DeCuong()
        {
            return View();
        }
        public IActionResult NoiDungChinh()
        {
            return View();
        }
        public IActionResult GiaoVien()
        {
            var giaovien = _context.GiaoVien.AsNoTracking();
            return View(giaovien);
        }
        public IActionResult Bai(int id)
        {

            var lstBai = _context.Bai.Where(x => x.BaiId == id).Include(a => a.Mucs).AsNoTracking();
            return View(lstBai);
        }
        public IActionResult Lich()
        {

            return View();
        }
        public IActionResult BaiTap()
        {

            return View();
        }
        [Route("/BaiTap/thuc-hanh-tong-hop-1-xay-dung-ung-dung-quan-ly-sach")]
        public IActionResult BookShop()
        {

            return View();
        }
        [Route("/BaiTap/thuc-hanh-tong-hop-2-crud-trong-MVC")]
        public IActionResult Crue()
        {
            return View();
        }

    }
}
