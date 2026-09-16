# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Sobre el proyecto

QOA DEMO es un sistema legado de emisión de seguros (protección de tarjeta y seguro vehicular), construido **deliberadamente con deuda técnica** para una charla sobre uso progresivo de IA en ambientes corporativos. No es base para producción: antes de "corregir" un patrón raro (bucles con `Thread.Sleep`, SQL con cursores, entidades anémicas, saltos entre capas, etc.), confirma si el usuario quiere resolver esa deuda específica o si es parte intencional de la demo — ver `README.md` y `UserStories/README.md`.

## Comandos

Es una solución **.NET Framework 4.8.1 / ASP.NET MVC 5** (`QOA.DEMO.sln`), pensada para Visual Studio 2022 en Windows. No hay `dotnet` CLI, ni scripts de build/lint/test, ni proyecto de pruebas ni CI configurada en el repo.

- **Compilar/ejecutar**: abrir `QOA.DEMO.sln` en Visual Studio, restaurar paquetes NuGet, establecer `QOA.DEMO.ClienteWeb` como proyecto de inicio y ejecutar con IIS Express.
- **Modo de datos**: por defecto usa datos simulados en memoria (`UsarDatosSimulados` en `Web.config`, sección `appSettings`), así que no requiere SQL Server para correr la demo.
  - Para usar SQL Server real: ejecutar `BaseDatos/QOA_DEMO.sql` (elimina y recrea tablas; nunca contra una base compartida/productiva), ajustar la cadena `QoaDemo` en `connectionStrings` de `Web.config`, y poner `UsarDatosSimulados` en `false`.
- **Login de demo**: `admin` / `admin`.

## Arquitectura

Cuatro proyectos/capas dentro de la solución, cada uno con su propio `.csproj`:

1. **`QOA.DEMO.Entidades`** — entidades anémicas sufijo `BE` (p. ej. `PolizaBE`, `CompaniaBE`, `CotizacionBE`), compartidas por todas las capas. Incluye `MensajeResultado` (con `CODIGO`, `MENSAJE`, `ID`, `DATA`, `EXITO`), el tipo de retorno estándar para operaciones de escritura/negocio.
2. **`QOA.DEMO.AccesoDatos`** — clases sufijo `DL`. Heredan de `ConexionDL`, que expone `conexion` (connection string `QoaDemo`) y `UsarDatosSimulados()`. Cada método de una `DL` normalmente ramifica: si `UsarDatosSimulados()` es verdadero, opera sobre listas en memoria (`DatosDemo.cs`); si no, ejecuta ADO.NET puro contra procedimientos almacenados (`SqlConnection`/`SqlCommand`, sin ORM). Ambas rutas deben mantenerse sincronizadas al modificar comportamiento.
3. **`QOA.DEMO.LogicaNegocio`** — clases sufijo `BL` (`PolizaBL`, `CotizacionBL`, `UsuarioBL`). Validan datos de entrada y orquestan llamadas a la `DL` correspondiente instanciándola con `new` (no hay inyección de dependencias ni interfaces).
4. **`QOA.DEMO.ClienteWeb`** — aplicación ASP.NET MVC con vistas Razor, jQuery y Bootstrap. Los controladores llaman a las `BL`, aunque hay excepciones deliberadas donde saltan directo a la `DL` (comentadas en el código, p. ej. `PolizaController.Detalle`). La ruta por defecto entra por `Login/Ingresar` (`App_Start/RouteConfig.cs`); las acciones protegidas verifican `Session["idUsuario"]` manualmente al inicio de cada método (no hay atributo de autorización centralizado).

### Convención de manejo de errores

- Entrada inválida / regla de negocio no cumplida en una `BL`: lanzar `throw new Exception("mensaje")` antes de delegar en la `DL` (ver `CotizacionBL.cotizarPoliza`).
- Todo bloque `try/catch` que reenvía la excepción debe usar `throw;`, nunca `throw ex;`, para conservar el stack trace original.
- Resultado esperado de una operación de escritura (éxito o fallo de negocio, p. ej. "póliza ya anulada"): devolver `MensajeResultado` con `EXITO`/`CODIGO`/`MENSAJE`, no lanzar excepción.

### Flujo de negocio principal

Cotización multi-compañía → selección de oferta → emisión de póliza: `CotizacionBL.cotizarPoliza` arma ofertas de varias `CompaniaBE`; `CotizacionBL.emitirCotizacion` selecciona la oferta elegida y delega en `PolizaBL.registrarPoliza` para emitir la póliza. Ver `UserStories/US-008-cotizacion-multicompania.md` y `US-009-seleccionar-oferta-y-emitir.md` para el detalle funcional.

Este proyecto atiende a varias aseguradoras (Rimac, Pacífico, MAPFRE, La Positiva) desde una misma plataforma; cada una tiene su propio código, RUC, color de marca y factor de tarificación, definidos en CotizacionDL.ObtenerCompaniasEnMemoria().
