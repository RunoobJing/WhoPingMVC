using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WhoPingMVC.Controllers {
    //[Authorize]
    public class DepartmentController : Controller {
        [HttpGet]
        public async Task<JsonResult> Detail(string Name) => new JsonResult() { Data = await DatabaseSession.GetDepartment(Name) };

        [HttpGet]
        public async Task<JsonResult> Index() => new JsonResult() { Data = await DatabaseSession.GetDepartments() , JsonRequestBehavior = JsonRequestBehavior.AllowGet};
    }
}
