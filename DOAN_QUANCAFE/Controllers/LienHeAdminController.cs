using System.Linq;
using System.Web.Mvc;

namespace DOAN_QUANCAFE.Controllers
{
    public class LienHeAdminController : Controller
    {
        private QL_QuanCafeEntities db = new QL_QuanCafeEntities();

        // GET: LienHeAdmin
        public ActionResult Index()
        {
            var dsLienHe = db.LienHes
                             .OrderByDescending(lh => lh.NgayGui)
                             .ToList();
            return View(dsLienHe);
        }
    }
}
