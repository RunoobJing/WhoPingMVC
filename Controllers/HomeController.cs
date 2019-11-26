using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WhoPingMVC.Controllers {
    [Authorize]
    public class HomeController : Controller {
        // GET: Home
        [AllowAnonymous]
        public ActionResult Index() {
            return View();
        }

        public async Task<ActionResult> Init() {
            await DatabaseSession.Init();
            return new RedirectResult("Index");
        }

        [AllowAnonymous, HttpPost]
        public async Task<JsonResult> Login(string token) {
            JsonResult jr = new JsonResult();

            object staffId = Session["StaffId"];
            if (staffId != null) {
                string _ = staffId.ToString();
                if (token != _) {
                    Session["StaffId"] = null; jr.Data = new { Success = false, Message = "该用户已经登录" };
                } else {
                    jr.Data = new { Success = true };
                }
            } else {
                var staff = await DatabaseSession.GetStaff(token);
                if (staff == null) { jr.Data = new { Success = false, Message = "无效的登录名称" }; }

                Session["StaffId"] = token; jr.Data = new { Success = true };
            }
            return jr;
        }
    }
}
