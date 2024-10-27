using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using WApp.Datas;
using WApp.Helpers;
using WApp.Models;
using WApp.Utlis;
using WApp.ViewModels;
using WApp.ViewModels.Parameters;
using static WApp.Utlis.Enums;

namespace WApp.Controllers
{
    public class PegawaiController : BaseController
    {
        private readonly ILogger<PegawaiController> _logger;
        private readonly AppDataContext _context;
        private static readonly char[] separator = new[] { ',' };

        public PegawaiController(ILogger<PegawaiController> logger, AppDataContext context)
        {
            _logger = logger;
            _context = context;
        }

        private PegawaiViewModel InitModel(PegawaiViewModel model)
        {
            model.ListJenisKelamin =
            [
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = EnumHelper.GetDescription(Enums.JenisKelamin.L), Value = ((int)Enums.JenisKelamin.L).ToString() },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = EnumHelper.GetDescription(Enums.JenisKelamin.P), Value = ((int)Enums.JenisKelamin.P).ToString() }
            ];
            model.ListStatusPerkawinan =
            [
                .. _context.Statuses.Where(m => m.IsActive).Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id
                }),
            ];
            model.ListPendidikan =
            [
                .. _context.Pendidikans.Where(m => m.IsActive).Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id
                }),
            ];
            model.ListProdi =
            [
                .. _context.Prodis.Where(m => m.IsActive).Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id
                }),
            ];
            model.ListSatuanKerja =
            [
                .. _context.SatuanKerjas.Where(m => m.IsActive).Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id
                }),
            ];
            model.ListUnitKerja =
            [
                .. _context.Poldas.Where(m => m.IsActive).Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id
                }),
            ];

            return model;
        }

        private List<PegawaiViewModel> GetPegawaiModelFromExcel(byte[] excelDataStream, Enums.PegawaiType type)
        {
            Stream stream = new MemoryStream(excelDataStream);
            List<PegawaiViewModel> datas = [];

            using var package = new ExcelPackage(stream);
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            int rowCount = ExcelHelper.GetTotalRowCountByAnyNonNullData(worksheet);

            //check format
            if (type == Enums.PegawaiType.K2)
            {
                if ("NoRegistrasi*Name*JenisKelaminPendidikanProdiSatuanKerjaUnitKerjaAwalKerja(dd/mm/yyyy)AkhirKerja(dd/mm/yyyy)KeteranganStatus".ToLower().Trim() !=
                    $"{worksheet.Cells[1, 1].Value}{worksheet.Cells[1, 2].Value}{worksheet.Cells[1, 3].Value}{worksheet.Cells[1, 4].Value}{worksheet.Cells[1, 5].Value}{worksheet.Cells[1, 6].Value}{worksheet.Cells[1, 7].Value}{worksheet.Cells[1, 8].Value}{worksheet.Cells[1, 9].Value}{worksheet.Cells[1, 10].Value}{worksheet.Cells[1, 11].Value}".Replace(" ", "").ToLower().Trim())
                {
                    throw new Exception($"The format excel tidak sesuai, silahkan Download Template.");
                }
            }
            else
            {
                if ("NIK*Name*JenisKelaminPendidikanProdiSatuanKerjaUnitKerjaAwalKerja(dd/mm/yyyy)AkhirKerja(dd/mm/yyyy)KeteranganStatus".ToLower().Trim() !=
                    $"{worksheet.Cells[1, 1].Value}{worksheet.Cells[1, 2].Value}{worksheet.Cells[1, 3].Value}{worksheet.Cells[1, 4].Value}{worksheet.Cells[1, 5].Value}{worksheet.Cells[1, 6].Value}{worksheet.Cells[1, 7].Value}{worksheet.Cells[1, 8].Value}{worksheet.Cells[1, 9].Value}{worksheet.Cells[1, 10].Value}{worksheet.Cells[1, 11].Value}".Replace(" ", "").ToLower().Trim())
                {
                    throw new Exception($"The format excel tidak sesuai, silahkan Download Template.");
                }
            }
            for (int row = 2; row <= rowCount; row++)
            {
                var noRegistrasi = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(noRegistrasi))
                    throw new Exception($"Row [{row}]: {string.Format(Constans.Label.IsRequired, type == Enums.PegawaiType.K2 ? "No Registrasi" : "NIK")}");

                var nama = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(nama))
                    throw new Exception($"Row [{row}]: {string.Format(Constans.Label.IsRequired, "Nama")}");

                var jenisKelamin = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                var pendidikan = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                var prodi = worksheet.Cells[row, 5].Value?.ToString()?.Trim();
                var satuanKerja = worksheet.Cells[row, 6].Value?.ToString()?.Trim();
                var unitKerja = worksheet.Cells[row, 7].Value?.ToString()?.Trim();
                var awalKerja = worksheet.Cells[row, 8].Value?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(awalKerja))
                {
                    var awalKerjaDate = ConverterHelper.StringToDateTime(awalKerja);
                    if (!awalKerjaDate.HasValue)
                        throw new Exception($"Row [{row}]: The format Awal Kerja is not correct, its should be dd/MM/yyyy.");
                }

                var akhirKerja = worksheet.Cells[row, 9].Value?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(akhirKerja))
                {
                    var akhirKerjaDate = ConverterHelper.StringToDateTime(awalKerja);
                    if (!akhirKerjaDate.HasValue)
                    {
                        throw new Exception($"Row [{row}]: The format Akhir Kerja is not correct, its should be dd/MM/yyyy.");
                    }
                }
                var keterangan = worksheet.Cells[row, 10].Value?.ToString()?.Trim();
                var status = worksheet.Cells[row, 11].Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(status))
                    throw new Exception($"Row [{row}]: {string.Format(Constans.Label.IsRequired, "Status")}");

                var jenisKelaminEnum = EnumHelper.GetEnumValueFromDescription<Enums.JenisKelamin>(jenisKelamin);


                datas.Add(new PegawaiViewModel
                {
                    Name = nama,
                    Nomor = noRegistrasi,
                    JenisKelamin = string.IsNullOrEmpty(jenisKelamin) ? null : jenisKelaminEnum,
                    PendidikanText = pendidikan,
                    ProdiText = prodi,
                    SatuanKerjaText = satuanKerja,
                    UnitKerjaText = unitKerja,
                    TanggalAwal = awalKerja,
                    TanggalAkhir = akhirKerja,
                    Keterangan = keterangan,
                    IsActive = status == "Aktif",
                    Type = type
                });
            }

            return datas;
        }

        private void SaveDataFromExcel(List<PegawaiViewModel> datas)
        {
            foreach (var item in datas)
            {
                var newID = Guid.NewGuid().ToString();

                var pegawai = _context.Pegawais.FirstOrDefault(m => m.NIK.Trim() == item.Nomor.Trim() && m.Type == item.Type);
                pegawai ??= new PegawaiModel
                {
                    Id = newID,
                    CreatedBy = User?.Identity.Name,
                    CreatedDate = DateTime.Now,
                    UpdatedBy = User.Identity.Name,
                    UpdatedDate = DateTime.Now,
                    Type = item.Type,
                };

                pegawai.Name = item.Name;
                pegawai.NIK = item.Nomor;
                pegawai.JenisKelamin = item.JenisKelamin;

                if (!string.IsNullOrEmpty(item.PendidikanText))
                {
                    var pendidikan = _context.Pendidikans.FirstOrDefault(m => m.Name.Trim() == item.PendidikanText.Trim());
                    if (pendidikan == null)
                    {
                        pendidikan = new PendidikanModel
                        {
                            CreatedBy = User?.Identity.Name,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = User.Identity.Name,
                            UpdatedDate = DateTime.Now,
                            IsActive = true,
                            Name = item.PendidikanText,
                            Id = Guid.NewGuid().ToString()
                        };

                        _context.Pendidikans.Add(pendidikan);
                    }

                    pegawai.PendidikanID = pendidikan.Id;
                }

                if (!string.IsNullOrEmpty(item.ProdiText))
                {
                    var prodi = _context.Prodis.FirstOrDefault(m => m.Name.Trim() == item.ProdiText.Trim() && m.PendidikanID == pegawai.PendidikanID);
                    if (prodi == null)
                    {
                        prodi = new ProdiModel
                        {
                            CreatedBy = User?.Identity.Name,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = User.Identity.Name,
                            UpdatedDate = DateTime.Now,
                            IsActive = true,
                            Name = item.ProdiText,
                            Id = Guid.NewGuid().ToString(),

                            PendidikanID = pegawai.PendidikanID
                        };

                        _context.Prodis.Add(prodi);
                    }

                    pegawai.ProdiID = prodi.Id;
                }

                if (!string.IsNullOrEmpty(item.SatuanKerjaText))
                {
                    var prodi = _context.SatuanKerjas.FirstOrDefault(m => m.Name.Trim() == item.SatuanKerjaText.Trim());
                    if (prodi == null)
                    {
                        prodi = new SatuanKerjaWilayahModel
                        {
                            CreatedBy = User?.Identity.Name,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = User.Identity.Name,
                            UpdatedDate = DateTime.Now,
                            IsActive = true,
                            Name = item.SatuanKerjaText,
                            Id = Guid.NewGuid().ToString()
                        };

                        _context.SatuanKerjas.Add(prodi);
                    }

                    pegawai.SatuanKerjaID = prodi.Id;
                }

                if (!string.IsNullOrEmpty(item.UnitKerjaText))
                {
                    var prodi = _context.Poldas.FirstOrDefault(m => m.Name.Trim() == item.UnitKerjaText.Trim() && m.SatuanKerjaID == pegawai.UnitKerjaID);
                    if (prodi == null)
                    {
                        prodi = new PoldaModel
                        {
                            CreatedBy = User?.Identity.Name,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = User.Identity.Name,
                            UpdatedDate = DateTime.Now,
                            IsActive = true,
                            Name = item.UnitKerjaText,
                            Id = Guid.NewGuid().ToString(),

                            SatuanKerjaID = pegawai.SatuanKerjaID
                        };

                        _context.Poldas.Add(prodi);
                    }

                    pegawai.UnitKerjaID = prodi.Id;
                }

                pegawai.TanggalAwal = ConverterHelper.StringToDateTime(item.TanggalAwal);
                pegawai.TanggalAkhir = ConverterHelper.StringToDateTime(item.TanggalAkhir);
                pegawai.Keterangan = item.Keterangan;
                pegawai.IsActive = item.IsActive;

                if (newID == pegawai.Id)
                    _context.Pegawais.Add(pegawai);
                else
                    _context.Pegawais.Update(pegawai);
            }
        }

        private List<string> EnsureFilter(IEnumerable<string> items)
        {
            var itemList = items.ToList();
            while (itemList.Count < 4)
            {
                itemList.Add(string.Empty);
            }
            return itemList;
        }

        #region K-II
        public IActionResult K2()
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "K-II", Url = Url.Action("K2", "Pegawai"), IsActive = true },
            };
            return View(new PegawaiIndexViewModel
            {
                ListSatuanKerja =
                [
                    .. _context.SatuanKerjas.Where(m => m.IsActive).Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = m.Name,
                        Value = m.Id
                    }),
                ],
                ListUnitKerja =
                [
                    .. _context.Poldas.Where(m => m.IsActive).Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = m.Name,
                        Value = m.Id
                    }),
                ]
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetK2(BaseDTParameters param)
        {
            var results = new List<PegawaiViewModel>();

            if (param.AdditionalValues != null && param.AdditionalValues.Any())
            {
                var filters = param.AdditionalValues.ToList();
                filters = EnsureFilter(filters);
                results = await (from pegawai in _context.Pegawais.Where(m =>
                        m.NIK.ToLower().Contains((filters[0] ?? string.Empty).ToLower()) &&
                        m.Name.ToLower().Contains((filters[1] ?? string.Empty).ToLower()) &&
                        m.Type == Enums.PegawaiType.K2)
                                 join pen in _context.Pendidikans on pegawai.PendidikanID equals pen.Id into pendidikan
                                 from pen in pendidikan.DefaultIfEmpty()

                                 join prod in _context.Prodis on pegawai.ProdiID equals prod.Id into prodi
                                 from prod in prodi.DefaultIfEmpty()

                                 join satuanKerja in _context.SatuanKerjas on pegawai.SatuanKerjaID equals satuanKerja.Id into SK
                                 from satuanKerja in SK.DefaultIfEmpty()

                                 join unitKerja in _context.Poldas on pegawai.UnitKerjaID equals unitKerja.Id into P
                                 from unitKerja in P.DefaultIfEmpty()

                                 select new PegawaiViewModel
                                 {
                                     Id = pegawai.Id,
                                     Nomor = pegawai.Nomor,
                                     NIK = pegawai.NIK,
                                     JenisKelaminText = EnumHelper.GetDescription(pegawai.JenisKelamin),
                                     PendidikanText = pen.Name,
                                     ProdiText = prod.Name,
                                     SatuanKerjaID = satuanKerja.Id ?? string.Empty,
                                     SatuanKerjaText = satuanKerja.Name,
                                     UnitKerjaID = unitKerja.Id ?? string.Empty,
                                     UnitKerjaText = unitKerja.Name,
                                     TanggalAwal = ConverterHelper.DateTimeToString(pegawai.TanggalAwal),
                                     TanggalAkhir = ConverterHelper.DateTimeToString(pegawai.TanggalAkhir),
                                     Keterangan = pegawai.Keterangan,
                                     IsActive = pegawai.IsActive,
                                     Name = pegawai.Name,
                                     CreatedBy = pegawai.CreatedBy,
                                     CreatedDate = pegawai.CreatedDate,
                                     UpdatedBy = pegawai.UpdatedBy,
                                     UpdatedDate = pegawai.UpdatedDate
                                 }).OrderByDescending(m => m.IsActive).ThenByDescending(m => m.CreatedDate)
                                 .Where(m => m.SatuanKerjaID.Contains(filters[2] ?? string.Empty) &&
                                             m.UnitKerjaID.Contains(filters[3] ?? string.Empty)).ToListAsync();
            }

            return new JsonResult(DataTablePagedHelper.GetDatatablePaged(results, param));
        }

        public IActionResult CreateK2()
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "K-II", Url = Url.Action("K2", "Pegawai") },
                new() { Title = "Create K-II", Url = Url.Action("Create", "Pegawai"), IsActive = true },
            };
            return View(InitModel(new PegawaiViewModel()));
        }

        public IActionResult EditK2(string id)
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "K-II", Url = Url.Action("K2", "Pegawai") },
                new() { Title = "Edit K-II", Url = Url.Action("Create", "Pegawai"), IsActive = true },
            };

            var existingData = _context.Pegawais.FirstOrDefault(m => m.Id == id);
            if (existingData == null)
            {
                Alert(Constans.Label.DataNotFound, Enums.NotificationType.error);
                return RedirectToAction("K2", "Master");
            }

            var model = new PegawaiViewModel()
            {
                Id = existingData.Id,
                Name = existingData.Name,
                NIK = existingData.NIK,
                Nomor = existingData.Nomor,
                JenisKelamin = existingData.JenisKelamin,
                PendidikanID = existingData.PendidikanID,
                ProdiID = existingData.ProdiID,
                SatuanKerjaID = existingData.SatuanKerjaID,
                UnitKerjaID = existingData.UnitKerjaID,
                TanggalAwal = ConverterHelper.DateTimeToString(existingData.TanggalAwal),
                TanggalAkhir = ConverterHelper.DateTimeToString(existingData.TanggalAkhir),
                Keterangan = existingData.Keterangan,
                CreatedBy = existingData.CreatedBy,
                CreatedDate = existingData.CreatedDate,
                IsActive = existingData.IsActive,
                UpdatedBy = existingData.UpdatedBy,
                UpdatedDate = existingData.UpdatedDate
            };

            return View(InitModel(model));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditK2(PegawaiViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingK2 = _context.Pegawais.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.NIK == model.NIK && m.IsActive && m.Type == Enums.PegawaiType.K2);
                    if (!string.IsNullOrEmpty(model.Id))
                    {
                        existingK2 = _context.Pegawais.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.NIK == model.NIK && m.IsActive && m.Id != model.Id && m.Type == Enums.PegawaiType.K2);
                    }

                    if (existingK2)
                    {
                        Alert(string.Format(Constans.Label.AlreadyExists, model.Name), Enums.NotificationType.error);
                        return View("CreateK2", InitModel(model));
                    }
                    var entity = new PegawaiModel
                    {
                        Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id,
                        Name = model.Name,
                        NIK = model.NIK,
                        Nomor = model.Nomor,
                        JenisKelamin = model.JenisKelamin,
                        PendidikanID = model.PendidikanID,
                        ProdiID = model.ProdiID,
                        SatuanKerjaID = model.SatuanKerjaID,
                        UnitKerjaID = model.UnitKerjaID,
                        TanggalAwal = ConverterHelper.StringToDateTime(model?.TanggalAwal),
                        TanggalAkhir = ConverterHelper.StringToDateTime(model?.TanggalAkhir),
                        Keterangan = model?.Keterangan,
                        IsActive = model.IsActive,
                        CreatedBy = User?.Identity.Name,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = User.Identity.Name,
                        UpdatedDate = DateTime.Now,
                        Type = Enums.PegawaiType.K2
                    };

                    if (string.IsNullOrEmpty(model.Id))
                        _context.Add(entity);
                    else
                        _context.Update(entity);

                    await _context.SaveChangesAsync();

                    Alert(Constans.Label.SavedSuccess, Enums.NotificationType.success);
                    return RedirectToAction("K2", "Pegawai");
                }
                else
                {
                    var message = string.Empty;
                    foreach (var item in ModelState.Where(m => m.Value.ValidationState != Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid))
                    {
                        message += "<br/>" + item.Value.Errors[0].ErrorMessage;
                    }
                    Alert(message, Enums.NotificationType.error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");

                Alert(ex.Message, Enums.NotificationType.error);
            }
            if (string.IsNullOrEmpty(model.Id))
                return View("CreateK2", InitModel(model));
            else
                return View("EditK2", InitModel(model));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteK2(string id)
        {
            var existingData = _context.Pegawais.FirstOrDefault(m => m.Id == id);
            if (existingData == null)
                return Ok(new { isSuccess = false, error = Constans.Label.DataNotFound });
            else
            {
                _context.Pegawais.Remove(existingData);
                await _context.SaveChangesAsync();
                return Ok(new { isSuccess = true });
            }
        }


        [HttpPost]
        public async Task<IActionResult> UploadK2(IFormFile file)
        {
            var message = string.Empty;
            bool systemError = false;
            try
            {
                systemError = false;

                if (!Path.GetExtension(file.FileName).Contains(".xls", StringComparison.OrdinalIgnoreCase))
                    return Json(new { issuccess = systemError, error = "Format is not correct." });

                byte[] fileBytes = [];
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                };

                var datas = GetPegawaiModelFromExcel(fileBytes, Enums.PegawaiType.K2);
                SaveDataFromExcel(datas);
                await _context.SaveChangesAsync();


                return Json(new { issuccess = systemError, error = "Data berhasil di Upload." });
            }
            catch (Exception ex)
            {
                systemError = true;
                message = ex.Message;
            }

            return Json(new { issuccess = systemError, error = message });
        }


        #endregion

        #region PHL
        public IActionResult PHL()
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "PHL", Url = Url.Action("PHL", "Pegawai"), IsActive = true }
            };
            return View(new PegawaiIndexViewModel
            {
                ListSatuanKerja =
                [
                    .. _context.SatuanKerjas.Where(m => m.IsActive).Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = m.Name,
                        Value = m.Id
                    }),
                ],
                ListUnitKerja =
                [
                    .. _context.Poldas.Where(m => m.IsActive).Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = m.Name,
                        Value = m.Id
                    }),
                ]
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetPHL(BaseDTParameters param)
        {
            var results = new List<PegawaiViewModel>();

            if (param.AdditionalValues != null && param.AdditionalValues.Any())
            {
                var filters = param.AdditionalValues.ToList();
                filters = EnsureFilter(filters);
                results = await (from pegawai in _context.Pegawais.Where(m =>
                        m.NIK.ToLower().Contains((filters[0] ?? string.Empty).ToLower()) &&
                        m.Name.ToLower().Contains((filters[1] ?? string.Empty).ToLower()) &&
                        m.Type == Enums.PegawaiType.PHL)
                                 join pen in _context.Pendidikans on pegawai.PendidikanID equals pen.Id into pendidikan
                                 from pen in pendidikan.DefaultIfEmpty()

                                 join prod in _context.Prodis on pegawai.ProdiID equals prod.Id into prodi
                                 from prod in prodi.DefaultIfEmpty()

                                 join satuanKerja in _context.SatuanKerjas on pegawai.SatuanKerjaID equals satuanKerja.Id into SK
                                 from satuanKerja in SK.DefaultIfEmpty()

                                 join unitKerja in _context.Poldas on pegawai.UnitKerjaID equals unitKerja.Id into P
                                 from unitKerja in P.DefaultIfEmpty()

                                 select new PegawaiViewModel
                                 {
                                     Id = pegawai.Id,
                                     NIK = pegawai.NIK,
                                     JenisKelaminText = EnumHelper.GetDescription(pegawai.JenisKelamin),
                                     PendidikanText = pen.Name,
                                     ProdiText = prod.Name,
                                     SatuanKerjaID = satuanKerja.Id ?? string.Empty,
                                     SatuanKerjaText = satuanKerja.Name,
                                     UnitKerjaID = unitKerja.Id ?? string.Empty,
                                     UnitKerjaText = unitKerja.Name,
                                     TanggalAwal = ConverterHelper.DateTimeToString(pegawai.TanggalAwal),
                                     TanggalAkhir = ConverterHelper.DateTimeToString(pegawai.TanggalAkhir),
                                     Keterangan = pegawai.Keterangan,
                                     IsActive = pegawai.IsActive,
                                     Name = pegawai.Name,
                                     CreatedBy = pegawai.CreatedBy,
                                     CreatedDate = pegawai.CreatedDate,
                                     UpdatedBy = pegawai.UpdatedBy,
                                     UpdatedDate = pegawai.UpdatedDate
                                 }).OrderByDescending(m => m.IsActive).ThenByDescending(m => m.CreatedDate)
                                 .Where(m => m.SatuanKerjaID.Contains(filters[2] ?? string.Empty) &&
                                             m.UnitKerjaID.Contains(filters[3] ?? string.Empty)).ToListAsync();
            }

            return new JsonResult(DataTablePagedHelper.GetDatatablePaged(results, param));
        }

        public IActionResult CreatePHL()
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "PHL", Url = Url.Action("PHL", "Pegawai") },
                new() { Title = "Create PHL", Url = Url.Action("CreatePHL", "Pegawai"), IsActive = true },
            };
            return View(InitModel(new PegawaiViewModel()));
        }

        public IActionResult EditPHL(string id)
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "PHL", Url = Url.Action("PHL", "Pegawai") },
                new() { Title = "Edit PHL", Url = Url.Action("CreatePHL", "Pegawai"), IsActive = true },
            };
            var existingData = _context.Pegawais.FirstOrDefault(m => m.Id == id);
            if (existingData == null)
            {
                Alert(Constans.Label.DataNotFound, Enums.NotificationType.error);
                return RedirectToAction("PHL", "Master");
            }

            var model = new PegawaiViewModel()
            {
                Id = existingData.Id,
                Name = existingData.Name,
                NIK = existingData.NIK,
                Type = Enums.PegawaiType.PHL,
                JenisKelamin = existingData.JenisKelamin,
                PendidikanID = existingData.PendidikanID,
                ProdiID = existingData.ProdiID,
                SatuanKerjaID = existingData.SatuanKerjaID,
                UnitKerjaID = existingData.UnitKerjaID,
                TanggalAwal = ConverterHelper.DateTimeToString(existingData.TanggalAwal),
                TanggalAkhir = ConverterHelper.DateTimeToString(existingData.TanggalAkhir),
                Keterangan = existingData.Keterangan,
                CreatedBy = existingData.CreatedBy,
                CreatedDate = existingData.CreatedDate,
                IsActive = existingData.IsActive,
                UpdatedBy = existingData.UpdatedBy,
                UpdatedDate = existingData.UpdatedDate
            };

            return View(InitModel(model));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditPHL(PegawaiViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingPHL = _context.Pegawais.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.NIK == model.NIK && m.IsActive && m.Type == Enums.PegawaiType.PHL);
                    if (!string.IsNullOrEmpty(model.Id))
                    {
                        existingPHL = _context.Pegawais.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.NIK == model.NIK && m.IsActive && m.Id != model.Id && m.Type == Enums.PegawaiType.PHL);
                    }

                    if (existingPHL)
                    {
                        Alert(string.Format(Constans.Label.AlreadyExists, model.Name), Enums.NotificationType.error);
                        return View("CreatePHL", InitModel(model));
                    }
                    var entity = new PegawaiModel
                    {
                        Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id,
                        Name = model.Name,
                        NIK = model.NIK,
                        JenisKelamin = model.JenisKelamin,
                        PendidikanID = model.PendidikanID,
                        ProdiID = model.ProdiID,
                        SatuanKerjaID = model.SatuanKerjaID,
                        UnitKerjaID = model.UnitKerjaID,
                        TanggalAwal = ConverterHelper.StringToDateTime(model?.TanggalAwal),
                        TanggalAkhir = ConverterHelper.StringToDateTime(model?.TanggalAkhir),
                        Keterangan = model?.Keterangan,
                        IsActive = model.IsActive,
                        CreatedBy = User?.Identity.Name,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = User.Identity.Name,
                        UpdatedDate = DateTime.Now,
                        Type = Enums.PegawaiType.PHL
                    };

                    if (string.IsNullOrEmpty(model.Id))
                        _context.Add(entity);
                    else
                        _context.Update(entity);

                    await _context.SaveChangesAsync();

                    Alert(Constans.Label.SavedSuccess, Enums.NotificationType.success);
                    return RedirectToAction("PHL", "Pegawai");
                }
                else
                {
                    var message = string.Empty;
                    foreach (var item in ModelState.Where(m => m.Value.ValidationState != Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid))
                    {
                        message += "<br/>" + item.Value.Errors[0].ErrorMessage;
                    }
                    Alert(message, Enums.NotificationType.error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");

                Alert(ex.Message, Enums.NotificationType.error);
            }
            if (string.IsNullOrEmpty(model.Id))
                return View("CreatePHL", InitModel(model));
            else
                return View("EditPHL", InitModel(model));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePHL(string id)
        {
            var existingData = _context.Pegawais.FirstOrDefault(m => m.Id == id);
            if (existingData == null)
                return Ok(new { isSuccess = false, error = Constans.Label.DataNotFound });
            else
            {
                _context.Pegawais.Remove(existingData);
                await _context.SaveChangesAsync();
                return Ok(new { isSuccess = true });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadPHL(IFormFile file)
        {
            var message = string.Empty;
            bool systemError = false;
            try
            {
                systemError = false;

                if (!Path.GetExtension(file.FileName).Contains(".xls", StringComparison.OrdinalIgnoreCase))
                    return Json(new { issuccess = systemError, error = "Format is not correct." });

                byte[] fileBytes = [];
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                };

                var datas = GetPegawaiModelFromExcel(fileBytes, Enums.PegawaiType.PHL);
                SaveDataFromExcel(datas);
                await _context.SaveChangesAsync();


                return Json(new { issuccess = systemError, error = "Data berhasil di Upload." });
            }
            catch (Exception ex)
            {
                systemError = true;
                message = ex.Message;
            }

            return Json(new { issuccess = systemError, error = message });
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> DeleteAll(string ids)
        {
            try
            {
                if (string.IsNullOrEmpty(ids))
                {
                    return BadRequest(new { isSuccess = false, error = "No IDs provided." });
                }

                var idArray = ids.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                foreach (var id in idArray)
                {
                    // Assume you have a service to delete the data by ID
                    var pegawai = _context.Pegawais.FirstOrDefault(m => m.Id == id);
                    if (pegawai != null)
                        _context.Pegawais.Remove(pegawai);
                }
                await _context.SaveChangesAsync();
                return Ok(new { isSuccess = true });

            }
            catch (Exception)
            {
                return Ok(new { isSuccess = false });
            }
        }

        public List<Item> GetTotalPendidikans(bool isPHL)
        {
            var result = new List<Item>();

            List<PegawaiViewModel> query =
            [
                .. (from pegawai in _context.Pegawais.Where(m => m.Type == PegawaiType.PHL)
                    join pen in _context.Pendidikans on pegawai.PendidikanID equals pen.Id into pendidikan
                    from pen in pendidikan.DefaultIfEmpty()
                    select new PegawaiViewModel
                    {
                        Id = pegawai.Id,
                        JenisKelaminText = pegawai.JenisKelamin.HasValue ? EnumHelper.GetDescription(pegawai.JenisKelamin) : "-",
                        PendidikanText = pen.Name,
                    }),
            ];
            if (!isPHL)
            {
                query =
                [
                    .. (from pegawai in _context.Pegawais.Where(m => m.Type == PegawaiType.K2)
                        join pen in _context.Pendidikans on pegawai.PendidikanID equals pen.Id into pendidikan
                        from pen in pendidikan.DefaultIfEmpty()
                        select new PegawaiViewModel
                        {
                            Id = pegawai.Id,
                            JenisKelaminText = pegawai.JenisKelamin.HasValue ? EnumHelper.GetDescription(pegawai.JenisKelamin) : "-",
                            PendidikanText = pen.Name,
                        }),
                ];
            }

            var totalPendidikan = query.GroupBy(m => m.PendidikanText)
                .Select(m => new { Text = m.Key, Value = m.Count() });
            foreach (var item in totalPendidikan.OrderBy(m => m.Text))
            {
                result.Add(new Item { Text = item?.Text ?? "-", Total = item?.Value ?? 0 });
            }

            return result;
        }
    }
}
