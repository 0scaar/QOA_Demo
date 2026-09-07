# QOA DEMO

Proyecto pequeño para una charla sobre el uso progresivo de IA en ambientes corporativos. Representa un sistema legado de emisión de seguros y fue construido **deliberadamente con deuda técnica** para mejorarlo durante sesiones posteriores.

## Tecnología replicada

- .NET Framework 4.8.1.
- ASP.NET MVC 5.2.9 con vistas Razor.
- jQuery 3.7.1.
- Bootstrap 3.3.5, usando los mismos archivos del proyecto QOA.
- SQL Server y ADO.NET con procedimientos almacenados.

## Arquitectura

La solución `QOA.DEMO.sln` mantiene cuatro capas:

1. `QOA.DEMO.Entidades`: entidades anémicas `BE`.
2. `QOA.DEMO.AccesoDatos`: clases `DL`, ADO.NET y datos simulados.
3. `QOA.DEMO.LogicaNegocio`: clases `BL` que delegan en acceso a datos.
4. `QOA.DEMO.ClienteWeb`: aplicación ASP.NET MVC.

## Funcionalidad

- Login básico (`admin` / `admin`).
- Home con indicadores y últimas emisiones.
- Emisión de Protección de Tarjeta.
- Emisión de Seguro Vehicular.
- Consulta de pólizas por número, DNI o contratante.
- Detalle de póliza.
- Anulación de póliza.

## Ejecución

1. Abrir `QOA.DEMO.sln` en Visual Studio 2022 para Windows.
2. Restaurar los paquetes NuGet si Visual Studio lo solicita.
3. Establecer `QOA.DEMO.ClienteWeb` como proyecto de inicio.
4. Ejecutar con IIS Express.

La aplicación usa datos en memoria por defecto (`UsarDatosSimulados=true` en `Web.config`), por lo que no necesita base de datos para la charla. Para utilizar SQL Server:

1. Ejecutar `BaseDatos/QOA_DEMO.sql` en una instancia de desarrollo desechable.
2. Ajustar la cadena `QoaDemo` de `Web.config`.
3. Cambiar `UsarDatosSimulados` a `false`.

> El script elimina y vuelve a crear sus tablas. No debe ejecutarse en una base compartida o productiva.

## Deuda técnica intencional

No usar este código como base de producción. Entre los problemas preparados para la demostración están:

- Entidades anémicas y sobredimensionadas, con tipos débiles y datos de UI/ADO.NET mezclados.
- Dependencias instanciadas con `new`, falta de interfaces y saltos entre capas.
- Manejo redundante de excepciones con `throw ex`.
- Estado global mutable, credenciales de demostración en texto plano y controles de seguridad incompletos.
- Operaciones síncronas, esperas por fila, búsquedas lineales, copias innecesarias y ausencia de paginación.
- SQL con cursores, `WHILE`, tablas variables, UDF escalares, subconsultas repetidas y filtros no SARGables.
- Índices recomendados presentes al final del SQL, comentados para no aplicar todavía la mejora.

Las demoras son pequeñas y controladas para que los síntomas puedan observarse sin bloquear por completo la demo.
