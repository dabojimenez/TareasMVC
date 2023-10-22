// guardamos una referencia al arhivoTarea
let inputArchivoTarea = document.getElementById('archivoATarea');

function manejarClickAgregarArchivoAdjunto() {
    //aun que el input type="file", este oculto vamos a pdoer interactuar ocn el mismo gracias a la referencia realizada
    inputArchivoTarea.click();
}

async function manejarSeleccionArchivoTarea(event) {
    debugger;
    //accedermos a los archivios
    const archivos = event.target.files;
    //realizaremos un arreglo de archivos
    const archivosArreglo = Array.from(archivos);

    const idTarea = tareEditarViewModel.id;

    const formData = new FormData();
    //iteramos nuestro arreglo de archivos
    for (var i = 0; i < archivosArreglo.length; i++) {
        //el nombre entre comillas (archivos), viene del controaldor de Archvios, del metodo POST en [ [FromForm] IEnumerable<IFormFile> ]
        formData.append("archivos", archivosArreglo[i]);
    }

    //realizaremos la peticion
    const respuesta = await fetch(`${urlArchivos}/${idTarea}`,
        {
            body: formData,
            method: 'POST'
        }
    );

    if (!respuesta.ok) {
        manejoErrorApi(respuesta);
        return;
    }

    const json = await respuesta.json();
    console.log(json);

    inputArchivoTarea.value = null;
}