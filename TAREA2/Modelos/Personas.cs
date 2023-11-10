using System.ComponentModel.DataAnnotations;
namespace TAREA2.Modelos
{
    public class Personas
    {
        [Key]
        public int IdPersona { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string RutaImagen { get; set; }

    }
}
