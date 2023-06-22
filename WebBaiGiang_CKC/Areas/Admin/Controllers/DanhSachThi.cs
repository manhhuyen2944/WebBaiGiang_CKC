using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data.OleDb;
using System.Data;
using WebBaiGiang_CKC.Models;
using X.PagedList;
using WebBaiGiang_CKC.Data;

namespace WebBaiGiang_CKC.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DanhSachThiController : Controller
    {
        private readonly WebBaiGiangContext _context;
        private readonly IConfiguration _configuration;
        public INotyfService _notyfService { get; }
        public DanhSachThiController(WebBaiGiangContext context, INotyfService notyfService, IConfiguration configuration)
        {
            _context = context;
            _notyfService = notyfService;
            _configuration = configuration;
        }

        // GET: Admin/DanhSachThi

        public IActionResult Index(int? page)
        {
            var baiGiangContext = _context.DanhSachThi.Include(d => d.KyKiemTra).Include(d => d.TaiKhoan);

            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 12;
            PagedList<DanhSachThi> models = new PagedList<DanhSachThi>(baiGiangContext, pageNo, pageSize);
            return View(models);
        }
       
        // GET: Admin/DanhSachThi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DanhSachThi == null)
            {
                return NotFound();
            }

            var danhSachThi = await _context.DanhSachThi
                .Include(d => d.KyKiemTra)
                .Include(d => d.TaiKhoan)
                .FirstOrDefaultAsync(m => m.DanhSachThiId == id);
            if (danhSachThi == null)
            {
                return NotFound();
            }

            return View(danhSachThi);
        }

        // GET: Admin/DanhSachThi/Create
        public IActionResult Create()
        {
            ViewData["KyKiemTraId"] = new SelectList(_context.KyKiemTra, "KyKiemTraId", "TenKyKiemTra");
            ViewData["TaiKhoanId"] = new SelectList(_context.TaiKhoan, "TaiKhoanId", "HoTen");
            return View();
        }

        // POST: Admin/DanhSachThi/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DanhSachThiId,TaiKhoanId,KyKiemTraId,TrangThai")] DanhSachThi danhSachThi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(danhSachThi);
                _notyfService.Success("Thêm Thành Công");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KyKiemTraId"] = new SelectList(_context.KyKiemTra, "KyKiemTraId", "TenKyKiemTra", danhSachThi.KyKiemTraId);
            ViewData["TaiKhoanId"] = new SelectList(_context.TaiKhoan, "TaiKhoanId", "HoTen", danhSachThi.TaiKhoanId);
            return View(danhSachThi);
        }

        // GET: Admin/DanhSachThi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DanhSachThi == null)
            {
                return NotFound();
            }

            var danhSachThi = await _context.DanhSachThi.FindAsync(id);
            if (danhSachThi == null)
            {
                return NotFound();
            }
            ViewData["KyKiemTraId"] = new SelectList(_context.KyKiemTra, "KyKiemTraId", "TenKyKiemTra", danhSachThi.KyKiemTraId);
            ViewData["TaiKhoanId"] = new SelectList(_context.TaiKhoan, "TaiKhoanId", "HoTen", danhSachThi.TaiKhoanId);
            return View(danhSachThi);
        }

        // POST: Admin/DanhSachThi/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DanhSachThiId,TaiKhoanId,KyKiemTraId,TrangThai")] DanhSachThi danhSachThi)
        {
            if (id != danhSachThi.DanhSachThiId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(danhSachThi);
                    _notyfService.Success("Cập Nhật Thành Công");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhSachThiExists(danhSachThi.DanhSachThiId))
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
            ViewData["KyKiemTraId"] = new SelectList(_context.KyKiemTra, "KyKiemTraId", "TenKyKiemTra", danhSachThi.KyKiemTraId);
            ViewData["TaiKhoanId"] = new SelectList(_context.TaiKhoan, "TaiKhoanId", "HoTen", danhSachThi.TaiKhoanId);
            return View(danhSachThi);
        }

        // GET: Admin/DanhSachThi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DanhSachThi == null)
            {
                return NotFound();
            }

            var danhSachThi = await _context.DanhSachThi
                .Include(d => d.KyKiemTra)
                .Include(d => d.TaiKhoan)
                .FirstOrDefaultAsync(m => m.DanhSachThiId == id);
            if (danhSachThi == null)
            {
                return NotFound();
            }

            return View(danhSachThi);
        }

        // POST: Admin/DanhSachThi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DanhSachThi == null)
            {
                return Problem("Entity set 'BaiGiangContext.DanhSachThi'  is null.");
            }
            var danhSachThi = await _context.DanhSachThi.FindAsync(id);
            if (danhSachThi != null)
            {
                _context.DanhSachThi.Remove(danhSachThi);
            }
            _notyfService.Success("Xóa Thành Công");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DanhSachThiExists(int id)
        {
            return _context.DanhSachThi.Any(e => e.DanhSachThiId == id);
        }
    }
}
