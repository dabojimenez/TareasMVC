function agregarNuevaTareaAlListado() {
    tareaListadoViewModel.tareas.push(
        tareaElementoListadoViewModel({ id: 0, titulo: '' })
    );
}

function agregarTarea(tarea) {
    console.log(tarea);
}

async function manejarFocusoutTituloTarea(tarea) {
    const titulo = tarea.titulo();
    //const titulo = 'QUEMADO';
    //si no escribio nada se le mostrara
    if (!titulo) {
        //removemso el ultimo elemento
        tareasListadoViewModel.tareas.pop();
        return;
    }

    //tarea.id(1);
    const data = JSON.stringify(titulo);
    const respuesta = await fetch(
        urlTareas,
        {
            method: 'POST',
            body: data,
            headers: {
                'Content-Type': 'application/json',
                "Accept": "application/json"
            }
        }
    );

    if (respuesta.ok) {
        const json = await respuesta.json();
        tarea.id(json.id);
    } else {
        //mostrar mensaje de error
    }
}