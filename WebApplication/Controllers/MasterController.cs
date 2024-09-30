using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [Authorize]
    public class MasterController : BaseController
    {
        private readonly ILogger<MasterController> _logger;
        private readonly AppDataContext _context;

        public MasterController(ILogger<MasterController> logger, AppDataContext context)
        {
            _logger = logger;
            _context = context;
        }

        #region POLDA
        public IActionResult Polda()
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "Unit Kerja", IsActive = true },
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetPolda(BaseDTParameters param)
        {
            var results = new List<PoldaViewModel>();

            if (param.AdditionalValues != null && param.AdditionalValues.Any())
            {
                var filters = param.AdditionalValues.ToList();
                var query = _context.Poldas.Where(m => m.Name.ToLower().Contains((filters[0] ?? string.Empty).ToLower()))
                    .OrderByDescending(m => m.IsActive).ThenBy(m => m.Name)
                    .Select(m => new PoldaViewModel
                    {
                        Id = m.Id,
                        CreatedBy = m.CreatedBy,
                        CreatedDate = m.CreatedDate,
                        IsActive = m.IsActive,
                        Name = m.Name,
                        UpdatedBy = m.UpdatedBy,
                        UpdatedDate = m.UpdatedDate
                    });

                results = await query.ToListAsync();
            }

            return new JsonResult(DataTablePagedHelper.GetDatatablePaged(results, param));
        }

        public IActionResult CreatePolda()
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "Unit Kerja", Url = Url.Action("Polda", "Master") },
                new() { Title = "Create Unit Kerja", IsActive = true },
            };
            return View(new PoldaViewModel());
        }

        public IActionResult EditPolda(string id)
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "Unit Kerja", Url = Url.Action("Polda", "Master") },
                new() { Title = "Edit Unit Kerja", IsActive = true },
            };
            var existingData = _context.Poldas.FirstOrDefault(m => m.Id == id);
            if (existingData == null)
            {
                Alert(Constans.Label.DataNotFound, Enums.NotificationType.error);
                return RedirectToAction("Polda", "Master");
            }

            return View(new PoldaViewModel()
            {
                Id = existingData.Id,
                CreatedBy = existingData.CreatedBy,
                CreatedDate = existingData.CreatedDate,
                IsActive = existingData.IsActive,
                Name = existingData.Name,
                UpdatedBy = existingData.UpdatedBy,
                UpdatedDate = existingData.UpdatedDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditPolda(PoldaViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingPolda = _context.Poldas.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.IsActive);
                    if (!string.IsNullOrEmpty(model.Id))
                    {
                        existingPolda = _context.Poldas.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.IsActive && m.Id != model.Id);
                    }

                    if (existingPolda)
                    {
                        Alert(string.Format(Constans.Label.AlreadyExists, model.Name), Enums.NotificationType.error);
                        return View("CreatePolda", model);
                    }
                    var entity = new PoldaModel
                    {
                        Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id,
                        Name = model.Name,
                        IsActive = model.IsActive,
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = User.Identity.Name,
                        UpdatedDate = DateTime.Now,
                    };

                    if (string.IsNullOrEmpty(model.Id))
                        _context.Add(entity);
                    else
                        _context.Update(entity);
                    
                    await _context.SaveChangesAsync();

                    Alert(Constans.Label.SavedSuccess, Enums.NotificationType.success);
                    return RedirectToAction("Polda", "Master");
                }
                else
                {
                    var message = string.Empty;
                    foreach (var item in ModelState.Where(m => m.Value.ValidationState != Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid))
                    {
                        message += Environment.NewLine + item.Value.Errors[0].ErrorMessage;
                    }
                    Alert(message, Enums.NotificationType.error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");

                Alert(ex.Message, Enums.NotificationType.error);
            }
            return View("CreatePolda", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePolda(string id)
        {
            var existingData = _context.Poldas.FirstOrDefault(m => m.Id == id);
            if (existingData == null)
                return Ok(new { isSuccess = false, error = Constans.Label.DataNotFound });
            else
            {
                _context.Poldas.Remove(existingData);
                await _context.SaveChangesAsync();
                return Ok(new { isSuccess = true });
            }
        }
        #endregion

        #region PENDIDIKAN
        public IActionResult Pendidikan()
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "Pendidikan", IsActive = true },
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetPendidikan(BaseDTParameters param)
        {
            var results = new List<PendidikanViewModel>();

            if (param.AdditionalValues != null && param.AdditionalValues.Any())
            {
                var filters = param.AdditionalValues.ToList();
                var query = _context.Pendidikans.Where(m => m.Name.ToLower().Contains((filters[0] ?? string.Empty).ToLower()))
                    .OrderByDescending(m => m.IsActive).ThenBy(m => m.Name)
                    .Select(m => new PendidikanViewModel
                    {
                        Id = m.Id,
                        CreatedBy = m.CreatedBy,
                        CreatedDate = m.CreatedDate,
                        IsActive = m.IsActive,
                        Name = m.Name,
                        UpdatedBy = m.UpdatedBy,
                        UpdatedDate = m.UpdatedDate
                    });

                results = await query.ToListAsync();
            }

            return new JsonResult(DataTablePagedHelper.GetDatatablePaged(results, param));
        }

        public IActionResult CreatePendidikan()
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "Pendidikan", Url = Url.Action("Pendidikan", "Master") },
                new() { Title = "Create Pendidikan", IsActive = true },
            };

            return View(new PendidikanViewModel());
        }

        public IActionResult EditPendidikan(string id)
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "Pendidikan", Url = Url.Action("Pendidikan", "Master") },
                new() { Title = "Edit Pendidikan", IsActive = true },
            };

            var existingData = _context.Pendidikans.FirstOrDefault(m => m.Id == id);
            if (existingData == null)
            {
                Alert(Constans.Label.DataNotFound, Enums.NotificationType.error);
                return RedirectToAction("Pendidikan", "Master");
            }

            return View(new PendidikanViewModel()
            {
                Id = existingData.Id,
                CreatedBy = existingData.CreatedBy,
                CreatedDate = existingData.CreatedDate,
                IsActive = existingData.IsActive,
                Name = existingData.Name,
                UpdatedBy = existingData.UpdatedBy,
                UpdatedDate = existingData.UpdatedDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditPendidikan(PendidikanViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingPendidikan = _context.Pendidikans.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.IsActive);
                    if (!string.IsNullOrEmpty(model.Id))
                    {
                        existingPendidikan = _context.Pendidikans.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.IsActive && m.Id != model.Id);
                    }

                    if (existingPendidikan)
                    {
                        Alert(string.Format(Constans.Label.AlreadyExists, model.Name), Enums.NotificationType.error);
                        return View("CreatePendidikan", model);
                    }
                    var entity = new PendidikanModel
                    {
                        Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id,
                        Name = model.Name,
                        IsActive = model.IsActive,
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = User.Identity.Name,
                        UpdatedDate = DateTime.Now,
                    };

                    if (string.IsNullOrEmpty(model.Id))
                        _context.Add(entity);
                    else
                        _context.Update(entity);

                    await _context.SaveChangesAsync();

                    Alert(Constans.Label.SavedSuccess, Enums.NotificationType.success);
                    return RedirectToAction("Pendidikan", "Master");
                }
                else
                {
                    var message = string.Empty;
                    foreach (var item in ModelState.Where(m => m.Value.ValidationState != Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid))
                    {
                        message += Environment.NewLine + item.Value.Errors[0].ErrorMessage;
                    }
                    Alert(message, Enums.NotificationType.error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");

                Alert(ex.Message, Enums.NotificationType.error);
            }
            return View("CreatePendidikan", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePendidikan(string id)
        {
            var existingData = _context.Pendidikans.FirstOrDefault(m => m.Id == id);
            if (existingData == null)
                return Ok(new { isSuccess = false, error = Constans.Label.DataNotFound });
            else
            {
                _context.Pendidikans.Remove(existingData);
                await _context.SaveChangesAsync();
                return Ok(new { isSuccess = true });
            }
        }
        #endregion

        #region Prodi
        public IActionResult Prodi()
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "Prodi", IsActive = true },
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetProdi(BaseDTParameters param)
        {
            var results = new List<ProdiViewModel>();

            if (param.AdditionalValues != null && param.AdditionalValues.Any())
            {
                var filters = param.AdditionalValues.ToList();
                var query = _context.Prodis.Where(m => m.Name.ToLower().Contains((filters[0] ?? string.Empty).ToLower()))
                    .OrderByDescending(m => m.IsActive).ThenBy(m => m.Name)
                    .Select(m => new ProdiViewModel
                    {
                        Id = m.Id,
                        CreatedBy = m.CreatedBy,
                        CreatedDate = m.CreatedDate,
                        IsActive = m.IsActive,
                        Name = m.Name,
                        UpdatedBy = m.UpdatedBy,
                        UpdatedDate = m.UpdatedDate
                    });

                results = await query.ToListAsync();
            }

            return new JsonResult(DataTablePagedHelper.GetDatatablePaged(results, param));
        }

        public IActionResult CreateProdi()
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "Prodi", Url = Url.Action("Prodi", "Master") },
                new() { Title = "Create Prodi", IsActive = true },
            };
            return View(new ProdiViewModel());
        }

        public IActionResult EditProdi(string id)
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "Prodi", Url = Url.Action("Prodi", "Master") },
                new() { Title = "Edit Prodi", IsActive = true },
            };

            var existingData = _context.Prodis.FirstOrDefault(m => m.Id == id);
            if (existingData == null)
            {
                Alert(Constans.Label.DataNotFound, Enums.NotificationType.error);
                return RedirectToAction("Prodi", "Master");
            }

            return View(new ProdiViewModel()
            {
                Id = existingData.Id,
                CreatedBy = existingData.CreatedBy,
                CreatedDate = existingData.CreatedDate,
                IsActive = existingData.IsActive,
                Name = existingData.Name,
                UpdatedBy = existingData.UpdatedBy,
                UpdatedDate = existingData.UpdatedDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditProdi(ProdiViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingProdi = _context.Prodis.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.IsActive);
                    if (!string.IsNullOrEmpty(model.Id))
                    {
                        existingProdi = _context.Prodis.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.IsActive && m.Id != model.Id);
                    }

                    if (existingProdi)
                    {
                        Alert(string.Format(Constans.Label.AlreadyExists, model.Name), Enums.NotificationType.error);
                        return View("CreateProdi", model);
                    }
                    var entity = new ProdiModel
                    {
                        Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id,
                        Name = model.Name,
                        IsActive = model.IsActive,
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = User.Identity.Name,
                        UpdatedDate = DateTime.Now,
                    };

                    if (string.IsNullOrEmpty(model.Id))
                        _context.Add(entity);
                    else
                        _context.Update(entity);

                    await _context.SaveChangesAsync();

                    Alert(Constans.Label.SavedSuccess, Enums.NotificationType.success);
                    return RedirectToAction("Prodi", "Master");
                }
                else
                {
                    var message = string.Empty;
                    foreach (var item in ModelState.Where(m => m.Value.ValidationState != Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid))
                    {
                        message += Environment.NewLine + item.Value.Errors[0].ErrorMessage;
                    }
                    Alert(message, Enums.NotificationType.error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");

                Alert(ex.Message, Enums.NotificationType.error);
            }
            return View("CreateProdi", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProdi(string id)
        {
            var existingData = _context.Prodis.FirstOrDefault(m => m.Id == id);
            if (existingData == null)
                return Ok(new { isSuccess = false, error = Constans.Label.DataNotFound });
            else
            {
                _context.Prodis.Remove(existingData);
                await _context.SaveChangesAsync();
                return Ok(new { isSuccess = true });
            }
        }
        #endregion

        #region StatusPerkawinan
        public IActionResult Status()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetStatus(BaseDTParameters param)
        {
            var results = new List<StatusViewModel>();

            if (param.AdditionalValues != null && param.AdditionalValues.Any())
            {
                var filters = param.AdditionalValues.ToList();
                var query = _context.Statuses.Where(m => m.Name.ToLower().Contains((filters[0] ?? string.Empty).ToLower()))
                    .OrderByDescending(m => m.IsActive).ThenBy(m => m.Name)
                    .Select(m => new StatusViewModel
                    {
                        Id = m.Id,
                        CreatedBy = m.CreatedBy,
                        CreatedDate = m.CreatedDate,
                        IsActive = m.IsActive,
                        Name = m.Name,
                        UpdatedBy = m.UpdatedBy,
                        UpdatedDate = m.UpdatedDate
                    });

                results = await query.ToListAsync();
            }

            return new JsonResult(DataTablePagedHelper.GetDatatablePaged(results, param));
        }

        public IActionResult CreateStatus()
        {
            return View(new StatusViewModel());
        }

        public IActionResult EditStatus(string id)
        {
            var existingData = _context.Statuses.FirstOrDefault(m => m.Id == id);
            if (existingData == null)
            {
                Alert(Constans.Label.DataNotFound, Enums.NotificationType.error);
                return RedirectToAction("Status", "Master");
            }

            return View(new StatusViewModel()
            {
                Id = existingData.Id,
                CreatedBy = existingData.CreatedBy,
                CreatedDate = existingData.CreatedDate,
                IsActive = existingData.IsActive,
                Name = existingData.Name,
                UpdatedBy = existingData.UpdatedBy,
                UpdatedDate = existingData.UpdatedDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditStatus(StatusViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingStatus = _context.Statuses.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.IsActive);
                    if (!string.IsNullOrEmpty(model.Id))
                    {
                        existingStatus = _context.Statuses.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.IsActive && m.Id != model.Id);
                    }

                    if (existingStatus)
                    {
                        Alert(string.Format(Constans.Label.AlreadyExists, model.Name), Enums.NotificationType.error);
                        return View("CreateStatus", model);
                    }
                    var entity = new StatusModel
                    {
                        Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id,
                        Name = model.Name,
                        IsActive = model.IsActive,
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = User.Identity.Name,
                        UpdatedDate = DateTime.Now,
                    };

                    if (string.IsNullOrEmpty(model.Id))
                        _context.Add(entity);
                    else
                        _context.Update(entity);

                    await _context.SaveChangesAsync();

                    Alert(Constans.Label.SavedSuccess, Enums.NotificationType.success);
                    return RedirectToAction("Status", "Master");
                }
                else
                {
                    var message = string.Empty;
                    foreach (var item in ModelState.Where(m => m.Value.ValidationState != Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid))
                    {
                        message += Environment.NewLine + item.Value.Errors[0].ErrorMessage;
                    }
                    Alert(message, Enums.NotificationType.error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");

                Alert(ex.Message, Enums.NotificationType.error);
            }
            return View("CreateStatus", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStatus(string id)
        {
            var existingData = _context.Statuses.FirstOrDefault(m => m.Id == id);
            if (existingData == null)
                return Ok(new { isSuccess = false, error = Constans.Label.DataNotFound });
            else
            {
                _context.Statuses.Remove(existingData);
                await _context.SaveChangesAsync();
                return Ok(new { isSuccess = true });
            }
        }
        #endregion

        #region SatuanKerja
        public IActionResult SatuanKerja()
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "Satuan Kerja", IsActive = true },
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetSatuanKerja(BaseDTParameters param)
        {
            var results = new List<SatuanKerjaViewModel>();

            if (param.AdditionalValues != null && param.AdditionalValues.Any())
            {
                var filters = param.AdditionalValues.ToList();
                var query = _context.SatuanKerjas.Where(m => m.Name.ToLower().Contains((filters[0] ?? string.Empty).ToLower()))
                    .OrderByDescending(m => m.IsActive).ThenBy(m => m.Name)
                    .Select(m => new SatuanKerjaViewModel
                    {
                        Id = m.Id,
                        CreatedBy = m.CreatedBy,
                        CreatedDate = m.CreatedDate,
                        IsActive = m.IsActive,
                        Name = m.Name,
                        UpdatedBy = m.UpdatedBy,
                        UpdatedDate = m.UpdatedDate
                    });

                results = await query.ToListAsync();
            }

            return new JsonResult(DataTablePagedHelper.GetDatatablePaged(results, param));
        }

        public IActionResult CreateSatuanKerja()
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "Satuan Kerja", Url = Url.Action("SatuanKerja", "Master") },
                new() { Title = "Create Satuan Kerja", IsActive = true },
            };

            return View(new SatuanKerjaViewModel());
        }

        public IActionResult EditSatuanKerja(string id)
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "Satuan Kerja", Url = Url.Action("SatuanKerja", "Master") },
                new() { Title = "Edit Satuan Kerja", IsActive = true },
            };

            var existingData = _context.SatuanKerjas.FirstOrDefault(m => m.Id == id);
            if (existingData == null)
            {
                Alert(Constans.Label.DataNotFound, Enums.NotificationType.error);
                return RedirectToAction("SatuanKerja", "Master");
            }

            return View(new SatuanKerjaViewModel()
            {
                Id = existingData.Id,
                CreatedBy = existingData.CreatedBy,
                CreatedDate = existingData.CreatedDate,
                IsActive = existingData.IsActive,
                Name = existingData.Name,
                UpdatedBy = existingData.UpdatedBy,
                UpdatedDate = existingData.UpdatedDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditSatuanKerja(SatuanKerjaViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingSatuanKerja = _context.SatuanKerjas.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.IsActive);
                    if (!string.IsNullOrEmpty(model.Id))
                    {
                        existingSatuanKerja = _context.SatuanKerjas.Any(m => m.Name.Trim().ToLower() == model.Name.Trim().ToLower() && m.IsActive && m.Id != model.Id);
                    }

                    if (existingSatuanKerja)
                    {
                        Alert(string.Format(Constans.Label.AlreadyExists, model.Name), Enums.NotificationType.error);
                        return View("CreateSatuanKerja", model);
                    }
                    var entity = new SatuanKerjaWilayahModel
                    {
                        Id = string.IsNullOrEmpty(model.Id) ? Guid.NewGuid().ToString() : model.Id,
                        Name = model.Name,
                        IsActive = model.IsActive,
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = User.Identity.Name,
                        UpdatedDate = DateTime.Now,
                    };

                    if (string.IsNullOrEmpty(model.Id))
                        _context.Add(entity);
                    else
                        _context.Update(entity);

                    await _context.SaveChangesAsync();

                    Alert(Constans.Label.SavedSuccess, Enums.NotificationType.success);
                    return RedirectToAction("SatuanKerja", "Master");
                }
                else
                {
                    var message = string.Empty;
                    foreach (var item in ModelState.Where(m => m.Value.ValidationState != Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid))
                    {
                        message += Environment.NewLine + item.Value.Errors[0].ErrorMessage;
                    }
                    Alert(message, Enums.NotificationType.error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");

                Alert(ex.Message, Enums.NotificationType.error);
            }
            return View("CreateSatuanKerja", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSatuanKerja(string id)
        {
            var existingData = _context.SatuanKerjas.FirstOrDefault(m => m.Id == id);
            if (existingData == null)
                return Ok(new { isSuccess = false, error = Constans.Label.DataNotFound });
            else
            {
                _context.SatuanKerjas.Remove(existingData);
                await _context.SaveChangesAsync();
                return Ok(new { isSuccess = true });
            }
        }
        #endregion

    }
}
