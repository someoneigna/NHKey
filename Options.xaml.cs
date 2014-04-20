using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace NHkey
{
    /// <summary>
    /// Lógica de interacción para Opciones.xaml
    /// </summary>
    ///
    public partial class Opciones : Window
    {
        Options option = new Options();

        public Options OptionViewModel { get; protected set; }

        public Opciones(Options initialOptions)
        {
            InitializeComponent();
            OptionViewModel = initialOptions;
            DataContext = OptionViewModel;
        }

        public Options GetOptions()
        {
            return option;
        }

        private void guardarButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            OptionViewModel.Save();
            Close();
        }

        private void cancelarButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            OptionViewModel = null;
            Close();
        }
    }
}
