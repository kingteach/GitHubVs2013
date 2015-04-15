using SimplePaging.Models;
using System.Web.Mvc;

namespace SimplePaging.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(SimpleModel model)
        {
            model.TotalCount = 100;
            return View(model);
        }
    }
}