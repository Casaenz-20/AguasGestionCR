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
using System.Net;
using System.Net.Mail;
using AguasGestionCR.Services;

namespace AguasGestionCR
{
    /// <summary>
    /// Lógica de interacción para ReportView.xaml
    /// </summary>
    public partial class ReportView : Window
    {
        public ReportView()
        {
            InitializeComponent();
        }

       

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (UsuarioSesion.Rol == "Usuario")
            {
                txtCorreo.Text = "aguagestioncr@gmail.com";
                txtCorreo.IsReadOnly = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
