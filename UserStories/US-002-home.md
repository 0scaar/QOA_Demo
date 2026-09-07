# US-002 — Visualizar el panel de inicio

## Historia

**Como** usuario emisor autenticado  
**quiero** visualizar un resumen de la operación de seguros  
**para** conocer rápidamente el volumen emitido y acceder a las tareas más frecuentes.

## Prioridad y alcance

- Prioridad: Media.
- Actor: Usuario emisor.
- Incluye: totales de pólizas, emitidas, anuladas, prima total, últimas emisiones y accesos rápidos.
- No incluye: filtros por fecha, metas, gráficos ni actualización en tiempo real.

## Flujo principal actual

1. El usuario abre el Home después de iniciar sesión.
2. El controlador solicita todas las pólizas a `PolizaBL`.
3. Los indicadores se calculan en memoria.
4. Se muestran hasta cinco elementos de la lista recibida.
5. El usuario puede navegar a las dos emisiones o a la consulta de pólizas.

## Criterios de aceptación

### Escenario 1: indicadores

**Dado** que existen pólizas en el origen activo  
**cuando** el usuario abre el Home  
**entonces** visualiza el total registrado, el total emitido, el total anulado y la suma de prima total.

### Escenario 2: últimas emisiones

**Dado** que existen pólizas registradas  
**cuando** se carga el panel  
**entonces** se muestran como máximo cinco pólizas con número, producto, contratante, estado y prima.

### Escenario 3: navegación rápida

**Dado** que el usuario está en el Home  
**cuando** selecciona un acceso rápido  
**entonces** navega a Protección de Tarjeta, Seguro Vehicular o Consulta de Pólizas según corresponda.

### Escenario 4: usuario sin sesión

**Dado** que no existe sesión  
**cuando** se solicita el Home  
**entonces** el sistema redirige al login.

## Componentes involucrados

- `QOA.DEMO.ClienteWeb/Controllers/HomeController.cs`
- `QOA.DEMO.ClienteWeb/Views/Home/Index.cshtml`
- `QOA.DEMO.LogicaNegocio/PolizaBL.cs`
- `QOA.DEMO.AccesoDatos/PolizaDL.cs`

## Deuda técnica intencional observable

- Recupera la bandeja completa para calcular cuatro indicadores.
- Realiza conteos y suma en el servidor web, no en el origen de datos.
- La lógica del resumen está dentro del controlador mediante `ViewBag`.
- La selección de últimas pólizas ocurre después de cargar toda la colección.
- No existe caché, paginación, consulta agregada ni modelo específico para el Home.
- El orden se apoya en una fecha almacenada como texto en modo simulado.

## Punto de partida para una mejora posterior

La historia permite comparar agregaciones en memoria contra consultas especializadas, crear un view model, medir tiempos y diseñar indicadores escalables.
