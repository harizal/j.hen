using Microsoft.AspNetCore.Mvc;
using static WApp.Utlis.Enums;

namespace WApp.Controllers
{
    public class BaseController : Controller
    {
        public void Alert(string message, NotificationType notificationType)
        {
            var msg = "<script language='javascript'>var span = document.createElement('span');span.innerHTML = '" + message + "';swal({\r\n    title:'" + notificationType.ToString().ToUpper() + "',content:span});</script>";
            TempData["notification"] = msg;
        }
    }
}
