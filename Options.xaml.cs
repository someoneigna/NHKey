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

        public bool Hidden
        {
            get
            {
                return option.Hidden;
            }
            set
            {
                option.Hidden = value;
               
            }
        }

        public bool WindowsStartup
        {
            get
            {
                return option.WindowsStartup;
            }
            set
            {
                option.WindowsStartup = value;
            }
        }


        public Opciones(Options initialOptions = null)
        {
            InitializeComponent();
            
            if ( initialOptions != null )
            {
                option = initialOptions;
                initHiddenV.IsChecked = Hidden;
                windowsStartUpV.IsChecked = WindowsStartup;
            }
        }

        public Options GetOptions()
        {
            return option;
        }

        private void guardarButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void cancelarButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            option = null;
            Close();
        }

        private void initHiddenV_Checked(object sender, RoutedEventArgs e)
        {
            Hidden = !Hidden;
        }

        private void windowsStartUpV_Checked(object sender, RoutedEventArgs e)
        {
            WindowsStartup = !WindowsStartup;
        }

       

        
    }
}
