# APICore - Backend

Este repositorio contiene el backend de una aplicación para gestión de tareas y proyectos, desarrollado en ASP.NET Core. Esta API permite la creación, edición y administración de usuarios, proyectos y tareas, así como la autenticación de usuarios con roles específicos.

## Tecnologías utilizadas
- **ASP.NET Core** - Framework de backend
- **Entity Framework Core** - ORM para la manipulación de base de datos
- **SQL Server** - Base de datos
- **JWT** - Autenticación basada en JSON Web Tokens

## Instalación y Configuración

1. **Clona el repositorio**
   ```bash
   git clone https://github.com/camipaula/APICore.git
   cd APICore
2. **Configura la base de datos**

-Asegúrate de tener una instancia de SQL Server en ejecución.
-Configura la cadena de conexión en appsettings.json.
3. **Realiza las migraciones y actualiza la base de datos**
    ```bash
    dotnet ef database update
4. **Endpoints principales**
/api/Usuario/Login: Autenticación de usuarios
/api/Usuario/CreateUser: Creación de un nuevo usuario
/api/Tarea: Gestión de tareas (CRUD)
/api/Proyecto: Gestión de proyectos (CRUD)
5. **Estructura del proyecto**
Controllers: Define los controladores para cada entidad (Usuario, Tarea, Proyecto).
Models: Contiene las clases de las entidades y sus relaciones.
DTOs: Data Transfer Objects para manejar datos de entrada y salida.
Data: Configuración del contexto de base de datos (ApplicationDbContext).
Configurations: Configuración de autenticación y JWT.
