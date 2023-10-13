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
        //console.log(respuesta);

        //usamos sweetAlert
        manejoErrorApi(respuesta);
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
        manejoErrorApi(respuesta);
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

async function actualizarOrdenTareas() {
    const ids = obtenerIdsTareas();
    await enviarIdsTareasAlBackend(ids);

    //usaremos la funcion sorted, que nos permite arreglar los elementos de un arreglo usando JS
    const arregloOrdenado = tareaListadoViewModel.tareas.sorted(
        function (a, b) {
            //los compararemos entre si
            return ids.indexOf(a.id().toString()) - ids.indexOf(b.id().toString());
        }
    );

    tareaListadoViewModel.tareas([]);
    tareaListadoViewModel.tareas(arregloOrdenado);
}
//obtener los ids de las tareas
function obtenerIdsTareas() {
    const ids = $("[name=titulo-tarea]").map(
        function () {
            //attr, el atributo que estamos buscando
            return $(this).attr("data-id");
        }
    ).get();//obtenemos el arreglo de ids
    return ids;
}

async function enviarIdsTareasAlBackend(ids) {
    //transformamos en formato json los ids de las tareas
    var data = JSON.stringify(ids);
    await fetch(`${urlTareas}/ordenar`, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
}

//funcion para modificar el cambio realizado por el usuario
async function manejarCambioEditarTarea() {
    const obj = {
        id: tareEditarViewModel.id,
        titulo: tareEditarViewModel.titulo(),
        descripcion: tareEditarViewModel.descripcion()
    }
    //si la tarea no tiene titulo no le permitiremos grabar la tarea
    if (!obj.titulo) {
        return;
    }

    await editarTareaCompleta(obj);

    //si en caso de que el web api, nos arroje un error, no se ejecutara lo siguiente
    //findIndex, nos permtie buscar por indice
    const indice = tareaListadoViewModel.tareas()
        .findIndex(t => t.id() === obj.id);
    //obtendremos la tarea en si
    const tarea = tareaListadoViewModel.tareas()[indice];
    //podemos modificar su titulo, en memoria, para que se vea que esta sincronizado
    tarea.titulo(obj.titulo);
}

//funcion que ejecutar el contralador de tareas
async function editarTareaCompleta(tarea) {
    const data = JSON.stringify(tarea);
    const respuesta = await fetch(`${urlTareas}/${tarea.id}`, {
        method: 'PUT',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!respuesta.ok) {
        manejoErrorApi(respuesta);
        //detendremos la ejecucion de cuaqluier cosa que este ocue=rriendo en la aplicación
        throw "error";
    }
}

//al finalizar la carga de la pagina, se incoara esta funcion de jquery
$(function () {
    $("#reordenable").sortable({
        //solo podremos mover en el eje Y, de arriba hacia abajo
        axis: 'y',
        //funcion que se ejecutara al finalziar de arrastara la tarea
        stop: async function () {
            await actualizarOrdenTareas();
        }
    })
})


//funcion para obtener la tarea al dar click
async function manejarClickTarea(tarea) {
    //si es una tarea nueva, simplemente no ingresa
    if (tarea.esNuevo()) {
        return;
    }
    //
    const respuesta = await fetch(
        `${urlTareas}/${tarea.id()}`,
        {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        }
    );

    if (!respuesta.ok) {
        manejoErrorApi(respuesta);
        return;
    }

    const json = await respuesta.json();

    tareEditarViewModel.id = json.id;
    tareEditarViewModel.titulo(json.titulo);
    tareEditarViewModel.descripcion(json.descripcion);

    //abriremso el modal para mostrar la ifnromación
    modalEditarTareaBootstrap.show();
}