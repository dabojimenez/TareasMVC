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

//funcion para confirmar el borrado de una tarea
//callBack, es una función que se le pasa como parametro a otra función, siendo esta llamada en un contexto determinado
//esto si el usuario da en aceptar se ejecutara la funcion de aceptar y si da en cancelar, ejecutaremos la funcion cancelar
//titulo, que le va a salir al usuario
function confirmarAccion({ callBackAceptar, callBackCancelar, titulo }) {
    Swal.fire(
        {
            title: titulo || '¿Realmente deseas hacer esto?',
            icon: 'warning',
            //mostraremos el boton de cancelar
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí',
            focusConfirm: true
        }
    ).then(//funcion que se ejecutara al presiona aceptar o cancelar
        (resultado) => {
            //resultado, contiene si el usuario acepto o no
            if (resultado.isConfirmed) {
                callBackAceptar();
            } else if (callBackCancelar) {
                //el usuario preciono cancelar y si envio una funcion cancelar
                callBackCancelar();
            }
        }
    );
}

//funcion que servira para descregar archivos
function descargarArchivo(url, nombre) {
    //creramos un ancor en memoria, de forma tem,poral para decsrgar el archivo
    var link = document.createElement('a');
    link.download = nombre;
    //para que el arhcivo se coleque en otra pestana
    link.target = "_blank";
    link.href = url;
    //insertamos el ancor en el body de html
    document.body.appendChild(link);
    //le acemos click al ancor
    link.click();
    //removemos el ancor
    document.body.removeChild(link);
    //eliminamos el ancor creado
    delete link;
}