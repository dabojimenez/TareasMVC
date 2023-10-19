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

    }
}
//funcion para slavar el paso
async function manejarClickSalvarPaso(paso) {
    paso.modoEdicion(false);
    const esNuevo = paso.esNuevo();
    const idTarea = tareEditarViewModel.id;
    const data = obtenerCuerpoPeticionPaso(paso);

    if (esNuevo) {
        insertarPaso(paso, data, idTarea);
    } else {
        //actulizaremos
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
    } else {
        //manejaremos el error con el modal
        manejoErrorApi(respuesta);
    }
}