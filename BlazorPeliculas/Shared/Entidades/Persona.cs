using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculas.Shared.Entidades
{
    public class Persona
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        public string Nombre { get; set; }
        public string Biografia { get; set; }
        public string Foto { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public DateTime? FechaNacimiento { get; set; }
        public List<PeliculaActor> PeliculaActor { get; set; }
        [NotMapped]
        public string Personaje { get; set; }
    }
}
