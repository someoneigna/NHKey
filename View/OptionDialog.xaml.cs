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

        private void languageChosen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;

            string language = OptionViewModel.LanguageFile;

            App.Instance.SwitchLanguage(language);
        }
    }
}
