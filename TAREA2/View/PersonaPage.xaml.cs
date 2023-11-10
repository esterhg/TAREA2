namespace TAREA2
{
    using Microsoft.Maui.Controls;
    using TAREA2.ViewModels;

    public partial class PersonaPage : ContentPage
    {
        public PersonaPage(PersonaViewModel viewModel)
        {
            InitializeComponent();

            viewModel.ImgFotoControl = imgFoto; 

            BindingContext = viewModel;
        }
    }
}
