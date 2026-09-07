using System;
using System.IO;
using System.Web.Mvc;

namespace QOA.DEMO.ClienteWeb.Controllers
{
    // IIS Express no consigue servir archivos estaticos desde C:\Mac (Parallels Shared Folders).
    // Este controlador los lee en memoria y los entrega mediante el pipeline administrado de MVC.
    public class AssetsController : Controller
    {
        public ActionResult BootstrapStyle()
        {
            return EnviarArchivo("~/Content/BootStrap/css/bootstrap.css", "text/css");
        }

        public ActionResult BootstrapTheme()
        {
            return EnviarArchivo("~/Content/BootStrap/css/bootstrap-theme.min.css", "text/css");
        }

        public ActionResult Estilos()
        {
            return EnviarArchivo("~/Content/css/estilos.css", "text/css");
        }

        public ActionResult JQuery()
        {
            return EnviarArchivo("~/Scripts/jquery-3.7.1/jquery-3.7.1.min.js", "application/javascript");
        }

        public ActionResult BootstrapScript()
        {
            return EnviarArchivo("~/Content/BootStrap/js/bootstrap.min.js", "application/javascript");
        }

        public ActionResult Font(string archivo)
        {
            string[] permitidos =
            {
                "glyphicons-halflings-regular.eot",
                "glyphicons-halflings-regular.svg",
                "glyphicons-halflings-regular.ttf",
                "glyphicons-halflings-regular.woff",
                "glyphicons-halflings-regular.woff2"
            };

            if (Array.IndexOf(permitidos, archivo) < 0) return HttpNotFound();

            string extension = Path.GetExtension(archivo).ToLower();
            string tipo = extension == ".eot" ? "application/vnd.ms-fontobject" :
                          extension == ".svg" ? "image/svg+xml" :
                          extension == ".ttf" ? "font/ttf" :
                          extension == ".woff2" ? "font/woff2" : "font/woff";

            return EnviarArchivo("~/Content/BootStrap/fonts/" + archivo, tipo);
        }

        public ActionResult Favicon()
        {
            string svg = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 64 64'><rect width='64' height='64' rx='15' fill='#1474f7'/><text x='32' y='43' text-anchor='middle' font-family='Arial' font-size='34' font-weight='700' fill='white'>Q</text></svg>";
            return Content(svg, "image/svg+xml");
        }

        private ActionResult EnviarArchivo(string ruta, string tipoContenido)
        {
            string rutaFisica = Server.MapPath(ruta);
            if (!System.IO.File.Exists(rutaFisica)) return HttpNotFound();

            // La lectura completa en cada solicitud es intencionalmente simple para esta demo legada.
            byte[] contenido = System.IO.File.ReadAllBytes(rutaFisica);
            return File(contenido, tipoContenido);
        }
    }
}
