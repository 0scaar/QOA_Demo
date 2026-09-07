using QOA.DEMO.AccesoDatos;
using QOA.DEMO.Entidades;
using QOA.DEMO.LogicaNegocio;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace QOA.DEMO.ClienteWeb.Controllers
{
    public class PolizaController : Controller
    {
        public ActionResult Index(string filtroGeneral)
        {
            if (Session["idUsuario"] == null) return RedirectToAction("Ingresar", "Login");
            PolizaBE filtro = new PolizaBE(); filtro.filtroGeneral = filtroGeneral;
            List<PolizaBE> lista = new PolizaBL().consultarPolizas(filtro); ViewBag.Filtro = filtroGeneral;
            return View(lista);
        }

        public ActionResult Detalle(long id)
        {
            if (Session["idUsuario"] == null) return RedirectToAction("Ingresar", "Login");
            // Salto deliberado de la capa de negocio para mantener la inconsistencia del original.
            PolizaBE p = new PolizaDL().consultarDetalle(id);
            if (p == null) return HttpNotFound();
            return View(p);
        }

        [HttpPost]
        public ActionResult Anular(long idPoliza, string motivoAnulacion)
        {
            if (Session["idUsuario"] == null) return RedirectToAction("Ingresar", "Login");
            try { UsuarioBE u = Session["idUsuario"] as UsuarioBE; MensajeResultado r = new PolizaBL().anularPoliza(idPoliza, motivoAnulacion, u.usuario); TempData["Mensaje"] = r.MENSAJE; return RedirectToAction("Detalle", new { id = idPoliza }); }
            catch (Exception ex) { TempData["Mensaje"] = ex.Message; return RedirectToAction("Detalle", new { id = idPoliza }); }
        }
    }
}
