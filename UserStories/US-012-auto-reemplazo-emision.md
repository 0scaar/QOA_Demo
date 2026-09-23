# US-012 — Emitir una póliza vehicular con el beneficio de Auto de Reemplazo

## Historia

**Como** usuario emisor autenticado
**quiero** que al elegir una oferta que incluye "Auto de Reemplazo" la póliza emitida conserve ese beneficio y su costo
**para** que quede registrado que el asegurado lo contrató, con la prima correcta.

## Prioridad y alcance

- Prioridad: Alta.
- Actor: Usuario emisor.
- Precondición: existe una cotización vehicular activa con al menos una oferta que incluye el beneficio (ver [[US-010]] y [[US-011]]).
- Incluye: selección de la oferta, emisión de la póliza con el beneficio y su costo adicional reflejados, visualización en el detalle de la póliza.
- No incluye: modificar o anular el beneficio después de emitida la póliza, ni condiciones de uso del vehículo prestado.

## Reglas propuestas

- Al emitir, `PolizaBL`/`PolizaDL` deben conservar el indicador del beneficio y su costo, igual que hoy conservan `deducible` u otros datos copiados desde la oferta elegida.
- La `primaTotal` de la póliza emitida es la de la oferta seleccionada, que ya incluye el costo del beneficio cuando corresponde ([[US-011]], Escenario 3).
- El beneficio solo puede quedar registrado en pólizas `VEHICULAR`; una póliza de Protección de Tarjeta nunca lo lleva.

## Criterios de aceptación

### Escenario 1: la póliza conserva el beneficio elegido

**Dado** que el usuario elige una oferta vehicular que incluye "Auto de Reemplazo"
**cuando** se emite la póliza
**entonces** la póliza emitida queda marcada con el beneficio y con la prima total de la oferta elegida, incluyendo el costo adicional.

### Escenario 2: el detalle de la póliza lo muestra

**Dado** que la póliza fue emitida con el beneficio incluido
**cuando** el usuario abre su detalle
**entonces** "Auto de Reemplazo" aparece listado junto a las demás coberturas de la póliza (por ejemplo junto a "Daños propios" y "Responsabilidad civil").

### Escenario 3: póliza sin el beneficio

**Dado** que el usuario elige una oferta que no incluye "Auto de Reemplazo" (porque no lo marcó al cotizar)
**cuando** se emite y se abre su detalle
**entonces** el beneficio no aparece en ninguna parte de la póliza ni de su prima.

### Escenario 4: sin efecto sobre pólizas de Protección de Tarjeta

**Dado** que se emite una póliza de Protección de Tarjeta, con o sin ofertas de por medio
**cuando** se revisa su detalle
**entonces** no existe ninguna referencia al beneficio "Auto de Reemplazo" ni ningún costo asociado a él.

### Escenario 5: trazabilidad en la consulta de pólizas

**Dado** que existen pólizas vehiculares emitidas con y sin el beneficio
**cuando** el usuario abre la consulta de pólizas y luego el detalle de cada una
**entonces** puede distinguir, entrando al detalle, cuáles incluyeron el beneficio y cuáles no.

## Componentes probablemente involucrados

- `QOA.DEMO.Entidades/PolizaBE.cs` (campo(s) nuevos para el beneficio, junto a los ya mezclados de vehicular)
- `QOA.DEMO.LogicaNegocio/PolizaBL.cs`
- `QOA.DEMO.LogicaNegocio/CotizacionBL.cs` (copia de datos de la oferta elegida hacia `PolizaBE`, igual que hace hoy con `deducible`)
- `QOA.DEMO.AccesoDatos/PolizaDL.cs`
- `QOA.DEMO.ClienteWeb/Views/Poliza/Detalle.cshtml`

## Punto de partida para una mejora posterior

Al igual que en [[US-009]], la copia manual de campos entre `OfertaCotizacionBE` y `PolizaBE` es deuda técnica intencional; esta historia es una buena entrada para introducir un mapeo explícito o un objeto de valor "beneficios contratados" reutilizable entre cotización y póliza.
