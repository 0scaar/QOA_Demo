# US-003 — Emitir una póliza de Protección de Tarjeta

## Historia

**Como** usuario emisor autenticado  
**quiero** registrar una póliza de Protección de Tarjeta  
**para** asegurar al cliente frente a consumos fraudulentos y robo en cajero.

## Prioridad y alcance

- Prioridad: Alta.
- Actor: Usuario emisor.
- Incluye: contratante, tarjeta, banco, línea de crédito, plan, vigencia y envío a cotización multicompañía.
- No incluye: pago, firma, documentos, validación con el banco ni envío de certificado.

## Reglas actuales

| Plan | Prima neta |
|---|---:|
| PLAN 1 | S/ 29.90 |
| PLAN 2 | S/ 49.90 |
| Cualquier otro valor | S/ 69.90 |

- IGV: 18% de la prima neta.
- Prima total: prima neta más IGV.
- El número generado sigue el formato `PCT-{año}-{correlativo}`.
- Las coberturas cargadas por la lógica son `Compras no reconocidas` y `Robo en cajero`.

## Criterios de aceptación

### Escenario 1: apertura del formulario

**Dado** que el usuario inició sesión  
**cuando** selecciona **Cotizar y emitir > Protección de tarjeta**
**entonces** visualiza el formulario con PLAN 2, línea de crédito S/ 10,000 y vigencia anual como valores iniciales.

### Escenario 2: solicitud de cotización

**Dado** que el usuario completa DNI, nombre, datos de tarjeta, plan y vigencia  
**cuando** selecciona **Cotizar en compañías**
**entonces** el sistema genera ofertas y abre la comparación; la emisión ocurre únicamente después de elegir una.

### Escenario 3: información del detalle

**Dado** que la póliza fue emitida  
**cuando** se abre el detalle  
**entonces** se muestran banco, tipo de tarjeta, últimos cuatro dígitos, plan, importes y coberturas.

### Escenario 4: acceso sin sesión

**Dado** que no existe sesión de usuario  
**cuando** se abre o envía el formulario  
**entonces** la aplicación redirige al login.

## Datos sugeridos para la demostración

| Campo | Valor |
|---|---|
| DNI | `45871234` |
| Nombre | `Mariana Torres` |
| Banco | `BCP` |
| Tarjeta | `Visa` |
| Últimos dígitos | `4587` |
| Plan | `PLAN 2` |

## Componentes involucrados

- `QOA.DEMO.ClienteWeb/Controllers/EmisionController.cs`
- `QOA.DEMO.ClienteWeb/Views/Emision/ProteccionTarjeta.cshtml`
- `QOA.DEMO.Entidades/PolizaBE.cs`
- `QOA.DEMO.LogicaNegocio/PolizaBL.cs`
- `QOA.DEMO.AccesoDatos/PolizaDL.cs`
- `QOA.DEMO.LogicaNegocio/CotizacionBL.cs`
- `QOA.DEMO.AccesoDatos/CotizacionDL.cs`
- Procedimiento `SPI_POLIZA_EMITIR` y función `FN_CALCULAR_PRIMA_MALA`.

## Deuda técnica intencional observable

- Una sola `PolizaBE` contiene datos de tarjeta, vehículo, pantalla y acceso a datos.
- Precios y coberturas están codificados dentro de `PolizaBL`.
- Cálculos de prima e IGV se repiten entre BL, DL y SQL.
- El correlativo simulado recorre todas las pólizas y no es seguro ante concurrencia.
- El procedimiento usa `COUNT`, UDF escalar, tabla variable y `WHILE` por cobertura.
- Validación dependiente del HTML; el modo simulado no replica todas las validaciones SQL.
- No hay transacción que trate persona, póliza, riesgo, coberturas y movimiento como una unidad.
- El formulario no utiliza token antifalsificación.

## Punto de partida para una mejora posterior

La historia permite separar modelos por producto, centralizar reglas, agregar validación de servidor, aplicar transacciones y medir la emisión fila por fila.
