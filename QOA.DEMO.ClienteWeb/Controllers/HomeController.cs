using QOA.DEMO.Entidades;
using QOA.DEMO.LogicaNegocio;
using System.Linq;
using System.Web.Mvc;

namespace QOA.DEMO.ClienteWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["idUsuario"] == null) return RedirectToAction("Ingresar", "Login");
            var lista = new PolizaBL().consultarPolizas(new PolizaBE());
            ViewBag.Total = lista.Count; ViewBag.Emitidas = lista.Count(x => x.estado == "EMITIDA"); ViewBag.Anuladas = lista.Count(x => x.estado == "ANULADA"); ViewBag.Prima = lista.Sum(x => x.primaTotal);
            return View(lista.Take(5).ToList());
        }
    }
}
