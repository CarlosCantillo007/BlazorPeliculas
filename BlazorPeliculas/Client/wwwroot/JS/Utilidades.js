function pruebaPuntoNetStatic(){
    DotNet.invokeMethodAsync("BlazorPeliculas.Client", "ObtenerCurrectCount").then(resultado => {
        console.log("Conteo desde javascript " + resultado);
    })
}