using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WApp.Datas;
using WApp.Helpers;
using WApp.Models;
using WApp.Utlis;
using WApp.ViewModels;
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

        public IActionResult Search()
        {
            PegawaiViewModel model = new()
            {
                ListJenisKelamin =
                [
                    new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = EnumHelper.GetDescription(Enums.JenisKelamin.L), Value = ((int)Enums.JenisKelamin.L).ToString() },
                    new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = EnumHelper.GetDescription(Enums.JenisKelamin.P), Value = ((int)Enums.JenisKelamin.P).ToString() }
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
                ]
            };
            return View(model);
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
