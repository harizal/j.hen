using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WApp.Datas;
using WApp.Helpers;
using WApp.Models;
using WApp.Utlis;
using WApp.ViewModels;
using WApp.ViewModels.Parameters;

namespace WApp.Controllers
{
    public class PegawaiController : BaseController
    {
        private readonly ILogger<PegawaiController> _logger;
        private readonly AppDataContext _context;
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

        #region K-II
        public IActionResult K2()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetK2(BaseDTParameters param)
        {
            var results = new List<PegawaiViewModel>();

            if (param.AdditionalValues != null && param.AdditionalValues.Any())
            {
                var filters = param.AdditionalValues.ToList();
                results = await (from pegawai in _context.Pegawais.Where(m =>
                        m.Nomor.ToLower().Contains((filters[0] ?? string.Empty).ToLower()) &&
                        m.NIK.ToLower().Contains((filters[1] ?? string.Empty).ToLower()) &&
                        m.Name.ToLower().Contains((filters[2] ?? string.Empty).ToLower()) &&
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
                                     SatuanKerjaText = satuanKerja.Name,
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
                                 }).OrderByDescending(m => m.IsActive).ThenBy(m => m.Name).ToListAsync();
            }

            return new JsonResult(DataTablePagedHelper.GetDatatablePaged(results, param));
        }

        public IActionResult CreateK2()
        {
            return View(InitModel(new PegawaiViewModel()));
        }

        public IActionResult EditK2(string id)
        {
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
                    var existingK2 = _context.Pegawais.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.IsActive && m.Type == Enums.PegawaiType.K2);
                    if (!string.IsNullOrEmpty(model.Id))
                    {
                        existingK2 = _context.Pegawais.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.IsActive && m.Id != model.Id && m.Type == Enums.PegawaiType.K2);
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
        #endregion

        #region PHL
        public IActionResult PHL()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetPHL(BaseDTParameters param)
        {
            var results = new List<PegawaiViewModel>();

            if (param.AdditionalValues != null && param.AdditionalValues.Any())
            {
                var filters = param.AdditionalValues.ToList();
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
                                     SatuanKerjaText = satuanKerja.Name,
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
                                 }).OrderByDescending(m => m.IsActive).ThenBy(m => m.Name).ToListAsync();
            }

            return new JsonResult(DataTablePagedHelper.GetDatatablePaged(results, param));
        }

        public IActionResult CreatePHL()
        {
            return View(InitModel(new PegawaiViewModel()));
        }

        public IActionResult EditPHL(string id)
        {
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
                    var existingPHL = _context.Pegawais.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.IsActive && m.Type == Enums.PegawaiType.PHL);
                    if (!string.IsNullOrEmpty(model.Id))
                    {
                        existingPHL = _context.Pegawais.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.IsActive && m.Id != model.Id && m.Type == Enums.PegawaiType.PHL);
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
        #endregion
    }
}
