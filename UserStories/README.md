# User stories — QOA Demo

Este directorio documenta la línea base funcional del sistema legado creado para la charla. Las historias describen **lo que hace hoy** la aplicación; la sección de deuda técnica no implica que la mejora ya esté implementada.

## Mapa funcional

| ID | Historia | Actor principal | Prioridad | Estado actual |
|---|---|---|---|---|
| US-001 | [Iniciar y cerrar sesión](US-001-login.md) | Usuario emisor | Alta | Implementada |
| US-002 | [Visualizar el panel de inicio](US-002-home.md) | Usuario emisor | Media | Implementada |
| US-003 | [Capturar Protección de Tarjeta](US-003-emision-proteccion-tarjeta.md) | Usuario emisor | Alta | Implementada |
| US-004 | [Capturar Seguro Vehicular](US-004-emision-vehicular.md) | Usuario emisor | Alta | Implementada |
| US-005 | [Consultar pólizas emitidas](US-005-consulta-polizas.md) | Usuario emisor | Alta | Implementada |
| US-006 | [Consultar el detalle de una póliza](US-006-detalle-poliza.md) | Usuario emisor | Alta | Implementada |
| US-007 | [Anular una póliza](US-007-anulacion-poliza.md) | Usuario emisor | Alta | Implementada |
| US-008 | [Cotizar en múltiples compañías](US-008-cotizacion-multicompania.md) | Usuario emisor | Alta | Implementada |
| US-009 | [Seleccionar oferta y emitir](US-009-seleccionar-oferta-y-emitir.md) | Usuario emisor | Alta | Implementada |

## Convenciones

- `BE`: entidad anémica utilizada para transportar datos entre todas las capas.
- `BL`: clase de lógica de negocio.
- `DL`: clase de acceso a datos.
- Modo simulado: `UsarDatosSimulados=true`; no requiere SQL Server.
- Modo SQL Server: `UsarDatosSimulados=false`; utiliza los procedimientos de `BaseDatos/QOA_DEMO.sql`.

## Uso sugerido durante la charla

Cada historia puede trabajarse como un incremento independiente:

1. Ejecutar el escenario de demostración y observar el comportamiento actual.
2. Identificar la deuda técnica señalada, sin asumir todavía una solución.
3. Elegir una mejora y definir nuevos criterios de aceptación no funcionales.
4. Pedir a la IA una modificación pequeña y verificable.
5. Comparar comportamiento, diseño, seguridad o rendimiento antes y después.

La deuda está intencionalmente distribuida entre interfaz, controladores, lógica, acceso a datos y SQL para permitir varios tipos de ejercicio.
