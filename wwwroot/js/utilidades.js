//function mostrarError() {
//    alert(mensajeDeError);
//}

//crearemos una funcion, para amnejr los errores de la api.
//Mostraremos un error deacuerdo al tipo de error que obtuvimos de la api
async function manejoErrorApi(respuesta) {
    //colocamos una variable
    let mensajeError = '';
    //si es 400, es porque puede ser un erro cualquiera, por ser un badrequest
    if (respuesta.status === 400) {
        mensajeError = await respuesta.text();
    } else if (respuesta.status === 404) {
        //mensjae de no encontrado
        mensajeError = recursoNoEncontrado;
    } else {
        //para cualqueir otro error
        mensajeError = errorInesperado;
    }

    mostrarMensajeError(mensajeError);
}

//aqui usaremos verdaderamente sweetAlert
function mostrarMensajeError(mensaje) {
    Swal.fire(
        {
            icon: 'error',
            title: 'Error...',
            text: mensaje
        }
    );
}