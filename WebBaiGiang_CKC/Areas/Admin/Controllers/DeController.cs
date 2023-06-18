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
    public class DeController : Controller
    {
        private readonly WebBaiGiangContext _context;
        public INotyfService _notyfService { get; }
        public DeController(WebBaiGiangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/De
        public async Task<IActionResult> Index()
        {
            var baiGiangContext = _context.De.Include(d => d.KyKiemTra);
            return View(await baiGiangContext.ToListAsync());
        }

        // GET: Admin/De/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.De == null)
            {
                return NotFound();
            }

            var de = await _context.De
                .Include(d => d.KyKiemTra)
                .FirstOrDefaultAsync(m => m.DeId == id);
            if (de == null)
            {
                return NotFound();
            }

            return View(de);
        }
        [HttpGet]
        public async Task<IActionResult> LayDanhSachChuong()
        {
            var danhSachChuong = await _context.CauHoi
                .Include(c => c.Chuong)
                .Select(c => new { c.ChuongId, tenChuong = c.Chuong.TenChuong })
                .Distinct()
                .ToListAsync();
            return Json(danhSachChuong);
        }
        // GET: Admin/De/Create
        public IActionResult Create()
        {
            ViewData["KyKiemTraId"] = new SelectList(_context.KyKiemTra, "KyKiemTraId", "TenKyKiemTra");
            return View();
        }

        // POST: Admin/De/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DeId,KyKiemTraId,SoCauHoi,DoKhoDe")] De de)
        {
            if (ModelState.IsValid)
            {
                var existingDe = await _context.De.FirstOrDefaultAsync(d => d.KyKiemTraId == de.KyKiemTraId);
                if (existingDe != null)
                {
                    _notyfService.Error("Đã có đề thi cho kỳ kiểm tra này!");
                    ViewData["KyKiemTraId"] = new SelectList(_context.KyKiemTra, "KyKiemTraId", "TenKyKiemTra", de.KyKiemTraId);
                    return View(de);
                }
                _context.Add(de);
                var danhSachChuong = await _context.CauHoi
                    .Select(c => c.Chuong)
                    .Distinct()
                    .ToListAsync();

                de.CauHoi_DeThi = new List<CauHoi_De>();
                int tongSoCauHoi = 0;
                bool hasInvalidInput = false;

                foreach (var chuong in danhSachChuong)
                {
                    if (int.TryParse(Request.Form["CauHoiChuong" + chuong.ChuongId], out var soCauHoiChuong))
                    {
                        var cauHoiChuongCount = await _context.CauHoi
                                      .CountAsync(c => c.ChuongId == chuong.ChuongId);

                        if (soCauHoiChuong > cauHoiChuongCount)
                        {
                            _notyfService.Error("Số câu hỏi nhập bổ sung cho chương " + chuong.TenChuong + " không được lớn hơn số câu hỏi có trong chương.");
                            hasInvalidInput = true;
                            break;
                        }
                        else
                        {
                            var cauHoiChuong = await _context.CauHoi
                                .Where(c => c.ChuongId == chuong.ChuongId)
                                .OrderBy(c => Guid.NewGuid())
                                .Take(soCauHoiChuong)
                                .ToListAsync();
                            var tongSoCauHoiChuong = cauHoiChuong.Count;
                            tongSoCauHoi += tongSoCauHoiChuong;

                            foreach (var cauHoi in cauHoiChuong)
                            {
                                var cauHoi_De = new CauHoi_De
                                {
                                    DeId = de.DeId,
                                    CauHoiId = cauHoi.CauHoiId
                                };
                                de.CauHoi_DeThi.Add(cauHoi_De);
                            }
                        }
                    }
                }

                if (!hasInvalidInput && tongSoCauHoi == de.SoCauHoi)
                {
                    _context.Add(de);
                    _notyfService.Success("Thêm Thành Công");
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else if (hasInvalidInput)
                {

                }
                else
                {
                    _notyfService.Error("Vui lòng nhập tổng số câu hỏi nhập từ form  bằng với số câu hỏi của đề thi.");
                }
            }

            ViewData["KyKiemTraId"] = new SelectList(_context.KyKiemTra, "KyKiemTraId", "TenKyKiemTra", de.KyKiemTraId);
            return View(de);
        }

        // GET: Admin/De/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.De == null)
            {
                return NotFound();
            }

            var de = await _context.De.FindAsync(id);
            if (de == null)
            {
                return NotFound();
            }
            ViewData["KyKiemTraId"] = new SelectList(_context.KyKiemTra, "KyKiemTraId", "TenKyKiemTra", de.KyKiemTraId);
            return View(de);
        }

        // POST: Admin/De/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DeId,KyKiemTraId,SoCauHoi,DoKhoDe")] De de)
        {
            if (id != de.DeId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["KyKiemTraId"] = new SelectList(_context.KyKiemTra, "KyKiemTraId", "TenKyKiemTra", de.KyKiemTraId);
                return View(de);
            }

            try
            {
                var cauHoiDe = _context.CauHoi_De.Where(ch => ch.DeId == id);
                int soCauHoiCu = cauHoiDe.Count();
                int soCauHoiMoi = de.SoCauHoi;
                int soCauHoiCanThem = Math.Max(soCauHoiMoi - soCauHoiCu, 0);
                int soCauHoiCanXoa = Math.Max(soCauHoiCu - soCauHoiMoi, 0);
                bool hasInvalidInput = false;
                if (soCauHoiCanThem > 0 || soCauHoiCanXoa > 0)
                {
                    _context.RemoveRange(cauHoiDe);

                    var danhSachChuong = await _context.CauHoi
                        .Select(c => c.Chuong)
                        .Distinct()
                        .ToListAsync();

                    de.CauHoi_DeThi = new List<CauHoi_De>();
                    int tongSoCauHoi = 0;

                    foreach (var chuong in danhSachChuong)
                    {
                        if (int.TryParse(Request.Form["CauHoiChuong" + chuong.ChuongId], out var soCauHoiChuong))
                        {
                            try
                            {
                                var cauHoiChuongCount = await _context.CauHoi
                                     .CountAsync(c => c.ChuongId == chuong.ChuongId);

                                if (soCauHoiChuong > cauHoiChuongCount)
                                {
                                    _notyfService.Error("Số câu hỏi nhập bổ sung cho chương " + chuong.TenChuong + " không được lớn hơn số câu hỏi có trong chương.");
                                    hasInvalidInput = true;
                                    break;
                                }
                                else
                                {
                                    var cauHoiChuong = await _context.CauHoi
                                        .Where(c => c.ChuongId == chuong.ChuongId)
                                        .OrderBy(c => Guid.NewGuid())
                                        .Take(soCauHoiChuong)
                                        .ToListAsync();
                                    var tongSoCauHoiChuong = cauHoiChuong.Count;
                                    tongSoCauHoi += tongSoCauHoiChuong;

                                    foreach (var cauHoi in cauHoiChuong)
                                    {
                                        var cauHoi_De = new CauHoi_De
                                        {
                                            DeId = de.DeId,
                                            CauHoiId = cauHoi.CauHoiId
                                        };
                                        de.CauHoi_DeThi.Add(cauHoi_De);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _notyfService.Error("Số câu hỏi nhập bổ sung cho chương " + chuong.TenChuong + " không được lớn hơn số câu hỏi có trong chương." + ex.Message);
                            }
                        }
                    }

                    if (!hasInvalidInput && tongSoCauHoi == de.SoCauHoi)
                    {
                        _context.Update(de);
                        _notyfService.Success("Sửa Thành Công");
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else if (hasInvalidInput)
                    {

                    }
                    else
                    {
                        _notyfService.Error("Vui lòng nhập tổng số câu hỏi nhập từ form bằng với số câu hỏi của đề thi.");
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeExists(de.DeId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            ViewData["KyKiemTraId"] = new SelectList(_context.KyKiemTra, "KyKiemTraId", "TenKyKiemTra", de.KyKiemTraId);
            return View(de);
        }
        // GET: Admin/De/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.De == null)
            {
                return NotFound();
            }

            var de = await _context.De
                .Include(d => d.KyKiemTra)
                .FirstOrDefaultAsync(m => m.DeId == id);
            if (de == null)
            {
                return NotFound();
            }

            return View(de);
        }

        // POST: Admin/De/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.De == null)
            {
                return Problem("Entity set 'BaiGiangContext.De'  is null.");
            }
            var de = await _context.De.FindAsync(id);
            if (de != null)
            {
                _context.De.Remove(de);
            }
            _notyfService.Success("Xóa Thành Công");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeExists(int id)
        {
            return _context.De.Any(e => e.DeId == id);
        }
    }
}
