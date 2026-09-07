# US-008 — Cotizar en múltiples compañías

## Historia

**Como** usuario emisor autenticado  
**quiero** solicitar ofertas de varias compañías de seguros  
**para** comparar precio, deducible y beneficios antes de emitir una póliza.

## Prioridad y alcance

- Prioridad: Alta.
- Actor: Usuario emisor.
- Incluye: Protección de Tarjeta, Seguro Vehicular, cuatro compañías simuladas y comparación de ofertas.
- No incluye: integraciones reales, persistencia de una cotización abandonada, vigencia tarifaria ni reintentos.

## Flujo principal actual

1. El usuario completa uno de los formularios de producto.
2. El controlador entrega la misma `PolizaBE` a `CotizacionBL`.
3. La capa de datos recorre las compañías Rímac, Pacífico, MAPFRE y La Positiva.
4. Cada compañía se procesa secuencialmente con una espera artificial.
5. El sistema guarda la cotización en `Session` y muestra las cuatro ofertas.
6. La oferta de menor prima total se marca como recomendada.

## Criterios de aceptación

### Escenario 1: cotización de tarjeta

**Dado** que el usuario completa contratante, tarjeta, plan y vigencia  
**cuando** selecciona **Cotizar en compañías**  
**entonces** visualiza cuatro ofertas con prima neta, IGV, prima total, beneficios y tiempo de respuesta.

### Escenario 2: cotización vehicular

**Dado** que el usuario completa contratante, vehículo, valor comercial, uso y vigencia  
**cuando** solicita la cotización  
**entonces** cada oferta incluye además su deducible referencial.

### Escenario 3: recomendación

**Dado** que se generaron varias ofertas  
**cuando** se abre la comparación  
**entonces** la oferta con menor prima total muestra la etiqueta `Mejor precio`.

### Escenario 4: modificar datos

**Dado** que el usuario está comparando ofertas  
**cuando** selecciona **Volver y modificar datos**  
**entonces** regresa al formulario del producto correspondiente.

## Componentes involucrados

- `QOA.DEMO.Entidades/CotizacionBE.cs`
- `QOA.DEMO.Entidades/OfertaCotizacionBE.cs`
- `QOA.DEMO.Entidades/CompaniaBE.cs`
- `QOA.DEMO.LogicaNegocio/CotizacionBL.cs`
- `QOA.DEMO.AccesoDatos/CotizacionDL.cs`
- `QOA.DEMO.ClienteWeb/Controllers/EmisionController.cs`
- `QOA.DEMO.ClienteWeb/Views/Emision/CompararCotizaciones.cshtml`
- Procedimiento `SPI_COTIZACION_MULTICOMPANIA`.

## Deuda técnica intencional observable

- La cotización reutiliza `PolizaBE` y agrega aún más responsabilidades a esa entidad.
- Las compañías, factores, beneficios y textos están codificados en la capa de datos.
- Las llamadas se ejecutan una después de otra con bloqueos deliberados.
- El cálculo tarifario se duplica entre C# y SQL.
- La cotización activa se conserva completa en `Session`.
- Se recorren colecciones varias veces para encontrar el menor precio.
- SQL utiliza cursor, `WAITFOR`, UDF escalar, conversiones y búsquedas repetidas.
- No hay manejo individual de timeout, error parcial o disponibilidad por compañía.

## Punto de partida para una mejora posterior

Esta historia permite introducir adaptadores por aseguradora, paralelismo controlado, tolerancia a fallos, cache, persistencia desacoplada, tipado monetario y reglas tarifarias configurables.
