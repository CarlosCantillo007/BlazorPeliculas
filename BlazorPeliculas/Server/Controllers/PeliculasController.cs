using AutoMapper;
using BlazorPeliculas.Server.Helpers;
using BlazorPeliculas.Shared.DTOs;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPeliculas.Server.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PeliculasController : ControllerBase
    {

        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IMapper mapper;
        private readonly string contenedor = "pelicula";

        public PeliculasController(ApplicationDbContext context, IAlmacenadorArchivos almacenadorArchivos, IMapper mapper)
        {
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<HomePageDTO>> Get()
        {
            var limite = 6;

            var peliculasEnCartelera = await context.Peliculas
                .Where(x => x.EnCartelera)
                .Take(limite)
                .OrderByDescending(x => x.Lanzamiento)
                .ToListAsync();

            var fechaActual = DateTime.Today;

            var proximosEstrenos = await context.Peliculas
                .Where(x => x.Lanzamiento > fechaActual)
                .OrderBy(x => x.Lanzamiento)
                .Take(limite)
                .ToListAsync();

            return new HomePageDTO() { PeliculasEnCartelera = peliculasEnCartelera, ProximosEstrenos = proximosEstrenos };
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PeliculaVistualizarDTO>> Get(int id)
        {
            var pelicula = await context.Peliculas.Where(x => x.Id == id)
                .Include(x => x.GenerosPelicula).ThenInclude(x => x.Genero)
                .Include(x => x.PeliculaActor).ThenInclude(x => x.Persona)
                .FirstOrDefaultAsync();

            if (pelicula == null) { return NotFound(); }

            // todo: sistema de votacion
            var promedioVotos = 4;
            var votoUsuario = 5;

            pelicula.PeliculaActor = pelicula.PeliculaActor.OrderBy(x => x.Orden).ToList();

            var model = new PeliculaVistualizarDTO();
            model.Pelicula = pelicula;
            model.Generos = pelicula.GenerosPelicula.Select(x => x.Genero).ToList();
            model.Actores = pelicula.PeliculaActor.Select(x =>
            new Persona
            {
                Nombre = x.Persona.Nombre,
                Foto = x.Persona.Foto,
                Personaje = x.Personaje,
                Id = x.PersonaId
            }).ToList();

            model.PromedioVotos = promedioVotos;
            model.VotoUsuario = votoUsuario;

            return model;

        }


        //[HttpGet("filtrar")]
        //public async Task<ActionResult<List<Pelicula>>> Get([FromQuery])

        [HttpGet("actualizar/{id}")]
        public async Task<ActionResult<PeliculaActualizacionDTO>> PutGet(int id)
        {
            var peliculaActionResult = await Get(id);
            if (peliculaActionResult.Result is NotFoundResult) { return NotFound(); }

            var peliculaVisualizarDTO = peliculaActionResult.Value;
            var generosSeleccionadosIds = peliculaVisualizarDTO.Generos.Select(x => x.Id).ToList();
            var generosNoSeleccionados = await context.Generos.Where(x => !generosSeleccionadosIds.Contains(x.Id)).ToListAsync();

            var model = new PeliculaActualizacionDTO();
            model.Pelicula = peliculaVisualizarDTO.Pelicula;
            model.GenerosSeleccionados = peliculaVisualizarDTO.Generos;
            model.GenerosNoSeleccionados = generosNoSeleccionados;
            model.Actores = peliculaVisualizarDTO.Actores;
            return model;
        }



        [HttpPost]
        public async Task<ActionResult<int>> Post(Pelicula pelicula)
        {
            if (!string.IsNullOrEmpty(pelicula.Poster))
            {
                var posterPelicula = Convert.FromBase64String(pelicula.Poster);
                pelicula.Poster = await almacenadorArchivos.GuardarArchivo(posterPelicula, ".jpg", contenedor);
            }

            if (pelicula.PeliculaActor != null)
            {
                for (int i = 0; i < pelicula.PeliculaActor.Count; i++)
                {
                    pelicula.PeliculaActor[i].Orden = i + 1;
                }
            }

            context.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id;
        }


        [HttpPut]
        public async Task<ActionResult> Put(Pelicula pelicula)
        {
            var peliculaDB = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == pelicula.Id);
            if (peliculaDB == null) { return NotFound(); }

            peliculaDB = mapper.Map(pelicula, peliculaDB);
            if (!string.IsNullOrEmpty(pelicula.Poster))
            {
                var posterImagen = Convert.FromBase64String(pelicula.Poster);
                peliculaDB.Poster = await almacenadorArchivos.EditarArchivo(posterImagen, ".jpg", "peliculas", peliculaDB.Poster);
            }

            await context.Database.ExecuteSqlInterpolatedAsync($"delete from GeneroPeliculas where PeliculaId={pelicula.Id}; delete from PeliculasActores where PeliculaId={pelicula.Id}");

            if (pelicula.PeliculaActor != null)
            {
                for (int i = 0; i < pelicula.PeliculaActor.Count; i++)
                {
                    pelicula.PeliculaActor[i].Orden = i + 1;
                }
            }

            peliculaDB.PeliculaActor = pelicula.PeliculaActor;
            peliculaDB.GenerosPelicula = pelicula.GenerosPelicula;

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var peliculaBD = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);
            if (peliculaBD == null) { return NotFound(); }

            context.Remove(peliculaBD);
            context.SaveChangesAsync();
            await almacenadorArchivos.EliminarArchivo(peliculaBD.Poster, "peliculas");
            return NoContent();

        }
    }
}
