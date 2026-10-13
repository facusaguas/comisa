# Integración de Tabler en Gestor de Nómina

## ✅ Archivos copiados

Se han copiado exitosamente todos los archivos de Tabler 1.6.1 desde la carpeta de descargas al proyecto:

```
wwwroot/
├── css/tabler/          # Archivos CSS de Tabler
│   ├── tabler.min.css
│   ├── tabler-flags.min.css
│   ├── tabler-payments.min.css
│   ├── tabler-vendors.min.css
│   └── ...
├── js/tabler/           # Archivos JavaScript de Tabler
│   ├── tabler.min.js
│   ├── tabler-theme.min.js
│   └── ...
├── img/tabler/          # Imágenes y recursos
│   ├── flags/
│   ├── payments/
│   └── social/
└── libs/                # Librerías externas (apexcharts, litepicker, etc.)
    ├── apexcharts/
    ├── litepicker/
    ├── tom-select/
    └── ...
```

## 📄 Layouts creados

### 1. `_LayoutTabler.cshtml`
Layout principal con sidebar para páginas autenticadas. Incluye:
- Sidebar vertical con navegación
- Menú de usuario con avatar
- Header de página con título y breadcrumb
- Footer
- Secciones para agregar CSS y JS adicionales

**Uso:**
```csharp
@{
    Layout = "_LayoutTabler";
    ViewData["Title"] = "Título de la página";
    ViewData["PreTitle"] = "Subtítulo o breadcrumb";
}
```

### 2. `_LoginLayoutTabler.cshtml`
Layout simple para páginas de login/registro (sin sidebar):
- Diseño centrado
- Sin navegación
- Perfecto para páginas públicas

**Uso:**
```csharp
@{
    Layout = "_LoginLayoutTabler";
    ViewData["Title"] = "Iniciar Sesión";
}
```

### 3. Vista de ejemplo: `IndexTabler.cshtml`
Vista del dashboard usando componentes de Tabler:
- Cards de estadísticas
- Listas de actividad reciente
- Grillas de datos (datagrid)
- Badges y avatares
- Empty states
- Accesos rápidos con botones

## 🚀 Cómo usar en tus vistas

### Cambiar el layout de una vista existente

Opción 1: Cambiar en la vista individual
```csharp
@{
    Layout = "_LayoutTabler";  // Cambiar de _Layout a _LayoutTabler
    ViewData["Title"] = "Mi Página";
    ViewData["PreTitle"] = "Sección";
}
```

Opción 2: Cambiar globalmente en `_ViewStart.cshtml`
```csharp
@{
    Layout = "_LayoutTabler";  // Afecta a todas las vistas
}
```

### Componentes de Tabler disponibles

#### Cards
```html
<div class="card">
    <div class="card-header">
        <h3 class="card-title">Título</h3>
    </div>
    <div class="card-body">
        Contenido
    </div>
</div>
```

#### Botones
```html
<a href="#" class="btn btn-primary">Primario</a>
<a href="#" class="btn btn-outline-success">Secundario</a>
```

#### Badges
```html
<span class="badge bg-primary">Activo</span>
<span class="badge bg-danger">Inactivo</span>
```

#### Avatares
```html
<span class="avatar">FS</span>
<span class="avatar avatar-lg" style="background-image: url(...)"></span>
```

#### Empty States
```html
<div class="empty">
    <div class="empty-icon">
        <!-- SVG icon aquí -->
    </div>
    <p class="empty-title">Sin resultados</p>
    <p class="empty-subtitle text-secondary">
        No se encontraron elementos
    </p>
</div>
```

#### Datagrid
```html
<div class="datagrid">
    <div class="datagrid-item">
        <div class="datagrid-title">Label</div>
        <div class="datagrid-content">Valor</div>
    </div>
</div>
```

#### Tablas
```html
<div class="table-responsive">
    <table class="table table-vcenter">
        <thead>
            <tr>
                <th>Columna 1</th>
                <th>Columna 2</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Dato 1</td>
                <td>Dato 2</td>
            </tr>
        </tbody>
    </table>
</div>
```

## 🎨 Personalización

### Colores disponibles
- `primary` - Azul (principal)
- `secondary` - Gris
- `success` - Verde
- `danger` - Rojo
- `warning` - Amarillo
- `info` - Cyan
- `azure`, `blue`, `indigo`, `purple`, `pink`, `red`, `orange`, `yellow`, `lime`, `green`, `teal`, `cyan`

### Agregar CSS adicional a una vista
```html
@section Styles {
    <link href="~/css/mi-estilo.css" rel="stylesheet" />
}
```

### Agregar JS adicional a una vista
```html
@section Scripts {
    <script src="~/js/mi-script.js"></script>
}
```

## 📚 Recursos

### Documentación oficial de Tabler
- Website: https://tabler.io/
- GitHub: https://github.com/tabler/tabler
- Preview: https://tabler.io/demo

### Archivos HTML de ejemplo
Puedes consultar los archivos HTML originales en:
```
C:\Users\facundo.sagua_klimbe\Downloads\tabler-1.6.1\dashboard\
```

Ejemplos disponibles:
- `accordion.html`
- `alerts.html`
- `cards.html`
- `form-elements.html`
- `tables.html`
- Y muchos más...

## 🔧 Próximos pasos

1. **Convertir vistas existentes**: Cambia el Layout de las vistas actuales a `_LayoutTabler`
2. **Actualizar formularios**: Usa los estilos de formularios de Tabler
3. **Mejorar tablas**: Aplica las clases de tabla de Tabler
4. **Agregar iconos**: Usa los íconos SVG de Tabler (inline en el HTML)
5. **Usar componentes avanzados**: Explora las librerías en `wwwroot/libs/` como:
   - ApexCharts para gráficos
   - Litepicker para calendarios
   - Tom-select para select mejorados
   - FullCalendar para calendarios completos

## 💡 Ejemplo completo

Ver el archivo `Views/Home/IndexTabler.cshtml` como referencia de implementación completa con:
- Cards de estadísticas
- Accesos rápidos
- Lista de actividad reciente
- Información del sistema
- Empty states
- Iconos SVG
- Y mucho más...

## ⚠️ Notas importantes

1. Los archivos originales de Tabler están en `wwwroot/css/tabler/` y `wwwroot/js/tabler/`
2. Las imágenes están en `wwwroot/img/tabler/`
3. Las librerías adicionales están en `wwwroot/libs/`
4. El layout mantiene compatibilidad con el sistema de autenticación existente
5. Los menús del sidebar se generan dinámicamente según los roles del usuario
