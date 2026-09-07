using QOA.DEMO.Entidades;
using QOA.DEMO.LogicaNegocio;
using System;
using System.Web.Mvc;

namespace QOA.DEMO.ClienteWeb.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Ingresar()
        {
            if (Session["idUsuario"] != null) return RedirectToAction("Index", "Home");
            return View(new UsuarioBE());
        }

        [HttpPost]
        public ActionResult LoginUsuario(UsuarioBE mUsuario, FormCollection formulario)
        {
            try
            {
                UsuarioBE usuario = new UsuarioBL().loginUsuario(mUsuario);
                if (usuario == null)
                {
                    ViewBag.Mensaje = "Usuario o clave incorrecta";
                    return View("Ingresar", mUsuario);
                }
                Session["idUsuario"] = usuario;
                Session["nombreUsuario"] = usuario.nombreCompleto;
                Session["perfil"] = usuario.perfil;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
                return View("Ingresar", mUsuario);
            }
        }

        public ActionResult Salir()
        {
            Session["idUsuario"] = null; Session.Clear(); Session.Abandon();
            return RedirectToAction("Ingresar");
        }
    }
}
