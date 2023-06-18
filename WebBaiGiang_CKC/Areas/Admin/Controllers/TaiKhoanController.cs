using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data.OleDb;
using System.Data;
using System.Globalization;
using WebBaiGiang_CKC.Models;
using X.PagedList;
using WebBaiGiang_CKC.Extension;
using WebBaiGiang_CKC.Data;

namespace WebBaiGiang_CKC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TaiKhoanController : Controller
    {
        private readonly WebBaiGiangContext _context;
        private readonly IConfiguration _configuration;

        public INotyfService _notyfService { get; }
        public TaiKhoanController(WebBaiGiangContext context, INotyfService notyfService, IConfiguration configuration)
        {
            _context = context;
            _notyfService = notyfService;
            _configuration = configuration;
        }


        // GET: Admin/TaiKhoan
        public IActionResult Index(int? page)
        {

            var _customer = from m in _context.TaiKhoan select m;
            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 12;
            PagedList<TaiKhoan> models = new PagedList<TaiKhoan>(_customer, pageNo, pageSize);
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

                var mainPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Files");
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
                        var mssv = row["MSSV"].ToString();
                        var email = row["Email"].ToString();
                        var hoTen = row["HoTen"].ToString();

                        // Viết hoa chữ cái đầu của tên
                        hoTen = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(hoTen.ToLower());

                        // Kiểm tra MSSV đã tồn tại trong cơ sở dữ liệu
                        var query = "SELECT COUNT(*) FROM TaiKhoan WHERE MSSV = @MSSV";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@MSSV", mssv);
                            int count = (int)cmd.ExecuteScalar();
                            if (count > 0)
                            {
                                _notyfService.Error($"MSSV {mssv} đã tồn tại trong cơ sở dữ liệu!");
                                return View();
                            }
                        }
                        // Kiểm tra Email đã tồn tại trong cơ sở dữ liệu
                        query = "SELECT COUNT(*) FROM TaiKhoan WHERE Email = @Email";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@Email", email);
                            int count = (int)cmd.ExecuteScalar();
                            if (count > 0)
                            {
                                _notyfService.Error($"Email {email} đã tồn tại trong cơ sở dữ liệu!");
                                return View();
                            }
                        }
                        row["HoTen"] = hoTen; // gán giá trị mới cho cột Họ tên
                    }
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        sqlBulkCopy.DestinationTableName = "TaiKhoan";
                        sqlBulkCopy.ColumnMappings.Add("MSSV", "MSSV");
                        sqlBulkCopy.ColumnMappings.Add("MatKhau", "MatKhau");
                        sqlBulkCopy.ColumnMappings.Add("Email", "Email");
                        sqlBulkCopy.ColumnMappings.Add("HoTen", "HoTen");
                        sqlBulkCopy.ColumnMappings.Add("TrangThai", "TrangThai");


                        foreach (DataRow row in dt.Rows)
                        {

                            var plainPassword = row["MatKhau"].ToString();
                            var md5 = plainPassword.ToMD5();
                            row["MatKhau"] = md5;

                        }
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
        // GET: Admin/TaiKhoan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TaiKhoan == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoan
                .FirstOrDefaultAsync(m => m.TaiKhoanId == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan);
        }

        // GET: Admin/TaiKhoan/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TaiKhoan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaiKhoanId,MSSV,MatKhau,Email,HoTen,TrangThai")] TaiKhoan taiKhoan)
        {
            if (ModelState.IsValid)
            {
                taiKhoan.HoTen = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(taiKhoan.HoTen);

                // Kiểm tra xem MSSV đã tồn tại trong cơ sở dữ liệu hay chưa
                if (_context.TaiKhoan.Any(a => a.MSSV == taiKhoan.MSSV))
                {
                    ModelState.AddModelError("MSSV", "MSSV đã tồn tại trong hệ thống.");
                    return View(taiKhoan);
                }

                // Kiểm tra xem Email đã tồn tại trong cơ sở dữ liệu hay chưa
                if (_context.TaiKhoan.Any(a => a.Email == taiKhoan.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại trong hệ thống.");
                    return View(taiKhoan);
                }
                taiKhoan.MatKhau = taiKhoan.MatKhau.ToMD5();
                _context.Add(taiKhoan);
                _notyfService.Success("Thêm thành công!");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taiKhoan);
        }

        // GET: Admin/TaiKhoan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TaiKhoan == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoan.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }
            return View(taiKhoan);
        }

        // POST: Admin/TaiKhoan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaiKhoanId,MSSV,Email,HoTen,TrangThai")] TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.TaiKhoanId)
            {
                return NotFound();
            }

            try
            {
                taiKhoan.HoTen = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(taiKhoan.HoTen);
                var existingAccount = await _context.TaiKhoan.FindAsync(id);
                if (existingAccount == null)
                {
                    return NotFound();
                }

                if (await _context.TaiKhoan.AnyAsync(x => x.MSSV == taiKhoan.MSSV && x.TaiKhoanId != taiKhoan.TaiKhoanId))
                {
                    ModelState.AddModelError("MSSV", "MSSV đã tồn tại trong hệ thống.");
                    return View(taiKhoan);
                }

                if (await _context.TaiKhoan.AnyAsync(x => x.Email == taiKhoan.Email && x.TaiKhoanId != taiKhoan.TaiKhoanId))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại trong hệ thống.");
                    return View(taiKhoan);
                }

                taiKhoan.MatKhau = existingAccount.MatKhau;
                _notyfService.Success("Sửa thành công!");
                _context.Entry(existingAccount).State = EntityState.Detached;
                _context.Update(taiKhoan);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaiKhoanExists(taiKhoan.TaiKhoanId))
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
        // GET: Admin/TaiKhoan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TaiKhoan == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoan
                .FirstOrDefaultAsync(m => m.TaiKhoanId == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan);
        }

        // POST: Admin/TaiKhoan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TaiKhoan == null)
            {
                return Problem("Entity set 'BaiGiangContext.TaiKhoan'  is null.");
            }
            var taiKhoan = await _context.TaiKhoan.FindAsync(id);
            if (taiKhoan != null)
            {
                _context.TaiKhoan.Remove(taiKhoan);
            }
            _notyfService.Success("Xóa thành công!");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaiKhoanExists(int id)
        {
            return _context.TaiKhoan.Any(e => e.TaiKhoanId == id);
        }
    }
}
