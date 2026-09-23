# US-011 — Ver el beneficio de Auto de Reemplazo en la comparación de ofertas

## Historia

**Como** usuario emisor autenticado
**quiero** que la comparación de ofertas vehiculares muestre el beneficio "Auto de Reemplazo" y su costo dentro de cada oferta
**para** decidir con qué aseguradora emitir sin confundirlo con un producto o póliza aparte.

## Prioridad y alcance

- Prioridad: Alta.
- Actor: Usuario emisor.
- Precondición: existe una cotización vehicular activa generada con el beneficio marcado (ver [[US-010]]).
- Incluye: visualización del beneficio dentro de cada una de las cinco ofertas, recálculo de la prima total con el costo adicional, comparación de precio entre ofertas ya con ese costo incluido.
- No incluye: una pantalla, pestaña o listado separado para el beneficio; no debe aparecer como si fuera una cotización o póliza independiente.

## Reglas propuestas

- El costo adicional del beneficio se suma a `primaNeta` antes de calcular el IGV y la `primaTotal` de cada oferta, igual que el resto de la prima.
- El beneficio se lista junto a los demás beneficios/coberturas de la oferta (mismo bloque visual que hoy usa `OfertaCotizacionBE.beneficios`), no en una sección ni tarjeta separada.
- Si el usuario no marcó el beneficio en el formulario ([[US-010]], Escenario 2), ninguna de las cinco ofertas lo muestra ni lo cobra.
- La etiqueta "Mejor precio" sigue determinándose por `primaTotal`, ya incluyendo el costo del beneficio cuando corresponda.

## Criterios de aceptación

### Escenario 1: el beneficio aparece en las cinco ofertas

**Dado** que el usuario cotizó un vehículo marcando "Auto de Reemplazo"
**cuando** abre la comparación de ofertas
**entonces** las cinco ofertas (Rímac, Pacífico, MAPFRE, La Positiva, Seguros Andina) muestran "Auto de Reemplazo" dentro de su lista de beneficios.

### Escenario 2: se ve como beneficio, no como póliza aparte

**Dado** que el usuario está viendo la comparación con el beneficio activo
**cuando** revisa la pantalla `CompararCotizaciones`
**entonces** "Auto de Reemplazo" aparece listado junto a las demás coberturas de cada oferta (por ejemplo, junto a "Daños propios" o "Responsabilidad civil"), y no existe ninguna oferta, tarjeta o sección adicional que lo presente como un producto independiente.

### Escenario 3: la prima total incluye el costo adicional

**Dado** que el usuario cotizó con el beneficio marcado
**cuando** compara las primas de las cinco ofertas
**entonces** cada `primaTotal` es mayor que la que se habría obtenido sin el beneficio, en un monto verificable atribuible al costo adicional acordado para ese beneficio.

### Escenario 4: sin el beneficio, no hay costo ni mención

**Dado** que el usuario cotizó sin marcar "Auto de Reemplazo"
**cuando** compara las ofertas
**entonces** ninguna de las cinco ofertas menciona el beneficio ni su `primaTotal` incluye costo adicional por él.

### Escenario 5: consistencia entre aseguradoras

**Dado** que el beneficio fue marcado
**cuando** se generan las cinco ofertas
**entonces** todas lo incluyen (ninguna aseguradora lo omite), aunque el costo adicional pueda variar según el factor de tarifa de cada compañía.

## Componentes probablemente involucrados

- `QOA.DEMO.ClienteWeb/Views/Emision/CompararCotizaciones.cshtml`
- `QOA.DEMO.Entidades/OfertaCotizacionBE.cs`
- `QOA.DEMO.AccesoDatos/CotizacionDL.cs` (cálculo de `primaNeta`/`primaTotal` y texto de `beneficios`)

## Punto de partida para una mejora posterior

Modelar los beneficios como una lista tipada de objetos (código, nombre, costo) en `OfertaCotizacionBE` permitiría distinguir en la vista qué beneficios son base y cuáles son opcionales con costo, sin depender de parsear una cadena de texto separada por `|`.
