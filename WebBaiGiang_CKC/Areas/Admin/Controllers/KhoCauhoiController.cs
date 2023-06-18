﻿using AspNetCoreHero.ToastNotification.Abstractions;
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
using DocumentFormat.OpenXml.Math;

namespace WebBaiGiang_CKC.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class KhoCauHoiController : Controller
    {
        private readonly WebBaiGiangContext _context;
        private readonly IConfiguration _configuration;
        public INotyfService _notyfService { get; }
        public KhoCauHoiController(WebBaiGiangContext context, IConfiguration configuration, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
            _configuration = configuration;
        }

        // GET: Admin/KhoCauHoi
        public IActionResult Index(int? page)
        {
            var baiGiangContext = _context.CauHoi.Include(k => k.Chuong);
            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 12;
            PagedList<CauHoi> models = new PagedList<CauHoi>(baiGiangContext, pageNo, pageSize);

            return View(models);
        }

        // GET: Admin/KhoCauHoi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CauHoi == null)
            {
                return NotFound();
            }

            var khoCauHoi = await _context.CauHoi
                .Include(k => k.Chuong)
                .FirstOrDefaultAsync(m => m.CauHoiId == id);
            if (khoCauHoi == null)
            {
                return NotFound();
            }

            return View(khoCauHoi);
        }

        // GET: Admin/KhoCauHoi/Create
        public IActionResult Create()
        {
            ViewData["ChuongId"] = new SelectList(_context.Chuong, "ChuongId", "TenChuong");
            return View();
        }

        // POST: Admin/KhoCauHoi/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CauHoiId,ChuongId,NoiDung,DapAnA,DapAnB,DapAnC,DapAnD,DapAnDung,DoKho,SoLanLay,SoLanTraLoiDung")] CauHoi khoCauHoi)
        {
            if (khoCauHoi.ChuongId == 0)
            {
                _notyfService.Error("Vui lòng chọn chương học!");
            }
            else if (ModelState.IsValid)
            {
                _context.Add(khoCauHoi);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm Thành Công");
                return RedirectToAction(nameof(Index));
            }

            ViewData["ChuongId"] = new SelectList(_context.Chuong, "ChuongId", "TenChuong", khoCauHoi.ChuongId);
            return View(khoCauHoi);
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
                // Lấy danh sách các giá trị khóa ngoại từ bảng KyKiemTra
                List<int> ChuongIds = new List<int>();
                string ConString = _configuration.GetConnectionString("WebBaiGiang");
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string query = "SELECT ChuongId FROM CHUONG";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ChuongIds.Add(reader.GetInt32(reader.GetOrdinal("ChuongId")));
                            }
                        }
                        con.Close();
                    }
                }
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
                // Kiểm tra giá trị khóa ngoại trước khi thêm bản ghi vào cơ sở dữ liệu
                bool hasInvalidChuongHoc = false;
                foreach (DataRow row in dt.Rows)
                {
                    string dapAnDung = row["DapAnDung"].ToString().ToUpper();
                    row["DapAnDung"] = dapAnDung;
                    if (!ChuongIds.Contains(Convert.ToInt32(row["ChuongId"])))
                    {
                        _notyfService.Warning("Chưa tạo chương học hoặc sai chương học !");
                        hasInvalidChuongHoc = true;
                        break;
                    }
                }

                if (hasInvalidChuongHoc)
                {
                    return RedirectToAction("Index");
                }
                conString = _configuration.GetConnectionString("WebBaiGiang");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        sqlBulkCopy.DestinationTableName = "CauHoi";
                        sqlBulkCopy.ColumnMappings.Add("ChuongId", "ChuongId");
                        sqlBulkCopy.ColumnMappings.Add("NoiDung", "NoiDung");
                        sqlBulkCopy.ColumnMappings.Add("DapAnA", "DapAnA");
                        sqlBulkCopy.ColumnMappings.Add("DapAnB", "DapAnB");
                        sqlBulkCopy.ColumnMappings.Add("DapAnC", "DapAnC");
                        sqlBulkCopy.ColumnMappings.Add("DapAnD", "DapAnD");
                        sqlBulkCopy.ColumnMappings.Add("DapAnDung", "DapAnDung");
                        sqlBulkCopy.ColumnMappings.Add("DoKho", "DoKho");
                        sqlBulkCopy.ColumnMappings.Add("SoLanLay", "SoLanLay");
                        sqlBulkCopy.ColumnMappings.Add("SoLanTraLoiDung", "SoLanTraLoiDung");
                        con.Open();
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
        // GET: Admin/KhoCauHoi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CauHoi == null)
            {
                return NotFound();
            }

            var khoCauHoi = await _context.CauHoi.FindAsync(id);
            if (khoCauHoi == null)
            {
                return NotFound();
            }
            ViewData["ChuongId"] = new SelectList(_context.Chuong, "ChuongId", "TenChuong", khoCauHoi.ChuongId);
            return View(khoCauHoi);
        }

        // POST: Admin/KhoCauHoi/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CauHoiId,ChuongId,NoiDung,DapAnA,DapAnB,DapAnC,DapAnD,DapAnDung,DoKho,SoLanLay,SoLanTraLoiDung")] CauHoi khoCauHoi)
        {
            if (id != khoCauHoi.CauHoiId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (khoCauHoi.ChuongId == 0)
                    {
                        _notyfService.Error("Vui lòng chọn chương học!");
                    }
                    else
                    {
                        _context.Update(khoCauHoi);
                        _notyfService.Success("Cập Nhật Thành Công");
                        await _context.SaveChangesAsync();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhoCauHoiExists(khoCauHoi.CauHoiId))
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
            ViewData["ChuongId"] = new SelectList(_context.Chuong, "ChuongId", "TenChuong", khoCauHoi.ChuongId);
            return View(khoCauHoi);
        }

        // GET: Admin/KhoCauHoi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CauHoi == null)
            {
                return NotFound();
            }

            var khoCauHoi = await _context.CauHoi
                .Include(k => k.Chuong)
                .FirstOrDefaultAsync(m => m.CauHoiId == id);
            if (khoCauHoi == null)
            {
                return NotFound();
            }

            return View(khoCauHoi);
        }

        // POST: Admin/KhoCauHoi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CauHoi == null)
            {
                return Problem("Entity set 'BaiGiangContext.KhoCauHoi'  is null.");
            }
            var khoCauHoi = await _context.CauHoi.FindAsync(id);
            if (khoCauHoi != null)
            {
                _context.CauHoi.Remove(khoCauHoi);
            }

            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa Thành Công");
            return RedirectToAction(nameof(Index));
        }

        private bool DanhSachThiExists(int id)
        {
            return _context.DanhSachThi.Any(e => e.DanhSachThiId == id);
        }
        private bool KhoCauHoiExists(int id)
        {
            return _context.CauHoi.Any(e => e.CauHoiId == id);
        }
    }
}

