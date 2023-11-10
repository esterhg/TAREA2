using TAREA2.ViewModels;
namespace TAREA2
{
  
    
        public partial class MainPage : ContentPage
        {
            int count = 0;

            public MainPage(MainViewModel viewModel)
            {
                InitializeComponent();
                BindingContext = viewModel;
            }



        }
}