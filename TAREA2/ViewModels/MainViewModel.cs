using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using   TAREA2.DataAccess;
using TAREA2.Dtos;
using TAREA2.Utilidades;

namespace TAREA2.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly PDbContext _dbContext;
        [ObservableProperty]
        private ObservableCollection<PersonaDTO> listaPersona = new ObservableCollection<PersonaDTO>();

        public MainViewModel(PDbContext context)
        {
            _dbContext = context;

            MainThread.BeginInvokeOnMainThread(new Action(async () => await Obtener()));

            WeakReferenceMessenger.Default.Register<PersonaMensajeria>(this, (r, m) =>
            {
                PersonaMensajeRecibido(m.Value);
                ActualizarLista();
            });
        }
        private async void ActualizarLista()
        {
            ListaPersona.Clear();
            await Obtener();
        }
        public async Task Obtener()
        {


            var lista = await _dbContext.Personas.ToListAsync();
            if (lista.Any())
            {

                foreach (var item in lista)
                {
                    ListaPersona.Add(new PersonaDTO
                    {

                        IdPersona = item.IdPersona,
                        Nombre = item.Nombre,
                        Descripcion = item.Descripcion,
                        RutaImagen = item.RutaImagen

                    });
                }
            }

        }


        private void PersonaMensajeRecibido(PersonaMensaje PersonaMensaje)
        {
            var PersonaDto = PersonaMensaje.PersonaDto;

            if (PersonaMensaje.EsCrear)
            {
                ListaPersona.Add(PersonaDto);
            }
            else
            {
                var encontrado = ListaPersona
                    .First(e => e.IdPersona == PersonaDto.IdPersona);

                encontrado.Nombre = PersonaDto.Nombre;
                encontrado.Descripcion = PersonaDto.Descripcion;
                encontrado.RutaImagen  = PersonaDto.RutaImagen;

            }

        }

        [RelayCommand]
        public async Task Crear()
        {
            try
            {
                var uri = $"{nameof(PersonaPage)}?id=0";
                await Shell.Current.GoToAsync(uri);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al navegar a PersonaPage: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task Editar(PersonaDTO PersonaDto)
        {
            var uri = $"{nameof(PersonaPage)}?id={PersonaDto.IdPersona}";
            await Shell.Current.GoToAsync(uri);
        }

        [RelayCommand]
        public async Task Eliminar(PersonaDTO PersonaDto)
        {
            bool answer = await Shell.Current.DisplayAlert("Mensaje", "Desea eliminar el Persona?", "Si", "No");

            if (answer)
            {
                var encontrado = await _dbContext.Personas
                    .FirstAsync(e => e.IdPersona == PersonaDto.IdPersona);

                _dbContext.Personas.Remove(encontrado);
                await _dbContext.SaveChangesAsync();
                ListaPersona.Remove(PersonaDto);

            }

        }


    }
}

