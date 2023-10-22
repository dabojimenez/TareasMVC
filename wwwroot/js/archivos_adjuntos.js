// guardamos una referencia al arhivoTarea
let inputArchivoTarea = document.getElementById('archivoATarea');

function manejarClickAgregarArchivoAdjunto() {
    //aun que el input type="file", este oculto vamos a pdoer interactuar ocn el mismo gracias a la referencia realizada
    inputArchivoTarea.click();
}

async function manejarSeleccionArchivoTarea(event) {
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
    //colocamos en memoria los archivos
    prepararArchivosAdjuntos(json);

    inputArchivoTarea.value = null;
}

//procesamiento de lsoa rhcivos adjuntos para mostrar
function prepararArchivosAdjuntos(archivosAdjuntos) {
    
    archivosAdjuntos.forEach(archivoAdjunto => {
        let fechaCreación = archivoAdjunto.fechaCreacion;
        //si no contienen la Z, es porque no se esta expresdnaod como utc, pero en el sertvidor estamos suando utcnow, por tanto debemos colocar la z
        if (archivoAdjunto.fechaCreacion.indexOf('Z') === -1) {
            //LE AGREGAMOS LA Z
            fechaCreación += 'Z';
        }
        const fechaCreacionDT = new Date(fechaCreación);
        archivoAdjunto.publicado = fechaCreacionDT.toLocaleString();

        tareEditarViewModel.archivosAdjuntos.push(new archivoAdjuntoViewModel({ ...archivoAdjunto, modoEdicion: false }))
    });
}