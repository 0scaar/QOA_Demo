# US-010 — Elegir el beneficio de Auto de Reemplazo al cotizar un Seguro Vehicular

## Historia

**Como** usuario emisor autenticado
**quiero** marcar el beneficio opcional "Auto de Reemplazo" al completar el formulario de Seguro Vehicular
**para** que las ofertas que se cotizan a las cinco aseguradoras lo incluyan con su costo adicional.

## Prioridad y alcance

- Prioridad: Alta.
- Actor: Usuario emisor.
- Precondición: el usuario está cotizando un producto `VEHICULAR`.
- Incluye: casilla/opción "Auto de Reemplazo" en el formulario vehicular, envío del indicador de selección hacia la cotización multicompañía.
- No incluye: contratar el beneficio de forma independiente a una póliza vehicular, condiciones de uso del auto prestado (días de cobertura, categoría del vehículo de reemplazo), ni integración real con las aseguradoras.
- Fuera de alcance explícito: el formulario de Protección de Tarjeta no debe mostrar ni verse afectado por este beneficio (ver Escenario 4).

## Reglas propuestas

- El beneficio es opcional y está desmarcado por defecto.
- Solo se ofrece cuando `tipoProducto == "VEHICULAR"`.
- La selección (`true`/`false`) viaja junto con los demás datos del vehículo hacia `CotizacionBL.cotizarPoliza`, de modo que las cinco aseguradoras reciban el mismo indicador.
- No se cotiza el beneficio como una póliza ni como un producto independiente: es un atributo de la cotización vehicular existente.

## Criterios de aceptación

### Escenario 1: opción visible en el formulario vehicular

**Dado** que el usuario abrió **Cotizar y emitir > Vehicular**
**cuando** visualiza el formulario
**entonces** existe una opción "Auto de Reemplazo" que puede marcar o dejar sin marcar antes de cotizar.

### Escenario 2: cotización sin el beneficio (comportamiento por defecto)

**Dado** que el usuario completa el formulario vehicular sin marcar "Auto de Reemplazo"
**cuando** solicita la cotización
**entonces** las ofertas se generan exactamente como hoy, sin costo adicional ni mención al beneficio.

### Escenario 3: cotización con el beneficio marcado

**Dado** que el usuario completa el formulario vehicular y marca "Auto de Reemplazo"
**cuando** solicita la cotización
**entonces** el indicador de selección llega a `CotizacionBL.cotizarPoliza` y se aplica de forma idéntica a las cinco aseguradoras (Rímac, Pacífico, MAPFRE, La Positiva, Seguros Andina).

### Escenario 4: sin efecto sobre Protección de Tarjeta

**Dado** que el usuario abre **Cotizar y emitir > Protección de Tarjeta**
**cuando** visualiza el formulario y cotiza
**entonces** no aparece la opción "Auto de Reemplazo" en ningún momento y el cálculo de la prima de tarjeta no cambia respecto al comportamiento actual.

## Componentes probablemente involucrados

- `QOA.DEMO.ClienteWeb/Views/Emision/Vehicular.cshtml`
- `QOA.DEMO.Entidades/PolizaBE.cs` (nuevo campo para transportar la selección, junto a los datos ya mezclados de vehicular)
- `QOA.DEMO.LogicaNegocio/CotizacionBL.cs`
- `QOA.DEMO.AccesoDatos/CotizacionDL.cs`

## Nota sobre deuda técnica existente

El texto de beneficios de Rímac para vehicular ya incluye la frase "Auto de reemplazo" (`CotizacionDL.ObtenerBeneficios`), pero es un texto fijo, no seleccionable ni con costo. Esta historia reemplaza esa mención decorativa por un beneficio real, opcional y con impacto en la prima; al implementarla, revisar y actualizar ese texto para no duplicar la mención de forma inconsistente.

## Punto de partida para una mejora posterior

Esta historia es una buena entrada para modelar los "beneficios adicionales" como una colección estructurada (código, descripción, costo) en vez de listas de texto separadas por `|`, y para introducir catálogos de beneficios por aseguradora en lugar de valores codificados en la capa de datos.
