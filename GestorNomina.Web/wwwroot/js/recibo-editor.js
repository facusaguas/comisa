/**
 * ReciboEditor - Manejo de edición dinámica de recibos de sueldo
 */
class ReciboEditor {
    constructor(reciboId) {
        console.log('ReciboEditor constructor called with ID:', reciboId);
        this.reciboId = reciboId;
        this.modoEdicion = false;
        this.configuracion = null;
        this.valoresOriginales = new Map();
        this.init();
    }

    async init() {
        console.log('ReciboEditor init starting...');
        // Cargar configuración de edición
        await this.cargarConfiguracion();
        
        // Vincular eventos
        this.vincularEventos();
        console.log('ReciboEditor init completed');
    }

    async cargarConfiguracion() {
        try {
            const response = await fetch(`/Liquidacion/ConfiguracionEdicion?reciboId=${this.reciboId}`);
            if (!response.ok) {
                throw new Error('Error al cargar configuración de edición');
            }
            this.configuracion = await response.json();
            console.log('Configuración cargada:', this.configuracion);
        } catch (error) {
            console.error('Error cargando configuración:', error);
            this.mostrarError('No se pudo cargar la configuración de edición');
        }
    }

    vincularEventos() {
        console.log('Vinculando eventos...');
        
        // Botón activar/desactivar modo edición
        const btnEditar = document.getElementById('btnActivarEdicion');
        console.log('btnActivarEdicion encontrado:', btnEditar);
        
        if (btnEditar) {
            btnEditar.addEventListener('click', () => {
                console.log('Click en btnActivarEdicion');
                this.toggleModoEdicion();
            });
        } else {
            console.error('No se encontró el botón #btnActivarEdicion');
        }

        // Botones de editar en línea
        document.querySelectorAll('.btn-editar').forEach(btn => {
            btn.addEventListener('click', (e) => this.activarEdicionFila(e.target.closest('tr')));
        });

        // Botones de guardar
        document.querySelectorAll('.btn-guardar').forEach(btn => {
            btn.addEventListener('click', (e) => this.guardarEdicionFila(e.target.closest('tr')));
        });

        // Botones de cancelar
        document.querySelectorAll('.btn-cancelar').forEach(btn => {
            btn.addEventListener('click', (e) => this.cancelarEdicionFila(e.target.closest('tr')));
        });

        // Botones de eliminar
        document.querySelectorAll('.btn-eliminar').forEach(btn => {
            btn.addEventListener('click', (e) => this.eliminarDetalle(e.target.closest('tr')));
        });

        // Botones agregar concepto
        document.querySelectorAll('.btn-agregar-concepto').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const tipo = e.target.closest('button').dataset.tipo;
                this.abrirModalAgregarConcepto(tipo);
            });
        });

        // Modal agregar concepto
        const btnConfirmarAgregar = document.getElementById('btnConfirmarAgregarConcepto');
        if (btnConfirmarAgregar) {
            btnConfirmarAgregar.addEventListener('click', () => this.confirmarAgregarConcepto());
        }

        // Actualizar subtotal en preview del modal
        const inputUnidades = document.getElementById('inputUnidades');
        const inputValor = document.getElementById('inputValorUnitario');
        if (inputUnidades && inputValor) {
            inputUnidades.addEventListener('input', () => this.actualizarSubtotalPreview());
            inputValor.addEventListener('input', () => this.actualizarSubtotalPreview());
        }
    }

    toggleModoEdicion() {
        console.log('toggleModoEdicion called. Current mode:', this.modoEdicion);
        this.modoEdicion = !this.modoEdicion;
        console.log('New mode:', this.modoEdicion);

        const btnEditar = document.getElementById('btnActivarEdicion');
        const badgeModo = document.getElementById('badgeModoEdicion');

        console.log('btnEditar:', btnEditar);
        console.log('badgeModo:', badgeModo);

        if (this.modoEdicion) {
            // Mostrar controles de edición
            document.querySelectorAll('.acciones-edicion').forEach(el => el.classList.remove('d-none'));
            btnEditar.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" class="icon" width="16" height="16" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M5 12l5 5l10 -10" /></svg> Finalizar Edición';
            btnEditar.classList.remove('btn-primary');
            btnEditar.classList.add('btn-success');
            badgeModo.textContent = 'MODO EDICIÓN';
            badgeModo.classList.remove('bg-secondary');
            badgeModo.classList.add('bg-warning');
        } else {
            // Ocultar controles de edición
            document.querySelectorAll('.acciones-edicion').forEach(el => el.classList.add('d-none'));
            btnEditar.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" class="icon" width="16" height="16" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M4 20h4l10.5 -10.5a1.5 1.5 0 0 0 -4 -4l-10.5 10.5v4" /></svg> Editar Recibo';
            btnEditar.classList.remove('btn-success');
            btnEditar.classList.add('btn-primary');
            badgeModo.textContent = 'AUTOMÁTICO';
            badgeModo.classList.remove('bg-warning');
            badgeModo.classList.add('bg-secondary');
        }
    }

    activarEdicionFila(fila) {
        // Guardar valores originales
        const detalleId = fila.dataset.detalleId;
        const unidades = parseFloat(fila.querySelector('.valor-display').textContent);
        const valorUnitarioText = fila.querySelectorAll('.valor-display')[1].textContent.replace('$', '').replace(',', '');
        const valorUnitario = valorUnitarioText !== '-' ? parseFloat(valorUnitarioText) : null;

        this.valoresOriginales.set(detalleId, { unidades, valorUnitario });

        // Mostrar inputs, ocultar displays
        fila.querySelectorAll('.valor-display').forEach(el => el.classList.add('d-none'));
        fila.querySelectorAll('input').forEach(el => el.classList.remove('d-none'));

        // Cambiar botones
        fila.querySelector('.btn-editar').classList.add('d-none');
        fila.querySelector('.btn-guardar').classList.remove('d-none');
        fila.querySelector('.btn-cancelar').classList.remove('d-none');
        const btnEliminar = fila.querySelector('.btn-eliminar');
        if (btnEliminar) btnEliminar.classList.add('d-none');
    }

    cancelarEdicionFila(fila) {
        const detalleId = fila.dataset.detalleId;
        const originales = this.valoresOriginales.get(detalleId);

        if (originales) {
            // Restaurar valores originales
            fila.querySelector('.input-unidades').value = originales.unidades;
            if (originales.valorUnitario !== null) {
                fila.querySelector('.input-valor-unitario').value = originales.valorUnitario;
            }
        }

        // Ocultar inputs, mostrar displays
        fila.querySelectorAll('.valor-display').forEach(el => el.classList.remove('d-none'));
        fila.querySelectorAll('input').forEach(el => el.classList.add('d-none'));

        // Cambiar botones
        fila.querySelector('.btn-editar').classList.remove('d-none');
        fila.querySelector('.btn-guardar').classList.add('d-none');
        fila.querySelector('.btn-cancelar').classList.add('d-none');
        const btnEliminar = fila.querySelector('.btn-eliminar');
        if (btnEliminar) btnEliminar.classList.remove('d-none');
    }

    async guardarEdicionFila(fila) {
        const detalleId = fila.dataset.detalleId;
        const unidades = parseFloat(fila.querySelector('.input-unidades').value);
        const valorUnitarioInput = fila.querySelector('.input-valor-unitario');
        const valorUnitario = valorUnitarioInput ? parseFloat(valorUnitarioInput.value) : null;

        if (isNaN(unidades) || unidades <= 0) {
            this.mostrarError('Las unidades deben ser mayores a cero');
            return;
        }

        if (valorUnitario !== null && (isNaN(valorUnitario) || valorUnitario <= 0)) {
            this.mostrarError('El valor unitario debe ser mayor a cero');
            return;
        }

        try {
            const response = await fetch(`/Liquidacion/ModificarConcepto?reciboId=${this.reciboId}&detalleId=${detalleId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    unidades: unidades,
                    valorUnitario: valorUnitario
                })
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.mensaje || 'Error al guardar cambios');
            }

            const resultado = await response.json();
            
            // Actualizar vista con datos del servidor
            this.actualizarFilaDesdeDatos(fila, resultado);
            
            // Actualizar totales
            this.actualizarTotales(resultado);

            // Salir del modo edición de esta fila
            this.cancelarEdicionFila(fila);

            this.mostrarExito('Concepto actualizado correctamente');

        } catch (error) {
            console.error('Error guardando cambios:', error);
            this.mostrarError(error.message);
        }
    }

    async eliminarDetalle(fila) {
        const detalleId = fila.dataset.detalleId;
        const nombreConcepto = fila.querySelectorAll('td')[1].textContent;

        if (!confirm(`¿Está seguro de eliminar el concepto "${nombreConcepto}"?`)) {
            return;
        }

        try {
            const response = await fetch(`/Liquidacion/EliminarConcepto?reciboId=${this.reciboId}&detalleId=${detalleId}`, {
                method: 'DELETE'
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.mensaje || 'Error al eliminar concepto');
            }

            const resultado = await response.json();

            // Eliminar fila con animación
            fila.style.transition = 'opacity 0.3s';
            fila.style.opacity = '0';
            setTimeout(() => {
                fila.remove();
                this.actualizarTotales(resultado);
            }, 300);

            this.mostrarExito('Concepto eliminado correctamente');

        } catch (error) {
            console.error('Error eliminando concepto:', error);
            this.mostrarError(error.message);
        }
    }

    abrirModalAgregarConcepto(tipo) {
        const modal = new bootstrap.Modal(document.getElementById('modalAgregarConcepto'));
        const select = document.getElementById('selectConcepto');
        const tipoInput = document.getElementById('tipoConceptoAgregar');

        tipoInput.value = tipo;

        // Limpiar y poblar el select
        select.innerHTML = '<option value="">Seleccione un concepto...</option>';

        if (this.configuracion && this.configuracion.conceptosDisponibles) {
            const conceptos = this.configuracion.conceptosDisponibles.filter(c => c.tipo === tipo);
            conceptos.forEach(concepto => {
                const option = document.createElement('option');
                option.value = concepto.id;
                option.textContent = `${concepto.codigo} - ${concepto.nombre}`;
                option.dataset.valorBase = concepto.valorBase || 0;
                select.appendChild(option);
            });
        }

        // Limpiar campos
        document.getElementById('inputUnidades').value = 1;
        document.getElementById('inputValorUnitario').value = '';
        document.getElementById('subtotalPreview').textContent = '$0.00';

        // Actualizar valor al seleccionar concepto
        select.addEventListener('change', function() {
            const selectedOption = this.options[this.selectedIndex];
            if (selectedOption.dataset.valorBase) {
                document.getElementById('inputValorUnitario').value = selectedOption.dataset.valorBase;
            }
        }, { once: true });

        modal.show();
    }

    async confirmarAgregarConcepto() {
        const tipo = document.getElementById('tipoConceptoAgregar').value;
        const conceptoId = parseInt(document.getElementById('selectConcepto').value);
        const unidades = parseFloat(document.getElementById('inputUnidades').value);
        const valorUnitario = parseFloat(document.getElementById('inputValorUnitario').value);

        if (!conceptoId) {
            this.mostrarError('Debe seleccionar un concepto');
            return;
        }

        if (isNaN(unidades) || unidades <= 0) {
            this.mostrarError('Las unidades deben ser mayores a cero');
            return;
        }

        if (isNaN(valorUnitario) || valorUnitario <= 0) {
            this.mostrarError('El valor unitario debe ser mayor a cero');
            return;
        }

        try {
            const response = await fetch(`/Liquidacion/AgregarConcepto?reciboId=${this.reciboId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    conceptoId: conceptoId,
                    unidades: unidades,
                    valorUnitario: valorUnitario
                })
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.mensaje || 'Error al agregar concepto');
            }

            const resultado = await response.json();

            // Cerrar modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('modalAgregarConcepto'));
            modal.hide();

            // Recargar página para mostrar el nuevo concepto
            // En una implementación más sofisticada, podríamos agregar la fila dinámicamente
            this.mostrarExito('Concepto agregado correctamente. Recargando...');
            setTimeout(() => {
                window.location.reload();
            }, 1000);

        } catch (error) {
            console.error('Error agregando concepto:', error);
            this.mostrarError(error.message);
        }
    }

    actualizarSubtotalPreview() {
        const unidades = parseFloat(document.getElementById('inputUnidades').value) || 0;
        const valorUnitario = parseFloat(document.getElementById('inputValorUnitario').value) || 0;
        const subtotal = unidades * valorUnitario;
        document.getElementById('subtotalPreview').textContent = '$' + subtotal.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    }

    actualizarFilaDesdeDatos(fila, datos) {
        // Actualizar displays
        const displays = fila.querySelectorAll('.valor-display');
        displays[0].textContent = datos.unidades.toFixed(2);
        if (datos.valorUnitario !== null) {
            displays[1].textContent = '$' + datos.valorUnitario.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        }
        fila.querySelector('.subtotal').textContent = '$' + datos.subtotal.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');

        // Marcar como editado
        if (datos.editadoManualmente && !fila.classList.contains('detalle-editado')) {
            fila.classList.add('detalle-editado');
            const codigoCelda = fila.querySelector('td:first-child');
            if (!codigoCelda.querySelector('.badge')) {
                codigoCelda.innerHTML += ' <span class="badge bg-warning ms-1" title="Editado manualmente"><svg xmlns="http://www.w3.org/2000/svg" class="icon icon-sm" width="16" height="16" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M4 20h4l10.5 -10.5a1.5 1.5 0 0 0 -4 -4l-10.5 10.5v4" /></svg></span>';
            }
        }
    }

    actualizarTotales(resultado) {
        if (!resultado || !resultado.totales) return;

        const totales = resultado.totales;

        // Actualizar subtotales por tipo (en la tabla unificada)
        const totalRemunerativo = document.getElementById('totalRemunerativo');
        if (totalRemunerativo) {
            totalRemunerativo.textContent = '$' + totales.totalRemunerativo.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        }

        const totalNoRemunerativo = document.getElementById('totalNoRemunerativo');
        if (totalNoRemunerativo) {
            totalNoRemunerativo.textContent = '$' + totales.totalNoRemunerativo.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        }

        const totalHaberes = document.getElementById('totalHaberes');
        if (totalHaberes) {
            const sumaHaberes = totales.totalRemunerativo + totales.totalNoRemunerativo;
            totalHaberes.textContent = '$' + sumaHaberes.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        }

        const totalDeducciones = document.getElementById('totalDeducciones');
        if (totalDeducciones) {
            totalDeducciones.textContent = '$' + totales.totalDeducciones.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        }

        // Actualizar totales finales
        document.getElementById('totalBrutoFinal').textContent = '$' + totales.totalBruto.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        document.getElementById('totalDeduccionesFinal').textContent = '$' + totales.totalDeducciones.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        document.getElementById('totalNetoFinal').textContent = '$' + totales.totalNeto.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    }

    mostrarError(mensaje) {
        // Usar toast de Tabler si está disponible, sino alert
        if (typeof Tabler !== 'undefined' && Tabler.toast) {
            Tabler.toast.error(mensaje);
        } else {
            alert('Error: ' + mensaje);
        }
    }

    mostrarExito(mensaje) {
        // Usar toast de Tabler si está disponible, sino console
        if (typeof Tabler !== 'undefined' && Tabler.toast) {
            Tabler.toast.success(mensaje);
        } else {
            console.log('Éxito: ' + mensaje);
        }
    }
}

// Inicializar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function() {
    console.log('recibo-editor.js loaded');
    const reciboIdInput = document.getElementById('reciboId');
    console.log('reciboIdInput:', reciboIdInput);
    
    if (reciboIdInput) {
        const reciboId = parseInt(reciboIdInput.value);
        console.log('Initializing ReciboEditor with ID:', reciboId);
        window.reciboEditor = new ReciboEditor(reciboId);
    } else {
        console.error('No se encontró el elemento #reciboId');
    }
});
