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

