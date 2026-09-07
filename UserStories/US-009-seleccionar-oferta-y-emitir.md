# US-009 — Seleccionar una oferta y emitir la póliza

## Historia

**Como** usuario emisor autenticado  
**quiero** elegir una de las ofertas cotizadas  
**para** emitir la póliza con la compañía y el precio seleccionados.

## Prioridad y alcance

- Prioridad: Alta.
- Actor: Usuario emisor.
- Precondición: existe una cotización activa con ofertas.
- Incluye: selección, descarte lógico de las demás ofertas, emisión y navegación al detalle.
- No incluye: confirmación adicional, pago, firma, comunicación real con la compañía ni rollback distribuido.

## Flujo principal actual

1. El usuario selecciona **Elegir y emitir póliza** en una oferta.
2. El controlador recupera toda la cotización desde `Session`.
3. La capa de datos recorre todas las ofertas y marca la elegida.
4. La BL copia precio, compañía, deducible e identificadores a `PolizaBE`.
5. `PolizaBL` detecta la oferta y evita ejecutar su cálculo de emisión directa.
6. `PolizaDL` emite la póliza y el sistema abre el detalle.

## Criterios de aceptación

### Escenario 1: selección válida

**Dado** que existen cuatro ofertas disponibles  
**cuando** el usuario elige una  
**entonces** se emite una sola póliza con la prima de esa oferta y se abre su detalle.

### Escenario 2: trazabilidad

**Dado** que la póliza nació de una cotización  
**cuando** se visualiza su detalle  
**entonces** aparecen la compañía, el número de cotización y el deducible cuando corresponda.

### Escenario 3: bandeja

**Dado** que la póliza fue emitida desde una oferta  
**cuando** el usuario abre la consulta de pólizas  
**entonces** la tabla muestra la compañía seleccionada.

### Escenario 4: sesión perdida

**Dado** que la cotización ya no existe en `Session`  
**cuando** se intenta emitir una oferta  
**entonces** la aplicación no emite y vuelve al inicio con un mensaje.

## Componentes involucrados

- `QOA.DEMO.ClienteWeb/Controllers/EmisionController.cs`
- `QOA.DEMO.LogicaNegocio/CotizacionBL.cs`
- `QOA.DEMO.LogicaNegocio/PolizaBL.cs`
- `QOA.DEMO.AccesoDatos/CotizacionDL.cs`
- `QOA.DEMO.AccesoDatos/PolizaDL.cs`
- `QOA.DEMO.ClienteWeb/Views/Poliza/Detalle.cshtml`
- Procedimientos `SPU_COTIZACION_SELECCIONAR` y `SPI_POLIZA_EMITIR`.

## Deuda técnica intencional observable

- Se confía en una cotización mutable guardada en sesión y en un identificador enviado por el navegador.
- No hay transacción entre seleccionar oferta y emitir póliza.
- Si la emisión falla, la cotización puede quedar seleccionada sin póliza.
- La BL copia manualmente campos de una entidad a otra.
- Se crean dependencias con `new` y se recorren ofertas para encontrar una por identificador.
- No hay idempotencia; un doble envío puede intentar emitir dos veces.
- El procedimiento actualiza las ofertas una por una mediante cursor y espera artificial.
- No se vuelve a validar precio, vencimiento ni disponibilidad con la compañía.

## Punto de partida para una mejora posterior

La historia permite trabajar transacciones, idempotencia, autorización, validación del lado servidor, estados explícitos y un proceso de emisión resiliente.
