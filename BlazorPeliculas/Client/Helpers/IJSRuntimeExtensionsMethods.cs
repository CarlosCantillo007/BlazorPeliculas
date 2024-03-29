﻿using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPeliculas.Client.Helpers
{
    public static class IJSRuntimeExtensionsMethods
    {
        public static async ValueTask<bool> Confirme(this IJSRuntime js, string mensaje)
        {
            await js.InvokeVoidAsync("console.log","prueba de console log");
            return await js.InvokeAsync<bool>("confirm", mensaje);
        }
    }
}
