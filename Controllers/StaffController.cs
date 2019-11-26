using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WhoPingMVC.Controllers {
    public class StaffController : Controller {
        // GET: Staff
        public ActionResult Index() {
            return View();
        }

        public async Task<JsonResult> Detail(string id) {
            var data = await DatabaseSession.GetStaff(id);

            return new JsonResult() {
                Data = data,
                JsonRequestBehavior = data !=null ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet
            };
        }
    }
}
