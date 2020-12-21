using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPeliculas.Client.Helpers
{
    public interface IMostrarMensaje
    {
        Task MostrarMensajeError(string mensaje);
    }
}
