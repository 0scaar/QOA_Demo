using QOA.DEMO.Entidades;
using QOA.DEMO.LogicaNegocio;
using System;
using System.Web.Mvc;

namespace QOA.DEMO.ClienteWeb.Controllers
{
    public class EmisionController : Controller
    {
        public ActionResult ProteccionTarjeta()
        {
            if (Session["idUsuario"] == null) return RedirectToAction("Ingresar", "Login");
            PolizaBE p = Nuevo("TARJETA"); p.planTarjeta = "PLAN 2"; p.lineaCredito = 10000;
            return View(p);
        }

        [HttpPost]
        public ActionResult ProteccionTarjeta(PolizaBE objPoliza, FormCollection formulario)
        {
            if (Session["idUsuario"] == null) return RedirectToAction("Ingresar", "Login");
            try { objPoliza.tipoProducto = "TARJETA"; objPoliza.usuarioRegistro = ((UsuarioBE)Session["idUsuario"]).usuario; MensajeResultado r = new PolizaBL().registrarPoliza(objPoliza); TempData["Mensaje"] = r.MENSAJE; return RedirectToAction("Detalle", "Poliza", new { id = r.ID }); }
            catch (Exception ex) { ViewBag.Mensaje = ex.Message; return View(objPoliza); }
        }

        public ActionResult Vehicular()
        {
            if (Session["idUsuario"] == null) return RedirectToAction("Ingresar", "Login");
            PolizaBE p = Nuevo("VEHICULAR"); p.anioFabricacion = DateTime.Today.Year; p.valorComercial = 60000; p.usoVehiculo = "PARTICULAR";
            return View(p);
        }

        [HttpPost]
        public ActionResult Vehicular(PolizaBE objPoliza)
        {
            if (Session["idUsuario"] == null) return RedirectToAction("Ingresar", "Login");
            try { objPoliza.tipoProducto = "VEHICULAR"; objPoliza.usuarioRegistro = ((UsuarioBE)Session["idUsuario"]).usuario; MensajeResultado r = new PolizaBL().registrarPoliza(objPoliza); TempData["Mensaje"] = r.MENSAJE; return RedirectToAction("Detalle", "Poliza", new { id = r.ID }); }
            catch (Exception ex) { ViewBag.Mensaje = ex.Message; return View(objPoliza); }
        }

        private PolizaBE Nuevo(string tipo)
        {
            PolizaBE p = new PolizaBE(); p.tipoProducto = tipo; p.fecIniVigencia = DateTime.Today.ToString("yyyy-MM-dd"); p.fecFinVigencia = DateTime.Today.AddYears(1).ToString("yyyy-MM-dd"); p.contratante.tipoDocumento = "DNI"; return p;
        }
    }
}
