using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPeliculas.Client.Pages
{
    public partial class Counter
    {
        [Inject] public ServiciosSingleton singleton { get; set; }
        [Inject] public ServiciosTransient transient { get; set; }
        [Inject] public IJSRuntime JS { get; set; }

        private int currentCount = 0;
        static int currentCountStatic = 0;
        private async Task IncrementCount()
        {
            currentCount++;
            singleton.Valor = currentCount;
            transient.Valor = currentCount;
            currentCountStatic++;
            await JS.InvokeVoidAsync("pruebaPuntoNetStatic");
        }


        [JSInvokable]
        public static Task<int> ObtenerCurrectCount()
        {
            return Task.FromResult(currentCountStatic);
        }
    }
}
