using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WhoPingMVC.Controllers {
    [Authorize]
    public class ActivityController : Controller {
        public async Task<JsonResult> Detail(string Id) {
            return new JsonResult() { Data = await DatabaseSession.GetActivity(Id) };
        }

        [HttpPost]
        public async Task<JsonResult> Save(string Id) {
            var activity = await DatabaseSession.GetActivity(Id);
            var form = HttpContext.Request.Form;
            //需要判断是否是可以Save/Commit
            string staffId = Session["StaffId"].ToString();

            if (string.IsNullOrEmpty(staffId)) {
                RedirectToAction("/Home/Login");
            }

            if(staffId != activity.SourceStaffId) {
                return new JsonResult() {
                    Data = new { Success = false, Message = "不能保存非本部门/本人的评分结果." }
                };
            }

            int Profession = int.Parse(form[nameof(Profession)]),
                Duty = int.Parse(form[nameof(Duty)]),
                Cooperation = int.Parse(form[nameof(Cooperation)]),
                Result = int.Parse(form[nameof(Result)]);

            activity.Profession = Profession;
            activity.Duty = Duty;
            activity.Cooperation = Cooperation;
            activity.Result = Result;

            try {
                var response = await DatabaseSession.SaveActivity(activity);
                return new JsonResult() {
                    Data = response,
                };

            } catch (Exception ex) {
                return new JsonResult() {
                    Data = new { Success = false, ex.Message }
                };
            }
        }

        [HttpPost]
        public async Task<JsonResult> Commit(string Id) {
            var activity = await DatabaseSession.GetActivity(Id);
            var form = HttpContext.Request.Form;
            //需要判断是否是可以Save/Commit
            string staffId = Session["StaffId"].ToString();

            if (string.IsNullOrEmpty(staffId)) {
                RedirectToAction("/Home/Login");
            }

            if (staffId != activity.SourceStaffId) {
                return new JsonResult() {
                    Data = new { Success = false, Message = "不能保存非本部门/本人的评分结果." }
                };
            }

            int Profession = int.Parse(form[nameof(Profession)]),
                Duty = int.Parse(form[nameof(Duty)]),
                Cooperation = int.Parse(form[nameof(Cooperation)]),
                Result = int.Parse(form[nameof(Result)]);

            activity.Profession = Profession;
            activity.Duty = Duty;
            activity.Cooperation = Cooperation;
            activity.Result = Result;

            try {
                var response = await DatabaseSession.CommitActivity(activity);
                return new JsonResult() {
                    Data = response,
                };
            } catch(Exception ex) {
                return new JsonResult() {
                    Data = new { Success = false, ex.Message }
                };
            }
        }
    }
}
