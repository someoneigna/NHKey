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
using NHkey.Model;

namespace NHkey.View
{
    /// <summary>
    /// Handles the program options.
    /// </summary>
    public partial class OptionDialog : Window
    {
        public Options OptionViewModel { get; protected set; }

        public OptionDialog(Options initialOptions)
        {
            InitializeComponent();
            OptionViewModel = initialOptions;
            DataContext = OptionViewModel;
        }

        public OptionDialog()
            : this(new Options())
        {
        }

        #region Save and Cancel  // Removed for now
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
        #endregion

        private void languageChosen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResourceDictionary dictionary = new ResourceDictionary();
            var combobox = sender as ComboBox;

            string language = OptionViewModel.LanguageFile;

            App.Instance.SwitchLanguage(language);
        }
    }
}
