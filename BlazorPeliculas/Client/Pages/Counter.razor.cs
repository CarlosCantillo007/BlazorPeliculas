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
        [Inject] public IJSRuntime JS { get; set; }

        IJSObjectReference modulo;

        private int currentCount = 0;
        static int currentCountStatic = 0;

        [JSInvokable]
        public async Task IncrementCount()
        {
            modulo = await JS.InvokeAsync<IJSObjectReference>("import", "./js/Counter.js");
            await modulo.InvokeVoidAsync("mostrarAlerta", "Hola mundo");

            currentCount++;
            currentCountStatic++;
            await JS.InvokeVoidAsync("pruebaPuntoNetStatic");
        }

        protected async Task IncrementCountJavaScript()
        {
            await JS.InvokeVoidAsync("pruebaPunteNETInstancia", DotNetObjectReference.Create(this));
        }


        [JSInvokable]
        public static Task<int> ObtenerCurrectCount()
        {
            return Task.FromResult(currentCountStatic);
        }
    }
}
