# US-001 — Iniciar y cerrar sesión

## Historia

**Como** usuario emisor de seguros  
**quiero** ingresar al sistema con mis credenciales y cerrar mi sesión  
**para** acceder a las funciones de emisión y consulta de pólizas.

## Prioridad y alcance

- Prioridad: Alta.
- Actor: Usuario emisor.
- Incluye: pantalla de ingreso, validación de credenciales, creación de sesión, protección básica de pantallas y cierre de sesión.
- No incluye: recuperación de contraseña, MFA, bloqueo por intentos ni administración de usuarios.

## Flujo principal actual

1. El usuario abre la aplicación y visualiza la pantalla de ingreso.
2. Ingresa usuario y clave.
3. El controlador consulta `UsuarioBL`.
4. `UsuarioBL` instancia `UsuarioDL` y valida las credenciales.
5. Si son correctas, la aplicación guarda el usuario, nombre y perfil en `Session`.
6. El usuario es enviado al Home.
7. Al seleccionar **Salir**, la sesión se limpia y vuelve a la pantalla de ingreso.

## Criterios de aceptación

### Escenario 1: ingreso correcto

**Dado** que existe un usuario activo con usuario `admin` y clave `admin`  
**cuando** el usuario envía esas credenciales  
**entonces** el sistema crea la sesión y muestra el panel de inicio.

### Escenario 2: credenciales incorrectas

**Dado** que el usuario se encuentra en la pantalla de ingreso  
**cuando** envía un usuario inexistente o una clave incorrecta  
**entonces** permanece en la pantalla y ve el mensaje `Usuario o clave incorrecta`.

### Escenario 3: acceso sin sesión

**Dado** que no existe una sesión de usuario  
**cuando** se intenta abrir Home, emisión, consulta, detalle o anulación  
**entonces** el sistema redirige a la pantalla de ingreso.

### Escenario 4: cerrar sesión

**Dado** que el usuario inició sesión  
**cuando** selecciona **Salir**  
**entonces** la aplicación limpia y abandona la sesión y muestra nuevamente el ingreso.

## Datos para la demostración

| Campo | Valor |
|---|---|
| Usuario | `admin` |
| Clave | `admin` |
| Perfil | `EMISOR` |

## Componentes involucrados

- `QOA.DEMO.ClienteWeb/Controllers/LoginController.cs`
- `QOA.DEMO.ClienteWeb/Views/Login/Ingresar.cshtml`
- `QOA.DEMO.LogicaNegocio/UsuarioBL.cs`
- `QOA.DEMO.AccesoDatos/UsuarioDL.cs`
- `QOA.DEMO.Entidades/UsuarioBE.cs`
- Procedimiento `SPS_SEG_LOGIN`.

## Deuda técnica intencional observable

- Credencial de demostración almacenada y comparada en texto plano.
- Uso directo de `Session` y verificaciones repetidas en cada acción.
- Ausencia de autorización por roles, MFA y límite de intentos.
- Mensajes de excepción enviados a la vista.
- Dependencias creadas con `new` y `try/catch` con `throw ex`.
- Búsqueda lineal con espera por usuario en modo simulado.
- Cursor, `WAITFOR`, funciones sobre la columna y `SELECT *` en SQL.
- El formulario no utiliza token antifalsificación.

## Punto de partida para una mejora posterior

La historia permite trabajar autenticación, autorización, manejo de secretos, sesiones, pruebas automatizadas, observabilidad y rendimiento del procedimiento de login.
