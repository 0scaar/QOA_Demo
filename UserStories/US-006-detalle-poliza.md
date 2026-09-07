# US-006 — Consultar el detalle de una póliza

## Historia

**Como** usuario emisor autenticado  
**quiero** consultar todos los datos de una póliza  
**para** verificar el contratante, el riesgo, las coberturas, los importes y su estado.

## Prioridad y alcance

- Prioridad: Alta.
- Actor: Usuario emisor.
- Incluye: información general, datos específicos por producto, coberturas, estado y acceso a anulación.
- No incluye: documentos, historial completo, impresión, endosos ni trazabilidad de integraciones.

## Criterios de aceptación

### Escenario 1: detalle general

**Dado** que existe una póliza  
**cuando** el usuario abre su detalle  
**entonces** visualiza número, producto, contratante, documento, emisión, vigencia, prima neta, IGV, prima total y estado.

### Escenario 2: detalle de tarjeta

**Dado** que la póliza corresponde a Protección de Tarjeta  
**cuando** se muestra el detalle  
**entonces** se visualizan banco, tipo de tarjeta, últimos cuatro dígitos y plan.

### Escenario 3: detalle vehicular

**Dado** que la póliza corresponde a Seguro Vehicular  
**cuando** se muestra el detalle  
**entonces** se visualizan placa, marca, modelo, año, uso y valor comercial.

### Escenario 4: coberturas

**Dado** que la póliza tiene coberturas asociadas  
**cuando** se carga el detalle  
**entonces** las coberturas aparecen en una lista.

### Escenario 5: póliza inexistente

**Dado** un identificador que no existe  
**cuando** se solicita el detalle  
**entonces** la aplicación responde con HTTP 404.

### Escenario 6: acción según estado

**Dado** que la póliza está `EMITIDA`  
**cuando** se muestra el detalle  
**entonces** aparece la opción de anulación; si está `ANULADA`, se muestra fecha y motivo y se oculta esa opción.

## Componentes involucrados

- `QOA.DEMO.ClienteWeb/Controllers/PolizaController.cs`
- `QOA.DEMO.ClienteWeb/Views/Poliza/Detalle.cshtml`
- `QOA.DEMO.AccesoDatos/PolizaDL.cs`
- `QOA.DEMO.Entidades/PolizaBE.cs`
- Procedimiento `SPS_POLIZA_DETALLE`.

## Deuda técnica intencional observable

- El controlador evita `PolizaBL` e instancia directamente `PolizaDL`.
- En modo simulado, el detalle primero ejecuta la consulta completa y vuelve a recorrer sus resultados.
- La vista decide el tipo de riesgo mediante un literal y conoce toda la entidad anémica.
- SQL usa `P.*`, convierte el identificador a texto y ejecuta una subconsulta por cada dato relacionado.
- El procedimiento devuelve múltiples result sets y el mapeo es manual, frágil y dependiente de nombres de columna.
- No existe un DTO o view model específico para el detalle.

## Punto de partida para una mejora posterior

La historia permite reparar la dependencia entre capas, eliminar el patrón N+1, proyectar una consulta eficiente y diseñar modelos de lectura por producto.
