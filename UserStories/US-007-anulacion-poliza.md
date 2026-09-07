# US-007 — Anular una póliza

## Historia

**Como** usuario emisor autenticado  
**quiero** anular una póliza emitida indicando un motivo  
**para** impedir que permanezca vigente y dejar registro de la operación.

## Prioridad y alcance

- Prioridad: Alta.
- Actor: Usuario emisor.
- Precondición: la póliza existe y está en estado `EMITIDA`.
- Incluye: captura de motivo, cambio de estado, fecha de anulación y movimiento de auditoría en SQL.
- No incluye: devolución de prima, aprobación, notificación, reversión contable ni reactivación.

## Flujo principal actual

1. El usuario abre el detalle de una póliza emitida.
2. Selecciona **Anular póliza**.
3. El sistema abre un modal y solicita un motivo.
4. El usuario confirma.
5. La BL delega la operación a la DL.
6. La póliza cambia a `ANULADA` y registra fecha y motivo.
7. La aplicación vuelve al detalle y muestra el resultado.

## Criterios de aceptación

### Escenario 1: mostrar acción

**Dado** que la póliza está `EMITIDA`  
**cuando** el usuario abre el detalle  
**entonces** visualiza el botón **Anular póliza**.

### Escenario 2: confirmación

**Dado** que el modal de anulación está abierto  
**cuando** el usuario ingresa un motivo y confirma  
**entonces** el sistema cambia el estado a `ANULADA`, registra fecha y motivo y vuelve al detalle.

### Escenario 3: póliza ya anulada

**Dado** que la póliza ya se encuentra anulada  
**cuando** se muestra su detalle  
**entonces** no aparece el botón y se visualizan la fecha y el motivo de anulación.

### Escenario 4: validaciones en SQL

**Dado** que el modo SQL está activo  
**cuando** se solicita anular con identificador inválido, póliza inexistente, póliza ya anulada o motivo menor de cinco caracteres  
**entonces** `SPU_POLIZA_ANULAR` rechaza la operación con el mensaje correspondiente.

### Escenario 5: registro de movimiento

**Dado** que la anulación SQL finaliza  
**cuando** se consulta `POL_MOVIMIENTO`  
**entonces** existe un movimiento `ANULACION` con póliza, motivo, usuario y fecha.

## Datos sugeridos para la demostración

- Seleccionar una póliza con estado `EMITIDA`.
- Motivo: `Solicitud expresa del cliente`.
- Verificar que desaparece el botón después de confirmar.

## Componentes involucrados

- `QOA.DEMO.ClienteWeb/Controllers/PolizaController.cs`
- `QOA.DEMO.ClienteWeb/Views/Poliza/Detalle.cshtml`
- `QOA.DEMO.LogicaNegocio/PolizaBL.cs`
- `QOA.DEMO.AccesoDatos/PolizaDL.cs`
- Procedimiento `SPU_POLIZA_ANULAR`.

## Deuda técnica intencional observable

- La validación del motivo no es consistente: el HTML exige contenido, el modo simulado lo acepta sin reglas y SQL exige cinco caracteres.
- La operación HTTP no utiliza token antifalsificación.
- No existe control de concurrencia; dos solicitudes pueden intentar anular la misma póliza.
- No hay autorización específica para anulación ni flujo de aprobación.
- El modo simulado modifica un objeto global sin sincronización ni auditoría persistente.
- SQL desactiva coberturas una por una mediante cursor y `WAITFOR`.
- La actualización de póliza, coberturas y movimiento no está protegida por una transacción explícita.
- Se muestran mensajes técnicos provenientes de excepciones.

## Punto de partida para una mejora posterior

La historia permite trabajar seguridad, reglas consistentes, idempotencia, concurrencia, transacciones, auditoría y operaciones SQL por conjuntos.
