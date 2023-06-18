using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBaiGiang_CKC.Data;
using WebBaiGiang_CKC.Models;

namespace WebBaiGiang_CKC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ChuongController : Controller
    {
        private readonly WebBaiGiangContext _context;
        public INotyfService _notyfService { get; }
        public ChuongController(WebBaiGiangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/Chuong
        public async Task<IActionResult> Index()
        {
            var webBaiGiangContext = _context.Chuong.Include(c => c.MonHoc);
            return View(await webBaiGiangContext.ToListAsync());
        }

        // GET: Admin/Chuong/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Chuong == null)
            {
                return NotFound();
            }

            var chuong = await _context.Chuong
                .Include(c => c.MonHoc)
                .FirstOrDefaultAsync(m => m.ChuongId == id);
            if (chuong == null)
            {
                return NotFound();
            }

            return View(chuong);
        }

        // GET: Admin/Chuong/Create
        public IActionResult Create()
        {
            ViewData["MonHocId"] = new SelectList(_context.MonHoc, "MonHocId", "TenMonHoc");
            return View();
        }

        // POST: Admin/Chuong/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChuongId,TenChuong,MonHocId")] Chuong chuong)
        {
            if (ModelState.IsValid)
            {
                var existingChuong = await _context.Chuong.FirstOrDefaultAsync(c => c.ChuongId == chuong.ChuongId && c.MonHocId == chuong.MonHocId);
                if (existingChuong != null)
                {
                    ModelState.AddModelError("ChuongId", "Số chương đã tồn tại");
                    ViewData["MonHocId"] = new SelectList(_context.MonHoc, "MonHocId", "TenMonHoc", chuong.MonHocId);
                    return View(chuong);
                }
                _context.Add(chuong);
                _notyfService.Success("Thêm Thành Công");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MonHocId"] = new SelectList(_context.MonHoc, "MonHocId", "TenMonHoc", chuong.MonHocId);
            return View(chuong);
        }

        // GET: Admin/Chuong/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Chuong == null)
            {
                return NotFound();
            }

            var chuong = await _context.Chuong.FindAsync(id);
            if (chuong == null)
            {
                return NotFound();
            }
            ViewData["MonHocId"] = new SelectList(_context.MonHoc, "MonHocId", "TenMonHoc", chuong.MonHocId);
            return View(chuong);
        }

        // POST: Admin/Chuong/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ChuongId,TenChuong,MonHocId")] Chuong chuong)
        {
            if (id != chuong.ChuongId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chuong);
                    _notyfService.Success("Cập Nhật Thành Công");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChuongExists(chuong.ChuongId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MonHocId"] = new SelectList(_context.MonHoc, "MonHocId", "TenMonHoc", chuong.MonHocId);
            return View(chuong);
        }

        // GET: Admin/Chuong/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Chuong == null)
            {
                return NotFound();
            }

            var chuong = await _context.Chuong
                .Include(c => c.MonHoc)
                .FirstOrDefaultAsync(m => m.ChuongId == id);
            if (chuong == null)
            {
                return NotFound();
            }

            return View(chuong);
        }

        // POST: Admin/Chuong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Chuong == null)
            {
                return Problem("Entity set 'WebBaiGiangContext.Chuong'  is null.");
            }
            var chuong = await _context.Chuong.FindAsync(id);
            if (chuong != null)
            {
                _context.Chuong.Remove(chuong);
            }
            _notyfService.Success("Xóa Thành Công");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChuongExists(int id)
        {
          return _context.Chuong.Any(e => e.ChuongId == id);
        }
    }
}
