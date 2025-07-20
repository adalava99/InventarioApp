# InventarioApp

Sistema de gestión de productos y transacciones implementado como microservicios usando .NET 8.

## Tecnologías utilizadas

- Visual Studio 2022
- .NET 8.0
- SQL Server 2022 Developer Edition

## Instrucciones para ejecutar el proyecto

### 1. Clonar el repositorio

```bash
git clone https://github.com/adalava99/InventarioApp.git
cd InventarioApp
```

### 2. Restaurar los paquetes NuGet

Desde la terminal:

```bash
dotnet restore
```

> Por favor, ejecute el comando previo desde el root del proyecto (donde esta el `.sln`).
---
> Alternativamente, puede usar Visual Studio: clic derecho sobre la solución → Restore NuGet Packages.
---
> Si presenta algún problema puede instalarlos manualmente desde el administrador de packetes NuGet. Ambos microservicios usan los mismos paquetes y estan especificados al final del archivo.

---

### 3. Crear las bases de datos

Abrir SQL Server Management Studio y ejecutar el script `CrearBdInventario.sql` ubicado en la raíz del proyecto.

Esto creará:

- Base de datos `ProductosDb` con tabla `productos` y datos dummy.
- Base de datos `TransaccionesDb` con tabla `transacciones` y algunas transacciones de ejemplo.

---

### 4. Configurar la conexión a base de datos

En el archivo `appsettings.json` de cada microservicio (`ServicioProductos` y `ServicioTransacciones`), asegúrese de actualizar la clave `DefaultConnection` con el nombre de servidor local de SQL Server.

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=MI_SERVIDOR_SQL;Database=ProductosDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

> Reemplace `"MI_SERVIDOR_SQL"` por el nombre de su instancia local.

---

### 5. Configurar puertos y servicios externos (para ServicioTransacciones)

## Puertos por defecto

- **ServicioProductos**  
  `https://localhost:7057`  
  `http://localhost:5220`

- **ServicioTransacciones**  
  `https://localhost:7249`  
  `http://localhost:5085`

> Puede modificarlos desde Properties/launchSettings.json en cada proyecto si los puertos en su máquina no están disponibles.

En `appsettings.json` de `ServicioTransacciones`, actualice el puerto del ServicioProductos en caso de haberlo modificado:

```json
"ExternalServices": {
  "ServicioProductos": "http://localhost:5220"
}
```

---

### 6. Ejecutar el backend

Desde Visual Studio:

- Abrir `InventarioApp.sln`
- Click derecho sobre la solución → Establecer proyectos de inicio
- Seleccionar varios proyectos de inicio
- Setear Acción : Inicio para ambos proyectos
- Tipo de depuración : http

Desde terminal:

```bash
cd ServicioProductos
dotnet run
```

Y en otra terminal:

```bash
cd ServicioTransacciones
dotnet run
```

---

- El proyecto se ejecutó y probó correctamente usando **HTTP**, se recomienda aplicar lo mismo al ejecutar localmente.

## Paquetes NuGet utilizados

Asegúrese de tener instalados los siguientes paquetes en ambos proyectos:

- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.18
- `Microsoft.EntityFrameworkCore.Tools` 8.0.18
- `Swashbuckle.AspNetCore.Annotations` 6.6.2
- `Swashbuckle.AspNetCore` 6.6.2
- `Serilog` 4.3.0
- `Serilog.Sinks.File` 7.0.0
- `Serilog.AspNetCore` 8.0.3

---

## Notas finales

- Swagger está habilitado por defecto en ambos servicios.
- La carpeta `logs/` se crea automáticamente al iniciar los servicios si ocurre algún error de servidor.

---
