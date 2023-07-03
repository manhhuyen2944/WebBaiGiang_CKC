using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.InkML;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
                                                     .ToListAsync();
                            var rng = new Random();
                            int n = cauHoiChuong.Count;
                            while (n > 1)
                            {
                                n--;
                                int k = rng.Next(n + 1);
                                var value = cauHoiChuong[k];
                                cauHoiChuong[k] = cauHoiChuong[n];
                                cauHoiChuong[n] = value;
                            }
                            var cauHoiChuongSelected = cauHoiChuong.Take(soCauHoiChuong).ToList();
                            tongSoCauHoi += cauHoiChuongSelected.Count;

                            foreach (var cauHoi in cauHoiChuongSelected)
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

            if (ModelState.IsValid)
            {
                try
                {
                    var existingDe = await _context.De.FirstOrDefaultAsync(d => d.KyKiemTraId == de.KyKiemTraId && d.DeId != de.DeId);
                    if (existingDe != null)
                    {
                        _notyfService.Error("Đã có đề thi cho kỳ kiểm tra này!");
                        ViewData["KyKiemTraId"] = new SelectList(_context.KyKiemTra, "KyKiemTraId", "TenKyKiemTra", de.KyKiemTraId);
                        return View(de);
                    }

                    var dbDe = await _context.De
                        .Include(d => d.CauHoi_DeThi)
                        .FirstOrDefaultAsync(d => d.DeId == de.DeId);

                    if (dbDe == null)
                    {
                        return NotFound();
                    }

                    var danhSachChuong = await _context.CauHoi
                        .Select(c => c.Chuong)
                        .Distinct()
                        .ToListAsync();

                    dbDe.SoCauHoi = de.SoCauHoi;
                    dbDe.DoKhoDe = de.DoKhoDe;

                    dbDe.CauHoi_DeThi.Clear();

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
                                                         .ToListAsync();
                                var rng = new Random();
                                int n = cauHoiChuong.Count;
                                while (n > 1)
                                {
                                    n--;
                                    int k = rng.Next(n + 1);
                                    var value = cauHoiChuong[k];
                                    cauHoiChuong[k] = cauHoiChuong[n];
                                    cauHoiChuong[n] = value;
                                }
                                var cauHoiChuongSelected = cauHoiChuong.Take(soCauHoiChuong).ToList();
                                tongSoCauHoi += cauHoiChuongSelected.Count;

                                foreach (var cauHoi in cauHoiChuongSelected)
                                {
                                    var cauHoi_De = new CauHoi_De
                                    {
                                        DeId = dbDe.DeId,
                                        CauHoiId = cauHoi.CauHoiId
                                    };
                                    dbDe.CauHoi_DeThi.Add(cauHoi_De);
                                }
                            }
                        }
                    }

                    // Kiểm tra số câu hỏi nhập vào có vượt quá số câu hỏi tối đa trong các chương hay không
                    var totalNewQuestions = danhSachChuong.Sum(chuong => int.TryParse(Request.Form["CauHoiChuong" + chuong.ChuongId], out var soCauHoiChuong) ? soCauHoiChuong : 0);
                    if (tongSoCauHoi != dbDe.SoCauHoi)
                    {
                        _notyfService.Error("Tổng số câu hỏi nhập vào của các chương phải bằng số câu hỏi của đề thi.");
                        ViewData["KyKiemTraId"] = new SelectList(_context.KyKiemTra, "KyKiemTraId", "TenKyKiemTra", de.KyKiemTraId);
                        return View(dbDe);
                    }
                    if (totalNewQuestions > dbDe.SoCauHoi)
                    {
                        _notyfService.Error("Số câu hỏi nhập bổ sung vượt quá số câu hỏi của đề thi.");
                        hasInvalidInput = true;
                    }
                    else if (totalNewQuestions < dbDe.SoCauHoi)
                    {
                        // Kiểm tra xem có đủ số câu hỏi trong các chương không
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
                            }
                        }
                        if (!hasInvalidInput)
                        {
                            var cauHoiKhongThuocChuong = await _context.CauHoi
                                                                  .Where(c => !danhSachChuong.Contains(c.Chuong))
                                                                  .ToListAsync();
                            var rng = new Random();
                            int n = cauHoiKhongThuocChuong.Count;
                            while (n > 1)
                            {
                                n--;
                                int k = rng.Next(n + 1);
                                var value = cauHoiKhongThuocChuong[k];
                                cauHoiKhongThuocChuong[k] = cauHoiKhongThuocChuong[n];
                                cauHoiKhongThuocChuong[n] = value;
                            }
                            var cauHoiKhongThuocChuongSelected = cauHoiKhongThuocChuong.Take(dbDe.SoCauHoi - tongSoCauHoi).ToList();

                            foreach (var cauHoi in cauHoiKhongThuocChuongSelected)
                            {
                                var cauHoi_De = new CauHoi_De
                                {
                                    DeId = dbDe.DeId,
                                    CauHoiId = cauHoi.CauHoiId
                                };
                                dbDe.CauHoi_DeThi.Add(cauHoi_De);
                            }
                        }
                    }

                    if (hasInvalidInput)
                    {
                        ViewData["KyKiemTraId"] = new SelectList(_context.KyKiemTra, "KyKiemTraId", "TenKyKiemTra", de.KyKiemTraId);
                        return View(dbDe);
                    }
                    _notyfService.Success("Cập nhật thành công");
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> PdfDeViewer(int deId)
        {
            var htmlcontent = await System.IO.File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PdfDeViewer.html"));
            var cauhoiList = await _context.CauHoi_De.Include(c => c.CauHoi).Where(c => c.DeId == deId).ToListAsync();

            if (cauhoiList == null || cauhoiList.Count == 0)
            {
                return BadRequest("Không tìm thấy câu hỏi nào");
            }

            // Tạo một đối tượng Document
            Document document = new Document();

            // Tạo một MemoryStream để lưu tệp PDF
            MemoryStream memoryStream = new MemoryStream();

            // Tạo một đối tượng PdfWriter để viết dữ liệu vào tệp PDF
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            // Mở Document
            document.Open();

            int i = 1;
            // Thêm phần tử HTML vào Document bằng iTextSharp
            foreach (var cauhoi in cauhoiList)
            {
                document.Add(new Paragraph($"Câu {i}. {cauhoi.CauHoi.NoiDung}"));
                document.Add(new Paragraph($"A. {cauhoi.CauHoi.DapAnA}"));
                document.Add(new Paragraph($"B. {cauhoi.CauHoi.DapAnB}"));
                document.Add(new Paragraph($"C. {cauhoi.CauHoi.DapAnC}"));
                document.Add(new Paragraph($"D. {cauhoi.CauHoi.DapAnD}"));

                i++;
            }

            // Đóng Document
            document.Close();
            // Thiết lập kích thước giấy và viền giấy
            document.SetPageSize(new Rectangle(PageSize.A4));
            document.SetMargins(20f, 20f, 20f, 20f);
            // Lấy dữ liệu tệp PDF từ MemoryStream và trả về nó như là một FileContentResult
            byte[] pdfBytes = memoryStream.ToArray();
            return new FileContentResult(pdfBytes, "application/pdf");
        }
        private bool DeExists(int id)
        {
            return _context.De.Any(e => e.DeId == id);
        }
    }
}
