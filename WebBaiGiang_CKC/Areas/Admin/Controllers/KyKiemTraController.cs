using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Reflection;
using WebBaiGiang_CKC.Data;
using WebBaiGiang_CKC.Models;

namespace WebBaiGiang_CKC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class KyKiemTraController : Controller
    {
        private readonly WebBaiGiangContext _context;
        public INotyfService _notyfService { get; }
        public KyKiemTraController(WebBaiGiangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/KyKiemTras
        public async Task<IActionResult> Index()
        {
            return View(await _context.KyKiemTra.ToListAsync());
        }


        // GET: Admin/KyKiemTras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.KyKiemTra == null)
            {
                return NotFound();
            }

            var kyKiemTra = await _context.KyKiemTra
                .FirstOrDefaultAsync(m => m.KyKiemTraId == id);
            if (kyKiemTra == null)
            {
                return NotFound();
            }

            return View(kyKiemTra);
        }

        // GET: Admin/KyKiemTras/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/KyKiemTras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KyKiemTraId,TenKyKiemTra,SoCauHoi,ThoiGianBatDau,ThoiGianKetThuc,ThoiGianLamBai")] KyKiemTra kyKiemTra)
        {


            if (ModelState.IsValid)
            {
                kyKiemTra.TenKyKiemTra = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(kyKiemTra.TenKyKiemTra);
                _context.Add(kyKiemTra);
                _notyfService.Success("Thêm thành công!");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(kyKiemTra);
        }

        // GET: Admin/KyKiemTras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.KyKiemTra == null)
            {
                return NotFound();
            }

            var kyKiemTra = await _context.KyKiemTra.FindAsync(id);
            if (kyKiemTra == null)
            {
                return NotFound();
            }


            return View(kyKiemTra);
        }

        // POST: Admin/KyKiemTras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KyKiemTraId,TenKyKiemTra,SoCauHoi,ThoiGianBatDau,ThoiGianKetThuc,ThoiGianLamBai")] KyKiemTra kyKiemTra)
        {
            if (id != kyKiemTra.KyKiemTraId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    kyKiemTra.TenKyKiemTra = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(kyKiemTra.TenKyKiemTra);
                    _context.Update(kyKiemTra);
                    _notyfService.Success("Cập nhật thành công!");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KyKiemTraExists(kyKiemTra.KyKiemTraId))
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


            return View(kyKiemTra);
        }

        // GET: Admin/KyKiemTras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.KyKiemTra == null)
            {
                return NotFound();
            }

            var kyKiemTra = await _context.KyKiemTra
                .FirstOrDefaultAsync(m => m.KyKiemTraId == id);
            if (kyKiemTra == null)
            {
                return NotFound();
            }

            return View(kyKiemTra);
        }

        // POST: Admin/KyKiemTras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.KyKiemTra == null)
            {
                return Problem("Entity set 'BaiGiangContext.KyKiemTras'  is null.");
            }
            var kyKiemTra = await _context.KyKiemTra.FindAsync(id);
            if (kyKiemTra != null)
            {
                _context.KyKiemTra.Remove(kyKiemTra);
            }
            _notyfService.Success("Xóa thành công!");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KyKiemTraExists(int id)
        {
            return _context.KyKiemTra.Any(e => e.KyKiemTraId == id);
        }
        public IActionResult ExportExcel()
        {
            try
            {
                // var data = _context.CauHoi_BaiLam.Include(x=>x.BaiLam).Include(x => x.CauHoi_De).ThenInclude(x => x.De).ThenInclude(x => x.KyKiemTra).Where(x => x.CauHoi_De.De.KyKiemTra.KyKiemTraId == kykiemtra);

                var data = _context.BaiLam.ToList();
                if (data != null && data.Count > 0)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(ToConvertDataTable(data.ToList()));

                        using (MemoryStream stream = new MemoryStream())
                        {

                            wb.SaveAs(stream);
                            string fileName = $"BaiThiSinhVien_{DateTime.Now.ToString("dd/MM/yyyy")}.xlsx";
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocuments.spreadsheetml.sheet", fileName);
                        }

                    }

                }
                _notyfService.Error("Không Có Dữ Liệu Trong Database");
            }
            catch (Exception)
            {
                _notyfService.Error("Xuất Excel Thất Bại!");
            }
            return RedirectToAction("Index");
        }
        public DataTable ToConvertDataTable<T>(List<T> items)
        {
            DataTable dt = new DataTable(typeof(T).Name);
            PropertyInfo[] propInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<string> columnsToExport = new List<string> { "HoTen", "MSSV", "SoCauDung", "Diem", "ThoiGianBatDau", "ThoiGianDenHan" };
            foreach (PropertyInfo prop in propInfo)
            {
                if (columnsToExport.Contains(prop.Name))
                {
                    dt.Columns.Add(prop.Name);
                }
            }
            foreach (T item in items)
            {
                var values = new object[columnsToExport.Count];
                int columnIndex = 0;
                for (int i = 0; i < propInfo.Length; i++)
                {
                    if (columnsToExport.Contains(propInfo[i].Name))
                    {
                        if (propInfo[i].Name == "DeId")
                        {
                            int deId = (int)propInfo[i].GetValue(item, null);
                            De de = _context.De.Include(d => d.KyKiemTra).FirstOrDefault(d => d.DeId == deId);
                            values[columnIndex] = de?.KyKiemTra.TenKyKiemTra;
                        }
                        else
                        {
                            values[columnIndex] = propInfo[i].GetValue(item, null);
                        }
                        columnIndex++;
                    }
                }
                dt.Rows.Add(values);
            }
            return dt;
        }

    }
}
