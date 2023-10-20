function manejarClickAgregarPaso() {
    //si el paso es nuevo, no agregaremos dos de forma inmediata
    const indice = tareEditarViewModel.pasos().findIndex(p => p.esNuevo());
    //si es distinto de menos uno, es porque ya existe uno
    if (indice !== -1) {
        return;
    }
    tareEditarViewModel.pasos.push(new pasoViewModel({ modoEdicion: true, realizado: false }));
    //seleccionamos el texarea, y si esta visible, se le colocara el focus
    $("[name=txtPasoDescripcion]:visible").focus();
}

//funcion para cancelar
function manejarClickCancelarPaso(paso) {
    if (paso.esNuevo()) {
        //eliminamos el apso que nunca se creo
        tareEditarViewModel.pasos.pop();
    } else {
        //cuando estamos en la fase de edicion, mostraemos el texto anterior cuando cancele
        paso.modoEdicion(false);
        paso.descripcion(paso.descripcionAnterior);
    }
}
//funcion para slavar el paso
async function manejarClickSalvarPaso(paso) {
    paso.modoEdicion(false);
    const esNuevo = paso.esNuevo();
    const idTarea = tareEditarViewModel.id;
    const data = obtenerCuerpoPeticionPaso(paso);

    const descripcion = paso.descripcion();

    //validamos de que en caso de tener una descripcion vacia, no se realizara la acción
    if (!descripcion) {
        paso.descripcion(paso.descripcionAnterior);
        if (esNuevo) {
            tareEditarViewModel.pasos.pop();
        }
        return;
    }

    if (esNuevo) {
        insertarPaso(paso, data, idTarea);
    } else {
        //actulizaremos el paso. (su descripcion)
        actualizarPaso(data, paso.id());
    }
}

//funcion que creara el cuerpo en formato json, para neviar al controlador
function obtenerCuerpoPeticionPaso(paso) {
    return JSON.stringify(
        {
            descripcion: paso.descripcion(),
            realizado: paso.realizado()
        }
    );
}

//funcion que realziara la peticion http, para insertar
async function insertarPaso(paso, data, idTarea) {
    const respuesta = await fetch(`${urlPasos}/${idTarea}`,
        {
            body: data,
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        }
    );

    if (respuesta.ok) {
        const json = await respuesta.json();
        paso.id(json.id);
        //vamos a agregar un neuvo paso a pasos totales
        const tarea = obtenerTareaEnEdicion();
        tarea.pasosTotal(tarea.pasosTotal() + 1);
        //si ese paso estaba realizado
        if (paso.realizado()) {
            tarea.pasosRealizados(tarea.pasosRealizados() + 1);
        }
    } else {
        //manejaremos el error con el modal
        manejoErrorApi(respuesta);
    }
}

function manejarClickDescripcionPaso(paso) {
    paso.modoEdicion(true);
    paso.descripcionAnterior = paso.descripcion();
    $("[name=txtPasoDescripcion]:visible").focus();
}

async function actualizarPaso(data, id) {
    const respuesta = await fetch(`${urlPasos}/${id}`,
        {
            body: data,
            method: "PUT",
            headers: {
                'Content-Type': 'application/json'
            }
        }
    );

    if (!respuesta.ok) {
        manejoErrorApi(respuesta);
    }
}

function manejarClickCheckboxPaso(paso) {
    //retorno por defecto para marcar el check
    if (paso.esNuevo()) {
        return true;
    } else {
        const data = obtenerCuerpoPeticionPaso(paso);
        actualizarPaso(data, paso.id());

        const tarea = obtenerTareaEnEdicion();
        let pasosRealizadosActual = tarea.pasosRealizados();
        //verificamos si el paso esta siendo marcado o desmarcado
        if (paso.realizado()) {
            //si esta marcado, le sumamos uno
            pasosRealizadosActual++;
        } else {
            //estamso desmarcando y le restamos uno
            pasosRealizadosActual--;
        }

        tarea.pasosRealizados(pasosRealizadosActual);
    }
    return true;
}

function manejarClickBorrarPaso(paso) {
    //mostraremos la ventana de borrar
    modalEditarTareaBootstrap.hide();
    confirmarAccion(
        {
            callBackAceptar: () => {
                borrarPaso(paso);
                modalEditarTareaBootstrap.show();
            },
            callBackCancelar: () => {
                //mostraremso el modal nuevamente
                modalEditarTareaBootstrap.show();
            },
            titulo: `Desea borrar este paso => ${paso.descripcion()}?`
        }
    )
}

async function borrarPaso(paso) {
    const respuesta = await fetch(`${urlPasos}/${paso.id()}`,
        {
            method: 'DELETE'
        }
    );

    if (!respuesta.ok) {
        manejoErrorApi(respuesta);
        return;
    }
    //removemos del vm, el paso que se elimino
    tareEditarViewModel.pasos.remove(
        function (item) {
            return item.id() == paso.id()
        }
    )

    const tarea = obtenerTareaEnEdicion();
    tarea.pasosTotal(tarea.pasosTotal() - 1)
    //si el baso borrado tambien estaba realizado
    if (paso.realizado()) {
        //restaremso uno 
        tarea.pasosRealizados(tarea.pasosRealizados() - 1)
    }
}