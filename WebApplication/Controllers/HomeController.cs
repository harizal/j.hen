using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using WApp.Datas;
using WApp.Helpers;
using WApp.Models;
using WApp.Utlis;
using WApp.ViewModels;
using WApp.ViewModels.Parameters;
using static WApp.Utlis.Enums;

namespace WApp.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDataContext _context;

        public HomeController(ILogger<HomeController> logger, AppDataContext dataContext)
        {
            _logger = logger;
            _context = dataContext;
        }

        public IActionResult Index()
        {
            ViewBag.Breadcrumbs = new List<BreadcrumbItemViewModel>
            {
                new() { Title = "Dashboard", Url = Url.Action("Index", "Home") },
            };

            var viewModel = new DashboardViewModel();

            var queryK2 = (from pegawai in _context.Pegawais.Where(m => m.Type == PegawaiType.K2)
                           join pen in _context.Pendidikans on pegawai.PendidikanID equals pen.Id into pendidikan
                           from pen in pendidikan.DefaultIfEmpty()
                           select new PegawaiViewModel
                           {
                               Id = pegawai.Id,
                               JenisKelaminText = pegawai.JenisKelamin.HasValue ? EnumHelper.GetDescription(pegawai.JenisKelamin) : "-",
                               PendidikanText = pen.Name,
                           }).ToList();

            viewModel.TotalK2 = queryK2.Count();
            viewModel.TotalK2Aktif = queryK2.Count(m => m.IsActive);

            var totalJenisKelamin = queryK2.GroupBy(m => m.JenisKelaminText)
                .Select(m => new { Text = m.Key, Value = m.Count() });
            foreach (var item in totalJenisKelamin)
            {
                viewModel.K2JenisKelamin.Add(new Item { Text = item?.Text ?? "-", Total = item?.Value ?? 0 });
            }

            var totalPendidikan = queryK2.GroupBy(m => m.PendidikanText)
                .Select(m => new { Text = m.Key, Value = m.Count() });
            foreach (var item in totalPendidikan)
            {
                viewModel.K2Pendidikan.Add(new Item { Text = item?.Text ?? "-", Total = item?.Value ?? 0 });
            }

            var queryPHL = (from pegawai in _context.Pegawais.Where(m => m.Type == PegawaiType.PHL)
                            join pen in _context.Pendidikans on pegawai.PendidikanID equals pen.Id into pendidikan
                            from pen in pendidikan.DefaultIfEmpty()
                            select new PegawaiViewModel
                            {
                                Id = pegawai.Id,
                                JenisKelaminText = pegawai.JenisKelamin.HasValue ? EnumHelper.GetDescription(pegawai.JenisKelamin) : "-",
                                PendidikanText = pen.Name,
                            }).ToList();

            viewModel.TotalPHL = queryPHL.Count();
            viewModel.TotalPHLAktif = queryPHL.Count(m => m.IsActive);

            totalJenisKelamin = queryPHL.GroupBy(m => m.JenisKelaminText)
                .Select(m => new { Text = m.Key, Value = m.Count() });
            foreach (var item in totalJenisKelamin)
            {
                viewModel.PHLJenisKelamin.Add(new Item { Text = item?.Text ?? "-", Total = item?.Value ?? 0 });
            }

            totalPendidikan = queryPHL.GroupBy(m => m.PendidikanText)
                .Select(m => new { Text = m.Key, Value = m.Count() });
            foreach (var item in totalPendidikan)
            {
                viewModel.PHLPendidikan.Add(new Item { Text = item?.Text ?? "-", Total = item?.Value ?? 0 });
            }


            return View(viewModel);
        }

        public IActionResult Search(string name, string nik)
        {
            PegawaiViewModel model = new()
            {
                ListJenisKelamin =
                [
                    new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = EnumHelper.GetDescription(Enums.JenisKelamin.L), Value = (Enums.JenisKelamin.L).ToString() },
                    new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = EnumHelper.GetDescription(Enums.JenisKelamin.P), Value = (Enums.JenisKelamin.P).ToString() }
                ],
                ListStatusPerkawinan =
                [
                    .. _context.Statuses.Where(m => m.IsActive).Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = m.Name,
                        Value = m.Id
                    }),
                ],
                ListPendidikan =
                [
                    .. _context.Pendidikans.Where(m => m.IsActive).Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = m.Name,
                        Value = m.Id
                    }),
                ],
                ListProdi =
                [
                    .. _context.Prodis.Where(m => m.IsActive).Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = m.Name,
                        Value = m.Id
                    }),
                ],
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
                ],

                Name = string.IsNullOrEmpty(name) ? string.Empty : name,
                NIK = string.IsNullOrEmpty(nik) ? string.Empty : nik,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetDatas(BaseDTParameters param)
        {
            var results = new List<PegawaiViewModel>();

            if (param.AdditionalValues != null && param.AdditionalValues.Any())
            {
                var filters = param.AdditionalValues.ToList();
                try
                {
                    results = await (from pegawai in _context.Pegawais.Where(m =>
                        m.NIK.ToLower().Contains((filters[0] ?? string.Empty).ToLower()) &&
                        m.Name.ToLower().Contains((filters[1] ?? string.Empty).ToLower()))
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
                                         Nomor = pegawai.Type != PegawaiType.PHL ? pegawai.NIK : string.Empty,
                                         NIK = pegawai.Type == PegawaiType.PHL ? pegawai.NIK : string.Empty,
                                         JenisKelaminText = EnumHelper.GetDescription(pegawai.JenisKelamin),
                                         JenisKelamin = pegawai.JenisKelamin,
                                         PendidikanText = pen.Name ?? string.Empty,
                                         PendidikanID = pen.Id ?? string.Empty,
                                         ProdiText = prod.Name ?? string.Empty,
                                         ProdiID = prod.Id ?? string.Empty,
                                         SatuanKerjaText = satuanKerja.Name ?? string.Empty,
                                         SatuanKerjaID = satuanKerja.Id ?? string.Empty,
                                         UnitKerjaText = unitKerja.Name ?? string.Empty,
                                         UnitKerjaID = unitKerja.Id ?? string.Empty,
                                         TanggalAwal = ConverterHelper.DateTimeToString(pegawai.TanggalAwal),
                                         TanggalAkhir = ConverterHelper.DateTimeToString(pegawai.TanggalAkhir),
                                         Keterangan = pegawai.Keterangan,
                                         IsActive = pegawai.IsActive,
                                         Name = pegawai.Name,
                                         CreatedBy = pegawai.CreatedBy,
                                         CreatedDate = pegawai.CreatedDate,
                                         UpdatedBy = pegawai.UpdatedBy,
                                         UpdatedDate = pegawai.UpdatedDate,
                                         TypeText = EnumHelper.GetDescription(pegawai.Type),
                                         Type = pegawai.Type
                                     }).OrderByDescending(m => m.IsActive).ThenBy(m => m.Name).ToListAsync();

                    if (filters.Count() > 1 && filters[2] != null && filters[2] != "null")
                    {
                        var jenisKelamin = filters[2]?.Split(',');
                        if (jenisKelamin.Any())
                            results = results.Where(m => jenisKelamin.Contains(m.JenisKelamin.ToString())).ToList();
                    }

                    if (filters.Count() > 2 && filters[3] != null && filters[3] != "null")
                    {
                        var pendidikan = filters[3]?.Split(',');
                        if (pendidikan.Any())
                            results = results.Where(m => pendidikan.Contains(m.PendidikanID.ToString())).ToList();
                    }

                    if (filters.Count() > 3 && filters[4] != null && filters[4] != "null")
                    {
                        var prodi = filters[4]?.Split(',');
                        if (prodi.Any())
                            results = results.Where(m => prodi.Contains(m.ProdiID.ToString())).ToList();
                    }

                    if (filters.Count() > 4 && filters[5] != null && filters[5] != "null")
                    {
                        var satuanKerja = filters[5]?.Split(',');
                        if (satuanKerja.Any())
                            results = results.Where(m => satuanKerja.Contains(m.SatuanKerjaID.ToString())).ToList();
                    }

                    if (filters.Count() > 5 && filters[6] != null && filters[6] != "null")
                    {
                        var unitKerja = filters[6]?.Split(',');
                        if (unitKerja.Any())
                            results = results.Where(m => unitKerja.Contains(m.UnitKerjaID.ToString())).ToList();
                    }

                    if (filters.Count() > 6 && filters[7] != null && filters[7] != "null")
                    {
                        var type = filters[7]?.Split(',');
                        if (type.Any())
                            results = results.Where(m => type.Contains(m.Type.ToString())).ToList();
                    }

                    if (filters.Count() > 7 && filters[8] != null && filters[8] != "null")
                    {
                        var status = filters[8]?.Split(',');
                        if (status.Any())
                            results = results.Where(m => status.Contains(m.IsActive ? "1" : "0")).ToList();
                    }
                }
                catch (Exception ex)
                {
                    var asdsd = ex;
                }
            }

            return new JsonResult(DataTablePagedHelper.GetDatatablePaged(results, param));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
