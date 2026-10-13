// ============================================
// Sistema de Gestión de Horas Laborales
// JavaScript personalizado
// ============================================

(function () {
    'use strict';

    // ============================================
    // Inicialización al cargar el documento
    // ============================================
    document.addEventListener('DOMContentLoaded', function () {
        initializeTooltips();
        initializePopovers();
        initializeConfirmDialogs();
        initializeSidebarToggle();
        highlightActiveMenu();
    });

    // ============================================
    // Tooltips de Bootstrap
    // ============================================
    function initializeTooltips() {
        const tooltipTriggerList = [].slice.call(
            document.querySelectorAll('[data-bs-toggle="tooltip"]')
        );
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }

    // ============================================
    // Popovers de Bootstrap
    // ============================================
    function initializePopovers() {
        const popoverTriggerList = [].slice.call(
            document.querySelectorAll('[data-bs-toggle="popover"]')
        );
        popoverTriggerList.map(function (popoverTriggerEl) {
            return new bootstrap.Popover(popoverTriggerEl);
        });
    }

    // ============================================
    // Diálogos de confirmación para acciones destructivas
    // ============================================
    function initializeConfirmDialogs() {
        document.querySelectorAll('[data-confirm]').forEach(function (element) {
            element.addEventListener('click', function (e) {
                const message = this.getAttribute('data-confirm') || '¿Está seguro de realizar esta acción?';
                if (!confirm(message)) {
                    e.preventDefault();
                    e.stopPropagation();
                    return false;
                }
            });
        });
    }

    // ============================================
    // Toggle del Sidebar (móvil)
    // ============================================
    function initializeSidebarToggle() {
        const sidebarToggle = document.querySelector('[data-bs-toggle="sidebar"]');
        if (sidebarToggle) {
            sidebarToggle.addEventListener('click', function () {
                document.body.classList.toggle('sidebar-collapsed');
            });
        }
    }

    // ============================================
    // Resaltar menú activo
    // ============================================
    function highlightActiveMenu() {
        const currentPath = window.location.pathname;
        document.querySelectorAll('.navbar-nav .nav-link').forEach(function (link) {
            const href = link.getAttribute('href');
            if (href && currentPath.startsWith(href) && href !== '/') {
                link.classList.add('active');
                // Expandir el dropdown padre si existe
                const parentDropdown = link.closest('.nav-item.dropdown');
                if (parentDropdown) {
                    parentDropdown.querySelector('.dropdown-toggle')?.classList.add('active');
                }
            }
        });
    }

    // ============================================
    // Helpers globales
    // ============================================
    window.GestorNomina = {
        // Mostrar mensaje de éxito
        showSuccess: function (message, duration = 3000) {
            showAlert('success', message, duration);
        },

        // Mostrar mensaje de error
        showError: function (message, duration = 5000) {
            showAlert('danger', message, duration);
        },

        // Mostrar mensaje informativo
        showInfo: function (message, duration = 3000) {
            showAlert('info', message, duration);
        },

        // Mostrar mensaje de advertencia
        showWarning: function (message, duration = 4000) {
            showAlert('warning', message, duration);
        },

        // Confirmar acción
        confirm: function (message, callback) {
            if (confirm(message)) {
                callback();
            }
        },

        // Formatear fecha
        formatDate: function (date) {
            if (!(date instanceof Date)) {
                date = new Date(date);
            }
            return date.toLocaleDateString('es-AR', {
                year: 'numeric',
                month: '2-digit',
                day: '2-digit'
            });
        },

        // Formatear fecha y hora
        formatDateTime: function (date) {
            if (!(date instanceof Date)) {
                date = new Date(date);
            }
            return date.toLocaleDateString('es-AR', {
                year: 'numeric',
                month: '2-digit',
                day: '2-digit',
                hour: '2-digit',
                minute: '2-digit'
            });
        }
    };

    // ============================================
    // Función interna para mostrar alertas
    // ============================================
    function showAlert(type, message, duration) {
        // Buscar o crear contenedor de alertas
        let alertContainer = document.querySelector('.alert-container');
        if (!alertContainer) {
            alertContainer = document.createElement('div');
            alertContainer.className = 'alert-container position-fixed top-0 end-0 p-3';
            alertContainer.style.zIndex = '9999';
            document.body.appendChild(alertContainer);
        }

        // Crear alerta
        const alert = document.createElement('div');
        alert.className = `alert alert-${type} alert-dismissible fade show`;
        alert.setAttribute('role', 'alert');
        alert.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;

        alertContainer.appendChild(alert);

        // Auto-cerrar después de la duración especificada
        if (duration > 0) {
            setTimeout(function () {
                alert.classList.remove('show');
                setTimeout(function () {
                    alert.remove();
                }, 150);
            }, duration);
        }
    }

    // ============================================
    // Validación de formularios
    // ============================================
    const forms = document.querySelectorAll('.needs-validation');
    Array.from(forms).forEach(function (form) {
        form.addEventListener('submit', function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });

    // ============================================
    // Auto-hide alerts después de 5 segundos
    // ============================================
    document.querySelectorAll('.alert:not(.alert-permanent)').forEach(function (alert) {
        setTimeout(function () {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });

    // ============================================
    // Prevenir doble submit en formularios
    // ============================================
    document.querySelectorAll('form').forEach(function (form) {
        form.addEventListener('submit', function () {
            const submitBtn = form.querySelector('[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Procesando...';
                
                // Re-habilitar después de 5 segundos por seguridad
                setTimeout(function () {
                    submitBtn.disabled = false;
                    submitBtn.innerHTML = submitBtn.getAttribute('data-original-text') || 'Enviar';
                }, 5000);
            }
        });
    });

})();
