using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DOAN_QUANCAFE // Sửa tên này theo đúng Namespace project của bạn
{
    public class PhanQuyenAdmin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 1. Kiểm tra Session: Đã đăng nhập chưa?
            var user = HttpContext.Current.Session["User"];
            var role = HttpContext.Current.Session["Role"];

            // 2. Nếu chưa đăng nhập HOẶC đã đăng nhập nhưng không phải Admin
            if (user == null || role == null || role.ToString() != "Admin")
            {
                // Đá về trang Đăng nhập
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new
                    {
                        controller = "Account",
                        action = "Login",
                        area = "" // Nếu Admin nằm trong Area thì phải chỉnh lại, còn không thì để trống
                    })
                );

                // (Tùy chọn) Gửi thông báo lỗi
                // filterContext.Controller.TempData["Error"] = "Bạn không có quyền truy cập trang này!";
            }

            // Nếu đúng là Admin thì cho qua (chạy tiếp code bình thường)
            base.OnActionExecuting(filterContext);
        }
    }
}