// guardamos una referencia al arhivoTarea
let inputArchivoTarea = document.getElementById('archivoATarea');

function manejarClickAgregarArchivoAdjunto() {
    //aun que el input type="file", este oculto vamos a pdoer interactuar ocn el mismo gracias a la referencia realizada
    inputArchivoTarea.click();
}