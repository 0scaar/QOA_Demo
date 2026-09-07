# US-005 — Consultar pólizas emitidas

## Historia

**Como** usuario emisor autenticado  
**quiero** consultar las pólizas registradas  
**para** localizar una póliza y continuar con su detalle o anulación.

## Prioridad y alcance

- Prioridad: Alta.
- Actor: Usuario emisor.
- Incluye: listado completo y filtro general por número, documento o nombre.
- No incluye: paginación, exportación, filtros avanzados, orden configurable ni permisos por cartera.

## Flujo principal actual

1. El usuario abre **Consultar pólizas**.
2. El sistema carga todas las pólizas si no existe filtro.
3. El usuario puede ingresar un texto general.
4. La búsqueda compara el texto con número de póliza, DNI y nombre del contratante.
5. La tabla muestra información resumida y un botón para abrir el detalle.

## Criterios de aceptación

### Escenario 1: consulta sin filtro

**Dado** que existen pólizas registradas  
**cuando** el usuario abre la consulta sin enviar un filtro  
**entonces** visualiza todas las pólizas del origen activo.

### Escenario 2: búsqueda por póliza

**Dado** que existe la póliza `PCT-2026-000001`  
**cuando** el usuario busca una parte de ese número  
**entonces** la tabla incluye la póliza coincidente.

### Escenario 3: búsqueda por documento o nombre

**Dado** que existe una póliza asociada a un contratante  
**cuando** el usuario busca su DNI o parte de su nombre sin distinguir mayúsculas  
**entonces** se muestran las coincidencias.

### Escenario 4: sin resultados

**Dado** que el texto no coincide con ninguna póliza  
**cuando** finaliza la consulta  
**entonces** la tabla muestra `No se encontraron pólizas`.

### Escenario 5: abrir detalle

**Dado** que la tabla contiene resultados  
**cuando** el usuario selecciona **Detalle**  
**entonces** navega a la consulta detallada de la póliza elegida.

## Información mostrada

- Número de póliza.
- Producto.
- Compañía seleccionada o indicación de emisión directa.
- Documento y nombre del contratante.
- Fecha de emisión.
- Prima total.
- Estado.
- Acción de detalle.

## Componentes involucrados

- `QOA.DEMO.ClienteWeb/Controllers/PolizaController.cs`
- `QOA.DEMO.ClienteWeb/Views/Poliza/Index.cshtml`
- `QOA.DEMO.LogicaNegocio/PolizaBL.cs`
- `QOA.DEMO.AccesoDatos/PolizaDL.cs`
- Procedimiento `SPS_POLIZA_CONSULTAR`.

## Deuda técnica intencional observable

- No existe paginación ni límite de filas.
- El modo simulado copia, recorre, espera y filtra toda la colección en memoria.
- Convierte a minúsculas cada valor durante cada búsqueda.
- Ordena fechas representadas como texto.
- SQL utiliza tabla variable, UDF escalar, subconsultas correlacionadas, `LIKE '%texto%'` y conversiones sobre columnas.
- Devuelve todas las columnas de la tabla variable y no expone duración ni volumen de la consulta.
- El controlador depende de `ViewBag` y de una entidad de dominio usada como filtro de pantalla.

## Punto de partida para una mejora posterior

La historia es adecuada para introducir medición, paginación, índices, filtros SARGables, consultas proyectadas y separación entre filtro, resultado y entidad.
