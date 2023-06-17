using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBaiGiang_CKC.Data;
using WebBaiGiang_CKC.Models;

namespace WebBaiGiang_CKC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class MonHocController : Controller
    {
        private readonly WebBaiGiangContext _context;
        public INotyfService _notyfService { get; }
        public MonHocController(WebBaiGiangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/MonHoc
        public async Task<IActionResult> Index()
        {
            return View(await _context.MonHoc.ToListAsync());
        }

        // GET: Admin/MonHoc/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MonHoc == null)
            {
                return NotFound();
            }

            var monHoc = await _context.MonHoc
                .FirstOrDefaultAsync(m => m.MonHocId == id);
            if (monHoc == null)
            {
                return NotFound();
            }

            return View(monHoc);
        }

        // GET: Admin/MonHoc/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/MonHoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MonHocId,TenMonHoc,MaMonHoc,MoTa")] MonHoc monHoc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(monHoc);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm Thành Công");
                return RedirectToAction(nameof(Index));
            }
            return View(monHoc);
        }

        // GET: Admin/MonHoc/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MonHoc == null)
            {
                return NotFound();
            }

            var monHoc = await _context.MonHoc.FindAsync(id);
            if (monHoc == null)
            {
                return NotFound();
            }
            return View(monHoc);
        }

        // POST: Admin/MonHoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MonHocId,TenMonHoc,MaMonHoc,MoTa")] MonHoc monHoc)
        {
            if (id != monHoc.MonHocId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(monHoc);
                    _notyfService.Success("Sửa Thành Công");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonHocExists(monHoc.MonHocId))
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
            return View(monHoc);
        }

        // GET: Admin/MonHoc/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MonHoc == null)
            {
                return NotFound();
            }

            var monHoc = await _context.MonHoc
                .FirstOrDefaultAsync(m => m.MonHocId == id);
            if (monHoc == null)
            {
                return NotFound();
            }

            return View(monHoc);
        }

        // POST: Admin/MonHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MonHoc == null)
            {
                return Problem("Entity set 'BaiGiangContext.MonHoc'  is null.");
            }
            var monHoc = await _context.MonHoc.FindAsync(id);
            if (monHoc != null)
            {
                _context.MonHoc.Remove(monHoc);
            }
            _notyfService.Success("Xóa Thành Công");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MonHocExists(int id)
        {
            return _context.MonHoc.Any(e => e.MonHocId == id);
        }
    }
}
