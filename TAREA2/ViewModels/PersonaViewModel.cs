using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls; 
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TAREA2.DataAccess;
using TAREA2.Dtos;
using TAREA2.Modelos;
using TAREA2.Utilidades;

namespace TAREA2.ViewModels
{
    public partial class PersonaViewModel : ObservableObject, IQueryAttributable
    {
        private readonly PDbContext _dbContext;

        [ObservableProperty]
        private PersonaDTO personaDTO = new PersonaDTO();

        [ObservableProperty]
        private string tituloPagina;

        private int IdPersona;
        private FileResult _imgFoto;

        public FileResult ImgFoto
        {
            get { return _imgFoto; }
            set { SetProperty(ref _imgFoto, value); }
        }

        [ObservableProperty]
        private bool loadingEsVisible = false;

        private Image imgFotoControl;

        public Image ImgFotoControl
        {
            get { return imgFotoControl; }
            set { SetProperty(ref imgFotoControl, value); }
        }

        public PersonaViewModel(PDbContext context)
        {
            _dbContext = context;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var id = int.Parse(query["id"].ToString());
            IdPersona = id;

            if (IdPersona == 0)
            {
                TituloPagina = "Nueva Persona";
            }
            else
            {
                TituloPagina = "Editar Persona";
                LoadingEsVisible = true;
                await Task.Run(async () =>
                {
                    var encontrado = await _dbContext.Personas.FirstAsync(e => e.IdPersona == IdPersona);
                    PersonaDTO.IdPersona = encontrado.IdPersona;
                    PersonaDTO.Nombre = encontrado.Nombre;
                    PersonaDTO.Descripcion = encontrado.Descripcion;

                    MainThread.BeginInvokeOnMainThread(() => { LoadingEsVisible = false; });
                });
            }
        }

        [RelayCommand]
        public async Task TomarFoto()
        {
            ImgFoto = await MediaPicker.CapturePhotoAsync();

            if (ImgFoto != null)
            {
                var memoriaStream = await ImgFoto.OpenReadAsync();
                ImgFotoControl.Source = ImageSource.FromStream(() => memoriaStream);
            }
        }

        [RelayCommand]
        private async Task Guardar()
        {
            LoadingEsVisible = true;
            PersonaMensaje mensaje = new PersonaMensaje();

            await Task.Run(async () =>
            {
                if (IdPersona == 0)
                {
                    var tbPersona = new Personas
                    {
                        Nombre = PersonaDTO.Nombre,
                        Descripcion = PersonaDTO.Descripcion,
                    };

                    _dbContext.Personas.Add(tbPersona);
                    await _dbContext.SaveChangesAsync();

                    PersonaDTO.IdPersona = tbPersona.IdPersona;

                    if (ImgFoto != null)
                    {
                        var rutaImagen = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"imagen_{PersonaDTO.IdPersona}.jpg");

                        using (var stream = await ImgFoto.OpenReadAsync())
                        using (var fileStream = File.Create(rutaImagen))
                        {
                            await stream.CopyToAsync(fileStream);
                        }

                        tbPersona.RutaImagen = rutaImagen;
                        await _dbContext.SaveChangesAsync();
                    }


                    mensaje = new PersonaMensaje()
                    {
                        EsCrear = true,
                        PersonaDto = PersonaDTO
                    };
                }
                else
                {
                    var encontrado = await _dbContext.Personas.FirstAsync(e => e.IdPersona == IdPersona);
                    encontrado.Nombre = PersonaDTO.Nombre;
                    encontrado.Descripcion = PersonaDTO.Descripcion;



                    await _dbContext.SaveChangesAsync();

                    mensaje = new PersonaMensaje()
                    {
                        EsCrear = false,
                        PersonaDto = PersonaDTO
                    };
                }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    LoadingEsVisible = false;
                    WeakReferenceMessenger.Default.Send(new PersonaMensajeria(mensaje));
                    await Shell.Current.Navigation.PopAsync();
                });
            });
        }
    }
}