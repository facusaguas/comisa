# Ejemplos de Vistas con Tabler

## 📋 Vista de Lista (Index)

```cshtml
@model IEnumerable<Empleado>
@{
    Layout = "_LayoutTabler";
    ViewData["Title"] = "Empleados";
    ViewData["PreTitle"] = "Gestión";
}

@section PageActions {
    <div class="col-auto ms-auto">
        <div class="btn-list">
            <a asp-action="Create" class="btn btn-primary d-none d-sm-inline-block">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M12 5l0 14" /><path d="M5 12l14 0" /></svg>
                Nuevo Empleado
            </a>
        </div>
    </div>
}

<div class="row row-cards">
    <div class="col-12">
        <div class="card">
            <div class="card-header">
                <h3 class="card-title">Lista de Empleados</h3>
            </div>
            <div class="table-responsive">
                <table class="table table-vcenter table-mobile-md card-table">
                    <thead>
                        <tr>
                            <th>Empleado</th>
                            <th>Legajo</th>
                            <th>Proyecto</th>
                            <th>Estado</th>
                            <th class="w-1">Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        @foreach (var item in Model)
                        {
                            <tr>
                                <td data-label="Empleado">
                                    <div class="d-flex py-1 align-items-center">
                                        <span class="avatar me-2">@item.Nombre.Substring(0, 1)@item.Apellido.Substring(0, 1)</span>
                                        <div class="flex-fill">
                                            <div class="font-weight-medium">@item.Nombre @item.Apellido</div>
                                            <div class="text-secondary">@item.Email</div>
                                        </div>
                                    </div>
                                </td>
                                <td data-label="Legajo">
                                    <span class="badge bg-azure-lt">@item.Legajo</span>
                                </td>
                                <td data-label="Proyecto">
                                    @item.ProyectoActual?.Nombre ?? "Sin asignar"
                                </td>
                                <td data-label="Estado">
                                    @if (item.Estado)
                                    {
                                        <span class="badge bg-success">Activo</span>
                                    }
                                    else
                                    {
                                        <span class="badge bg-danger">Inactivo</span>
                                    }
                                </td>
                                <td>
                                    <div class="btn-list flex-nowrap">
                                        <a asp-action="Edit" asp-route-id="@item.Id" class="btn btn-sm btn-icon">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M7 7h-1a2 2 0 0 0 -2 2v9a2 2 0 0 0 2 2h9a2 2 0 0 0 2 -2v-1" /><path d="M20.385 6.585a2.1 2.1 0 0 0 -2.97 -2.97l-8.415 8.385v3h3l8.385 -8.415z" /><path d="M16 5l3 3" /></svg>
                                        </a>
                                        <a asp-action="Delete" asp-route-id="@item.Id" class="btn btn-sm btn-icon text-danger">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M4 7l16 0" /><path d="M10 11l0 6" /><path d="M14 11l0 6" /><path d="M5 7l1 12a2 2 0 0 0 2 2h8a2 2 0 0 0 2 -2l1 -12" /><path d="M9 7v-3a1 1 0 0 1 1 -1h4a1 1 0 0 1 1 1v3" /></svg>
                                        </a>
                                    </div>
                                </td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</div>
```

## ✏️ Vista de Formulario (Create/Edit)

```cshtml
@model EmpleadoViewModel
@{
    Layout = "_LayoutTabler";
    ViewData["Title"] = Model.Id == 0 ? "Nuevo Empleado" : "Editar Empleado";
    ViewData["PreTitle"] = "Empleados";
}

<form asp-action="Create" method="post">
    <div class="row row-cards">
        <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">Información Personal</h3>
                </div>
                <div class="card-body">
                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="form-label required">Nombre</label>
                            <input asp-for="Nombre" class="form-control" placeholder="Ingrese el nombre" />
                            <span asp-validation-for="Nombre" class="text-danger"></span>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label required">Apellido</label>
                            <input asp-for="Apellido" class="form-control" placeholder="Ingrese el apellido" />
                            <span asp-validation-for="Apellido" class="text-danger"></span>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label required">Email</label>
                            <div class="input-group">
                                <span class="input-group-text">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 7a2 2 0 0 1 2 -2h14a2 2 0 0 1 2 2v10a2 2 0 0 1 -2 2h-14a2 2 0 0 1 -2 -2v-10z" /><path d="M3 7l9 6l9 -6" /></svg>
                                </span>
                                <input asp-for="Email" type="email" class="form-control" placeholder="Email" />
                            </div>
                            <span asp-validation-for="Email" class="text-danger"></span>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label required">Legajo</label>
                            <input asp-for="Legajo" class="form-control" placeholder="Número de legajo" />
                            <span asp-validation-for="Legajo" class="text-danger"></span>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label required">Proyecto</label>
                            <select asp-for="ProyectoId" class="form-select" asp-items="ViewBag.Proyectos">
                                <option value="">Seleccione un proyecto...</option>
                            </select>
                            <span asp-validation-for="ProyectoId" class="text-danger"></span>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Estado</label>
                            <label class="form-check form-switch">
                                <input asp-for="Estado" class="form-check-input" type="checkbox" />
                                <span class="form-check-label">Activo</span>
                            </label>
                        </div>
                    </div>
                </div>
                <div class="card-footer text-end">
                    <div class="d-flex">
                        <a asp-action="Index" class="btn btn-link">Cancelar</a>
                        <button type="submit" class="btn btn-primary ms-auto">
                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M6 4h10l4 4v10a2 2 0 0 1 -2 2h-12a2 2 0 0 1 -2 -2v-12a2 2 0 0 1 2 -2" /><path d="M12 14m-2 0a2 2 0 1 0 4 0a2 2 0 1 0 -4 0" /><path d="M14 4l0 4l-6 0l0 -4" /></svg>
                            Guardar
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</form>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

## 🔍 Vista de Detalle (Details)

```cshtml
@model Empleado
@{
    Layout = "_LayoutTabler";
    ViewData["Title"] = "Detalle del Empleado";
    ViewData["PreTitle"] = "Empleados";
}

@section PageActions {
    <div class="col-auto ms-auto">
        <div class="btn-list">
            <a asp-action="Edit" asp-route-id="@Model.Id" class="btn btn-primary">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M7 7h-1a2 2 0 0 0 -2 2v9a2 2 0 0 0 2 2h9a2 2 0 0 0 2 -2v-1" /><path d="M20.385 6.585a2.1 2.1 0 0 0 -2.97 -2.97l-8.415 8.385v3h3l8.385 -8.415z" /><path d="M16 5l3 3" /></svg>
                Editar
            </a>
        </div>
    </div>
}

<div class="row row-cards">
    <div class="col-lg-4">
        <div class="card">
            <div class="card-body text-center">
                <div class="mb-3">
                    <span class="avatar avatar-xl" style="background-image: url(~/img/tabler/social/avatar-placeholder.svg)"></span>
                </div>
                <div class="card-title mb-1">@Model.Nombre @Model.Apellido</div>
                <div class="text-secondary mb-3">@Model.Email</div>
                @if (Model.Estado)
                {
                    <span class="badge bg-success-lt">Activo</span>
                }
                else
                {
                    <span class="badge bg-danger-lt">Inactivo</span>
                }
            </div>
            <div class="d-flex">
                <a asp-action="Edit" asp-route-id="@Model.Id" class="card-btn">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon me-2"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M7 7h-1a2 2 0 0 0 -2 2v9a2 2 0 0 0 2 2h9a2 2 0 0 0 2 -2v-1" /></svg>
                    Editar
                </a>
                <a asp-action="Delete" asp-route-id="@Model.Id" class="card-btn">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon me-2 text-danger"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M4 7l16 0" /></svg>
                    Eliminar
                </a>
            </div>
        </div>
    </div>

    <div class="col-lg-8">
        <div class="card">
            <div class="card-header">
                <h3 class="card-title">Información del Empleado</h3>
            </div>
            <div class="card-body">
                <div class="datagrid">
                    <div class="datagrid-item">
                        <div class="datagrid-title">Legajo</div>
                        <div class="datagrid-content">
                            <span class="badge bg-azure-lt">@Model.Legajo</span>
                        </div>
                    </div>
                    <div class="datagrid-item">
                        <div class="datagrid-title">Nombre Completo</div>
                        <div class="datagrid-content">@Model.Nombre @Model.Apellido</div>
                    </div>
                    <div class="datagrid-item">
                        <div class="datagrid-title">Email</div>
                        <div class="datagrid-content">
                            <a href="mailto:@Model.Email">@Model.Email</a>
                        </div>
                    </div>
                    <div class="datagrid-item">
                        <div class="datagrid-title">Proyecto Actual</div>
                        <div class="datagrid-content">
                            @if (Model.ProyectoActual != null)
                            {
                                <a asp-controller="Proyectos" asp-action="Details" asp-route-id="@Model.ProyectoActual.Id">
                                    @Model.ProyectoActual.Nombre
                                </a>
                            }
                            else
                            {
                                <span class="text-secondary">Sin asignar</span>
                            }
                        </div>
                    </div>
                    <div class="datagrid-item">
                        <div class="datagrid-title">Fecha de Alta</div>
                        <div class="datagrid-content">@Model.CreatedAt.ToString("dd/MM/yyyy")</div>
                    </div>
                    <div class="datagrid-item">
                        <div class="datagrid-title">Estado</div>
                        <div class="datagrid-content">
                            @if (Model.Estado)
                            {
                                <span class="status status-green">
                                    <span class="status-dot"></span>
                                    Activo
                                </span>
                            }
                            else
                            {
                                <span class="status status-red">
                                    <span class="status-dot"></span>
                                    Inactivo
                                </span>
                            }
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="card mt-3">
            <div class="card-header">
                <h3 class="card-title">Historial de Proyectos</h3>
            </div>
            @if (Model.EmpleadoProyectos?.Any() == true)
            {
                <div class="list-group list-group-flush">
                    @foreach (var ep in Model.EmpleadoProyectos.OrderByDescending(x => x.FechaAsignacion))
                    {
                        <div class="list-group-item">
                            <div class="row align-items-center">
                                <div class="col-auto">
                                    <span class="avatar" style="background-color: #206bc4;">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M9 4h3l2 2h5a2 2 0 0 1 2 2v7a2 2 0 0 1 -2 2h-10a2 2 0 0 1 -2 -2v-9a2 2 0 0 1 2 -2" /></svg>
                                    </span>
                                </div>
                                <div class="col">
                                    <div class="text-truncate">
                                        <strong>@ep.Proyecto?.Nombre</strong>
                                    </div>
                                    <div class="text-secondary small">
                                        Desde: @ep.FechaAsignacion.ToString("dd/MM/yyyy")
                                        @if (ep.FechaFinalizacion.HasValue)
                                        {
                                            <span> - Hasta: @ep.FechaFinalizacion.Value.ToString("dd/MM/yyyy")</span>
                                        }
                                    </div>
                                </div>
                                <div class="col-auto">
                                    @if (ep.Activo)
                                    {
                                        <span class="badge bg-success">Activo</span>
                                    }
                                    else
                                    {
                                        <span class="badge bg-secondary">Finalizado</span>
                                    }
                                </div>
                            </div>
                        </div>
                    }
                </div>
            }
            else
            {
                <div class="card-body">
                    <div class="empty">
                        <div class="empty-icon">
                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M9 4h3l2 2h5a2 2 0 0 1 2 2v7a2 2 0 0 1 -2 2h-10a2 2 0 0 1 -2 -2v-9a2 2 0 0 1 2 -2" /></svg>
                        </div>
                        <p class="empty-title">Sin proyectos asignados</p>
                        <p class="empty-subtitle text-secondary">
                            Este empleado no tiene proyectos en su historial
                        </p>
                    </div>
                </div>
            }
        </div>
    </div>
</div>
```

## 🎨 Vista de Login

```cshtml
@model LoginViewModel
@{
    Layout = "_LoginLayoutTabler";
    ViewData["Title"] = "Iniciar Sesión";
}

<div class="text-center mb-4">
    <a href="." class="navbar-brand navbar-brand-autodark">
        <svg xmlns="http://www.w3.org/2000/svg" width="110" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon">
            <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
            <path d="M3 7v10a2 2 0 0 0 2 2h14a2 2 0 0 0 2 -2v-10" />
            <path d="M8 7v-4h8v4" />
            <path d="M3 7h18" />
        </svg>
    </a>
</div>

<div class="card card-md">
    <div class="card-body">
        <h2 class="h2 text-center mb-4">Iniciar Sesión</h2>
        <form asp-action="Login" method="post">
            <div class="mb-3">
                <label class="form-label">Email</label>
                <input asp-for="Email" type="email" class="form-control" placeholder="su@email.com" autocomplete="off" />
                <span asp-validation-for="Email" class="text-danger"></span>
            </div>
            <div class="mb-2">
                <label class="form-label">
                    Contraseña
                    <span class="form-label-description">
                        <a asp-action="ForgotPassword">¿Olvidó su contraseña?</a>
                    </span>
                </label>
                <div class="input-group input-group-flat">
                    <input asp-for="Password" type="password" class="form-control" placeholder="Tu contraseña" autocomplete="off" />
                    <span class="input-group-text">
                        <a href="#" class="link-secondary" data-bs-toggle="tooltip" title="Mostrar contraseña">
                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10 12a2 2 0 1 0 4 0a2 2 0 0 0 -4 0" /><path d="M21 12c-2.4 4 -5.4 6 -9 6c-3.6 0 -6.6 -2 -9 -6c2.4 -4 5.4 -6 9 -6c3.6 0 6.6 2 9 6" /></svg>
                        </a>
                    </span>
                </div>
                <span asp-validation-for="Password" class="text-danger"></span>
            </div>
            <div class="mb-2">
                <label class="form-check">
                    <input asp-for="RememberMe" type="checkbox" class="form-check-input" />
                    <span class="form-check-label">Recordarme en este dispositivo</span>
                </label>
            </div>
            <div class="form-footer">
                <button type="submit" class="btn btn-primary w-100">Ingresar</button>
            </div>
        </form>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

## 💡 Tips y Trucos

### 1. Modales
```html
<div class="modal modal-blur fade" id="modal-danger" tabindex="-1" role="dialog">
    <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
        <div class="modal-content">
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            <div class="modal-status bg-danger"></div>
            <div class="modal-body text-center py-4">
                <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-danger icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M12 9v2m0 4v.01" /><path d="M5 19h14a2 2 0 0 0 1.84 -2.75l-7.1 -12.25a2 2 0 0 0 -3.5 0l-7.1 12.25a2 2 0 0 0 1.75 2.75" /></svg>
                <h3>¿Estás seguro?</h3>
                <div class="text-secondary">Esta acción no se puede deshacer</div>
            </div>
            <div class="modal-footer">
                <div class="w-100">
                    <div class="row">
                        <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancelar</a></div>
                        <div class="col"><a href="#" class="btn btn-danger w-100" data-bs-dismiss="modal">Eliminar</a></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
```

### 2. Paginación
```html
<div class="card-footer d-flex align-items-center">
    <p class="m-0 text-secondary">Mostrando <span>1</span> a <span>10</span> de <span>50</span> registros</p>
    <ul class="pagination m-0 ms-auto">
        <li class="page-item disabled">
            <a class="page-link" href="#" tabindex="-1">
                <svg xmlns="http://www.w3.org/2000/svg" class="icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><polyline points="15 6 9 12 15 18" /></svg>
                anterior
            </a>
        </li>
        <li class="page-item active"><a class="page-link" href="#">1</a></li>
        <li class="page-item"><a class="page-link" href="#">2</a></li>
        <li class="page-item"><a class="page-link" href="#">3</a></li>
        <li class="page-item">
            <a class="page-link" href="#">
                siguiente
                <svg xmlns="http://www.w3.org/2000/svg" class="icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><polyline points="9 6 15 12 9 18" /></svg>
            </a>
        </li>
    </ul>
</div>
```

### 3. Alertas con Iconos
```html
<div class="alert alert-success" role="alert">
    <div class="d-flex">
        <div>
            <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><circle cx="12" cy="12" r="9" /><path d="M9 12l2 2l4 -4" /></svg>
        </div>
        <div>
            <h4 class="alert-title">¡Operación exitosa!</h4>
            <div class="text-secondary">Los cambios se guardaron correctamente.</div>
        </div>
    </div>
</div>
```
