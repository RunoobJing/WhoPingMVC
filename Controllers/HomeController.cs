using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WhoPingMVC.Controllers {
    public class HomeController : Controller {
        // GET: Home
        public ActionResult Index() {
            return View();
        }

        public async Task<ActionResult> Init() {
            await DatabaseSession.Init();
            return new RedirectResult("Index");
        }
    }
}
