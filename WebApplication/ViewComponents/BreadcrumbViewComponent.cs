using Microsoft.AspNetCore.Mvc;
using WApp.ViewModels;

namespace WApp.ViewComponents
{
    public class BreadcrumbViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<BreadcrumbItemViewModel> breadcrumbs)
        {
            return View(breadcrumbs);
        }
    }
}
