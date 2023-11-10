using CommunityToolkit.Mvvm.ComponentModel;

namespace TAREA2.Dtos
{
    public partial class PersonaDTO : ObservableObject
    {
        [ObservableProperty]
        public int idPersona;
        [ObservableProperty]
        public string nombre;
        [ObservableProperty]
        public string descripcion;
        [ObservableProperty]
        public string rutaImagen;


    }
}
