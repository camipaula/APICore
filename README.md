# APICore - Backend

Este repositorio contiene el backend de una aplicación para gestión de tareas y proyectos, desarrollado en ASP.NET Core. Esta API permite la creación, edición y administración de usuarios, proyectos y tareas, así como la autenticación de usuarios con roles específicos.

## Tecnologías utilizadas
- **ASP.NET Core** - Framework de backend
- **Entity Framework Core** - ORM para la manipulación de base de datos
- **SQL Server** - Base de datos
- **JWT** - Autenticación basada en JSON Web Tokens

## Configuración

1. **Configuración de la Base de Datos:**
   - Abre el archivo `appsettings.json`.
   - Actualiza la cadena de conexión en `ConnectionStrings`:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=TU_SERVIDOR;Database=APICoreDB;User Id=TU_USUARIO;Password=TU_CONTRASEÑA;"
     }
     ```

2. **Migraciones:**
   Ejecuta los siguientes comandos para aplicar migraciones y actualizar la base de datos:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

## Uso

1. Inicia la aplicación:
   ```bash
   dotnet run
   ```

2. La API estará disponible en:
   ```
   http://localhost:5000
   ```
   La documentación Swagger estará disponible en:
   ```
   http://localhost:5000/swagger
   ```

## Estructura de Carpetas
```
APICore/
│
├── Controllers/         # Controladores de la API
├── Models/              # Modelos de Entidad
├── Data/                # Contexto de Base de Datos y Migraciones
├── Services/            # Lógica de Negocio y Servicios
├── appsettings.json     # Configuración de la Aplicación
├── Program.cs           # Punto de Entrada
└── Startup.cs           # Configuración y Middleware
```

## Contribuciones

¡Las contribuciones son bienvenidas! Sigue estos pasos para contribuir:

1. Haz un fork del repositorio.
2. Crea una nueva rama (`git checkout -b feature-nombre`).
3. Realiza tus cambios y haz un commit (`git commit -m 'Añadir nueva característica'`).
4. Haz un push a la rama (`git push origin feature-nombre`).
5. Abre un Pull Request.

## Licencia

Este proyecto está licenciado bajo la [Licencia MIT](LICENSE).

---

### Autor
Creado por **Camila Paula**. Si tienes preguntas o sugerencias, no dudes en contactarme.

---

### Agradecimientos
Gracias a la comunidad de código abierto por proporcionar herramientas y frameworks que facilitan el desarrollo.

