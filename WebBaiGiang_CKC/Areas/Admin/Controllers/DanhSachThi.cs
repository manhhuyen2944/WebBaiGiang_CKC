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
        public IActionResult CreateList()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateList(IFormFile formFile)
        {
            try
            {
                var mainPath = Path.Combine(Directory.GetCurrentDirectory(), "UpLoads", "Files");
                if (!Directory.Exists(mainPath))
                {
                    Directory.CreateDirectory(mainPath);
                }

                var filePath = Path.Combine(mainPath, formFile.FileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }
                var fileName = formFile.FileName;
                string extension = Path.GetExtension(fileName);
                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls":
                        conString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + filePath + ";Extended Properties='Excel 8.0; HDR=Yes'";
                        break;
                    case ".xlsx":
                        conString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + filePath + ";Extended Properties='Excel 8.0; HDR=Yes'";
                        break;
                }
                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);
#pragma warning disable CA1416 // Validate platform compatibility
                using (OleDbConnection conExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = conExcel;
                            conExcel.Open();
                            DataTable dtExcelSchema = conExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            cmdExcel.CommandText = "SELECT * FROM[" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            conExcel.Close();
                        }
                    }
                }
#pragma warning restore CA1416 // Validate platform compatibility
                conString = _configuration.GetConnectionString("WebBaiGiang");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    foreach (DataRow row in dt.Rows)
                    {
                        var taikhoanid = row["TaiKhoanId"].ToString();
                        var queryTaiKhoan = "SELECT MSSV FROM TaiKhoan WHERE TaiKhoanId = @TaiKhoanId";
                        using (SqlCommand cmdTaiKhoan = new SqlCommand(queryTaiKhoan, con))
                        {
                            cmdTaiKhoan.Parameters.AddWithValue("@TaiKhoanId", taikhoanid);
                            using (SqlDataReader readerTaiKhoan = cmdTaiKhoan.ExecuteReader())
                            {
                                if (!readerTaiKhoan.Read())
                                {
                                    _notyfService.Error($"Tài khoản {taikhoanid} chưa có tài khoản. Vui lòng tạo tài khoản trước khi import danh sách thi!");
                                    return RedirectToAction("Index");
                                }
                            }
                        }

                        var queryDanhSachThi = "SELECT DST.TaiKhoanId, TK.MSSV FROM DanhSachThi DST JOIN TaiKhoan TK ON DST.TaiKhoanId = TK.TaiKhoanId WHERE DST.TaiKhoanId = @TaiKhoanId";
                        using (SqlCommand cmdDanhSachThi = new SqlCommand(queryDanhSachThi, con))
                        {
                            cmdDanhSachThi.Parameters.AddWithValue("@TaiKhoanId", taikhoanid);
                            using (SqlDataReader readerDanhSachThi = cmdDanhSachThi.ExecuteReader())
                            {
                                if (readerDanhSachThi.Read())
                                {
                                    var mssv = readerDanhSachThi["MSSV"].ToString();
                                    _notyfService.Warning($"Tài khoản {taikhoanid} ({mssv}) đã tồn tại trong danh sách thi!");
                                    return RedirectToAction("Index");
                                }
                            }
                        }
                    }

                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        sqlBulkCopy.DestinationTableName = "DanhSachThi";
                        sqlBulkCopy.ColumnMappings.Add("TaiKhoanId", "TaiKhoanId");
                        sqlBulkCopy.ColumnMappings.Add("KyKiemTraId", "KyKiemTraId");
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }
                _notyfService.Success("Thêm Thành Công!");
                return RedirectToAction("Index");



            }
            catch (Exception)
            {
                _notyfService.Error("Thêm Thất Bại!");
            }


            return RedirectToAction("Index");
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
