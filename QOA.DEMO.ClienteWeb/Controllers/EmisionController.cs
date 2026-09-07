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
            Session.Remove("cotizacionActual");
            PolizaBE p = Nuevo("TARJETA"); p.planTarjeta = "PLAN 2"; p.lineaCredito = 10000;
            return View(p);
        }

        [HttpPost]
        public ActionResult ProteccionTarjeta(PolizaBE objPoliza, FormCollection formulario)
        {
            if (Session["idUsuario"] == null) return RedirectToAction("Ingresar", "Login");
            try { objPoliza.tipoProducto = "TARJETA"; objPoliza.usuarioRegistro = ((UsuarioBE)Session["idUsuario"]).usuario; CotizacionBE c = new CotizacionBL().cotizarPoliza(objPoliza); Session["cotizacionActual"] = c; return RedirectToAction("Comparar"); }
            catch (Exception ex) { ViewBag.Mensaje = ex.Message; return View(objPoliza); }
        }

        public ActionResult Vehicular()
        {
            if (Session["idUsuario"] == null) return RedirectToAction("Ingresar", "Login");
            Session.Remove("cotizacionActual");
            PolizaBE p = Nuevo("VEHICULAR"); p.anioFabricacion = DateTime.Today.Year; p.valorComercial = 60000; p.usoVehiculo = "PARTICULAR";
            return View(p);
        }

        [HttpPost]
        public ActionResult Vehicular(PolizaBE objPoliza)
        {
            if (Session["idUsuario"] == null) return RedirectToAction("Ingresar", "Login");
            try { objPoliza.tipoProducto = "VEHICULAR"; objPoliza.usuarioRegistro = ((UsuarioBE)Session["idUsuario"]).usuario; CotizacionBE c = new CotizacionBL().cotizarPoliza(objPoliza); Session["cotizacionActual"] = c; return RedirectToAction("Comparar"); }
            catch (Exception ex) { ViewBag.Mensaje = ex.Message; return View(objPoliza); }
        }

        public ActionResult Comparar()
        {
            if (Session["idUsuario"] == null) return RedirectToAction("Ingresar", "Login");
            CotizacionBE cotizacion = Session["cotizacionActual"] as CotizacionBE;
            if (cotizacion == null) return RedirectToAction("Index", "Home");
            return View("CompararCotizaciones", cotizacion);
        }

        [HttpPost]
        public ActionResult EmitirCotizacion(long idOferta)
        {
            if (Session["idUsuario"] == null) return RedirectToAction("Ingresar", "Login");
            CotizacionBE cotizacion = Session["cotizacionActual"] as CotizacionBE;
            if (cotizacion == null) { TempData["Mensaje"] = "La cotizacion ya no se encuentra disponible"; return RedirectToAction("Index", "Home"); }
            try
            {
                UsuarioBE usuario = (UsuarioBE)Session["idUsuario"];
                MensajeResultado r = new CotizacionBL().emitirCotizacion(cotizacion, idOferta, usuario.usuario);
                if (!r.EXITO) { ViewBag.Mensaje = r.MENSAJE; return View("CompararCotizaciones", cotizacion); }
                Session.Remove("cotizacionActual"); TempData["Mensaje"] = r.MENSAJE;
                return RedirectToAction("Detalle", "Poliza", new { id = r.ID });
            }
            catch (Exception ex) { ViewBag.Mensaje = ex.Message; return View("CompararCotizaciones", cotizacion); }
        }

        private PolizaBE Nuevo(string tipo)
        {
            PolizaBE p = new PolizaBE(); p.tipoProducto = tipo; p.fecIniVigencia = DateTime.Today.ToString("yyyy-MM-dd"); p.fecFinVigencia = DateTime.Today.AddYears(1).ToString("yyyy-MM-dd"); p.contratante.tipoDocumento = "DNI"; return p;
        }
    }
}
