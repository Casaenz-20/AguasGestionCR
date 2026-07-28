using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AguasGestionCR
{
    /// <summary>
    /// Interaction logic for Clientes.xaml
    /// </summary>
    public partial class Clientes : Window
    {
        public Clientes()
        {
            InitializeComponent();
        }
        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Editar");
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Eliminar");
        }
    }
}
