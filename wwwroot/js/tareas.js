function agregarNuevaTareaAlListado() {
    tareasListadoViewModel.tareas.push(tareaElementoListadoViewModel({ id: 0, titulo: 'qqqqq' }));
}

function manejarFocusoutTituloTarea(tarea) {
    //debugger;
    const titulo = tarea.titulo();
    //const titulo = 'QUEMADO';
    //si no escribio nada se le mostrara
    if (!titulo) {
        //removemso el ultimo elemento
        tareasListadoViewModel.tareas.pop();
        return;
    }

    tarea.id(1);
}