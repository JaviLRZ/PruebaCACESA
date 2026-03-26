# 🗂 Gestor de Peticiones de Material

Aplicación de escritorio en **C# / Windows Forms (.NET 8)** desarrollada como prueba técnica.  
Permite gestionar usuarios y solicitudes de material con roles diferenciados (Supervisor / Usuario).

---

## 🚀 Cómo ejecutar

### Prerrequisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Windows (WinForms es exclusivo de Windows)

### Pasos
```bash
git clone https://github.com/<tu-usuario>/GestorPeticiones.git
cd GestorPeticiones
dotnet run --project GestorPeticiones/GestorPeticiones.csproj
```

### Credenciales de ejemplo
| Usuario | Contraseña | Rol |
|---------|-----------|-----|
| `admin` | `admin123` | Supervisor |

> Los datos se persisten en `GestorPeticiones/bin/Debug/net8.0-windows/Data/`  
> (ficheros `usuarios.json` y `peticiones.json`)

---

## ✨ Funcionalidades

### 👥 Gestión de Usuarios *(solo Supervisor)*
- Alta, modificación y borrado de usuarios
- Validación de nombre de usuario único

### 📋 Peticiones de Material
- Cualquier usuario puede enviar una solicitud de material
- El **Supervisor** puede **aceptar o rechazar** peticiones pendientes con comentario

### 🔍 Búsqueda de Peticiones
| Filtro | Usuario | Supervisor |
|--------|---------|-----------|
| Por descripción | ✅ | ✅ |
| Por fecha desde/hasta | ✅ | ✅ |
| Por estado (Pendiente / Aceptada / Rechazada) | ✅ | ✅ |
| Por nombre de usuario | ❌ | ✅ |

---

## 🏗 Arquitectura

Arquitectura en capas siguiendo principios **SOLID** y **Clean Code**:

```
GestorPeticiones/
├── Models/              → Entidades de dominio
│   ├── Usuario.cs
│   └── PeticionMaterial.cs
├── Repositories/        → Acceso a datos (patrón Repository)
│   ├── IRepository.cs   → Interfaz genérica (Open/Closed)
│   └── JsonRepository.cs → Persistencia JSON con LINQ
├── Services/            → Lógica de negocio
│   ├── IUsuarioService.cs / UsuarioService.cs
│   └── IPeticionService.cs / PeticionService.cs
├── Forms/               → Capa de presentación (WinForms)
│   ├── LoginForm.cs
│   ├── MainForm.cs      → Menú adaptado al rol
│   ├── UsuarioForm.cs   → CRUD usuarios
│   └── PeticionForm.cs  → Solicitudes y búsquedas
├── ServiceLocator.cs    → Composición de dependencias
├── SeedData.cs          → Datos iniciales (admin por defecto)
└── Program.cs           → Composition Root
```

### Principios SOLID aplicados

| Principio | Aplicación |
|-----------|-----------|
| **S** – Single Responsibility | Cada clase tiene una única responsabilidad |
| **O** – Open/Closed | `IRepository<T>` admite nuevas implementaciones sin modificar el contrato |
| **L** – Liskov Substitution | `JsonRepository<T>` sustituye `IRepository<T>` sin romper el contrato |
| **I** – Interface Segregation | `IUsuarioService` e `IPeticionService` separados |
| **D** – Dependency Inversion | Los formularios dependen de interfaces, no de implementaciones |

### LINQ en la capa de servicio

El método `PeticionService.Buscar()` encadena filtros LINQ opcionales:
```csharp
var query = _repositorio.ObtenerTodos().AsQueryable();
if (!string.IsNullOrWhiteSpace(descripcion))
    query = query.Where(p => p.Descripcion.Contains(descripcion, StringComparison.OrdinalIgnoreCase));
if (fechaDesde.HasValue)
    query = query.Where(p => p.FechaSolicitud.Date >= fechaDesde.Value.Date);
// ... más filtros opcionales
return query.OrderByDescending(p => p.FechaSolicitud).ToList();
```

---

## 🛠 Tecnologías

- **C# / .NET 8** – Windows Forms
- **System.Text.Json** – Persistencia en JSON (sin dependencias externas)
- **LINQ** – Búsquedas y filtros
- **Patrón Repository** – Desacoplamiento de datos

---

## 📁 Datos persistidos

La aplicación guarda los datos en JSON automáticamente:

```json
// usuarios.json (ejemplo)
[
  {
    "IdUsuario": 1,
    "Nombre": "Admin",
    "NombreUsuario": "admin",
    "EsSupervisor": true
  }
]
```
---

## ⚡️ Desarrollo y Velocidad

Este proyecto ha sido desarrollado utilizando el asistente **Antigravity**.  
Debido a las restricciones de tiempo de la prueba técnica (2 horas), se optó por esta herramienta para garantizar la máxima **calidad de código**.
