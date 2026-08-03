using AguasGestionCR.Interfaces;
using AguasGestionCR.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace AguasGestionCR.ViewModels
{
    public class ClientesViewModel : INotifyPropertyChanged
    {
        private readonly IClientes _clienteService;


        private string _filtroNombre;
        private string _filtroIdentificacion;
        private string _filtroMedidor;
        private string _filtroEstado = "Todos";
        private Cliente _clienteSeleccionado;

        // La lista observable que se conecta directamente al DataGrid de tu XAML
        public ObservableCollection<Cliente> ListaClientes { get; set; }

        // Propiedades públicas vinculadas a tus TextBox y ComboBox con "UpdateSourceTrigger=PropertyChanged"
        public string FiltroNombre
        {
            get => _filtroNombre;
            set { _filtroNombre = value; OnPropertyChanged(nameof(FiltroNombre)); CargarClientes(); }
        }

        public string FiltroIdentificacion
        {
            get => _filtroIdentificacion;
            set { _filtroIdentificacion = value; OnPropertyChanged(nameof(FiltroIdentificacion)); CargarClientes(); }
        }

        public string FiltroMedidor
        {
            get => _filtroMedidor;
            set { _filtroMedidor = value; OnPropertyChanged(nameof(FiltroMedidor)); CargarClientes(); }
        }

        public string FiltroEstado
        {
            get => _filtroEstado;
            set { _filtroEstado = value; OnPropertyChanged(nameof(FiltroEstado)); CargarClientes(); }
        }

        // Constructor: Recibe el servicio por inyección de dependencias
        public ClientesViewModel(IClientes clienteService)
        {
            _clienteService = clienteService;
            ListaClientes = new ObservableCollection<Cliente>();

            // Carga inicial de los datos al abrir la vista
            CargarClientes();
        }

        // Método que llama a tu servicio para consultar la base de datos con los filtros actuales
        private void CargarClientes()
        {
            var resultados = _clienteService.ObtenerClientes(FiltroNombre, FiltroIdentificacion, FiltroMedidor, FiltroEstado);

            ListaClientes.Clear();
            foreach (var cliente in resultados)
            {
                ListaClientes.Add(cliente);
            }
        }
        // Esta propiedad se conectará con el DataGrid
        public Cliente ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set
            {
                _clienteSeleccionado = value;
                OnPropertyChanged(nameof(ClienteSeleccionado));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}