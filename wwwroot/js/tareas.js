function agregarNuevaTareaAlListado() {
    tareaListadoViewModel.tareas.push(
        new tareaElementoListadoViewModel({ id: 0, titulo: '' })
    );
}

function agregarTarea(tarea) {
    console.log(tarea);
}

async function manejarFocusoutTituloTarea(tarea) {
    const titulo = tarea.titulo();

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
        console.log(respuesta);
    }
}

//obtencionde tareas al inicio de la aplicación
async function obtenerTareas() {
    //inicializamos el spiner de cargando
    tareaListadoViewModel.cargando(true);

    const respuesta = await fetch(
        urlTareas,
        {
            method: 'GET',
            Headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            }
        }
    );

    if (!respuesta.ok) {
        return;
    }
    //obtenemos en formato json, la respuesta
    const json = await respuesta.json();
    //inicializamos las tareas ocn un arreglo vacio
    tareaListadoViewModel.tareas([]);

    //recorremos el json, con un foreach
    json.forEach(valor => {
        tareaListadoViewModel.tareas.push(
            new tareaElementoListadoViewModel(valor)
        );
    });
    //finalizamos el spiner de acrgando
    tareaListadoViewModel.cargando(false);
}
