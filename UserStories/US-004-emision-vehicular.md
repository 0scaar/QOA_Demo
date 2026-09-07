# US-004 — Emitir una póliza de Seguro Vehicular

## Historia

**Como** usuario emisor autenticado  
**quiero** registrar una póliza vehicular  
**para** asegurar un vehículo que circula en Perú.

## Prioridad y alcance

- Prioridad: Alta.
- Actor: Usuario emisor.
- Incluye: contratante, placa, marca, modelo, año, motor, VIN, valor comercial, uso, vigencia y envío a cotización multicompañía.
- No incluye: inspección, tarifario por zona, conductor, fotografías ni integración con una aseguradora externa real.

## Reglas actuales

- Prima neta base: 2.8% del valor comercial.
- Para uso `TAXI`, la prima base se multiplica por 1.45.
- IGV: 18% de la prima neta.
- El número generado sigue el formato `VEH-{año}-{correlativo}`.
- Coberturas iniciales: `Danos propios` y `Responsabilidad civil`.

## Criterios de aceptación

### Escenario 1: apertura del formulario

**Dado** que el usuario inició sesión  
**cuando** selecciona **Cotizar y emitir > Vehicular**
**entonces** visualiza el formulario con año actual, valor comercial inicial de S/ 60,000, uso particular y vigencia anual.

### Escenario 2: cotización particular

**Dado** que el usuario completa contratante y vehículo con uso `PARTICULAR`  
**cuando** solicita la cotización
**entonces** se usa como base el 2.8% del valor comercial y se muestran ofertas con factores distintos.

### Escenario 3: emisión de taxi

**Dado** que el usuario selecciona uso `TAXI`  
**cuando** solicita la cotización
**entonces** el sistema aplica el multiplicador 1.45 sobre la prima base antes de calcular el IGV.

### Escenario 4: detalle del riesgo

**Dado** que se emitió la póliza  
**cuando** se abre su detalle  
**entonces** se muestran placa, marca, modelo, año, uso, valor comercial, importes y coberturas.

## Datos sugeridos para la demostración

| Campo | Valor |
|---|---|
| DNI | `70112233` |
| Nombre | `Carlos Paredes` |
| Placa | `ABC-123` |
| Marca | `Toyota` |
| Modelo | `Corolla` |
| Año | `2022` |
| Valor comercial | `55000` |
| Uso | `PARTICULAR` |

## Componentes involucrados

- `QOA.DEMO.ClienteWeb/Controllers/EmisionController.cs`
- `QOA.DEMO.ClienteWeb/Views/Emision/Vehicular.cshtml`
- `QOA.DEMO.Entidades/PolizaBE.cs`
- `QOA.DEMO.LogicaNegocio/PolizaBL.cs`
- `QOA.DEMO.AccesoDatos/PolizaDL.cs`
- `QOA.DEMO.LogicaNegocio/CotizacionBL.cs`
- `QOA.DEMO.AccesoDatos/CotizacionDL.cs`
- Procedimiento `SPI_POLIZA_EMITIR` y función `FN_CALCULAR_PRIMA_MALA`.

## Deuda técnica intencional observable

- Datos vehiculares y de tarjeta conviven en la misma entidad.
- Porcentajes y multiplicadores son números mágicos.
- Reglas de cálculo duplicadas y no consistentes entre memoria y SQL.
- El procedimiento recibe numerosos parámetros, varios opcionales y sin objeto específico de producto.
- Generación de correlativo mediante conteo total y estado global sin sincronización.
- Inserción de coberturas fila por fila con tabla variable y `WHILE`.
- Validación incompleta en el servidor web.
- Ausencia de transacción de negocio y de token antifalsificación.

## Punto de partida para una mejora posterior

La historia permite trabajar estrategias de cálculo por producto, catálogos, objetos de valor, validación, concurrencia y consistencia transaccional.
