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
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;
using System.ComponentModel;

namespace NHkey
{
    /// <summary>
    /// Lógica de interacción para hotkey_config.xaml
    /// </summary>
    public partial class HotkeyConfig : Window
    {
        private KeyBinding tempBind = new KeyBinding();
        
        public Hotkey ResultHotkey { get; protected set; }

        private static string GetBindString(Key key, ModifierKeys mod)
        {
            KeyBinding bind = new KeyBinding();
            bind.Key = key;
            bind.Modifiers = mod;
            return bind.ToString();
        }
        public HotkeyConfig(Hotkey editHotkey = null)
        {
            InitializeComponent();

            ResultHotkey = (editHotkey != null) ? new Hotkey(editHotkey) : new Hotkey();

            DataContext = ResultHotkey;

            ResultHotkey.Icon = NHKeyController.GetIcon(ResultHotkey.FilePath);
            //parametersField.Text = ResultHotkey.ParametersString;
        }

        
        private void searchProgramButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Title = "Elija un programa:";
            dialog.Multiselect = false;
            dialog.ShowDialog();

            if (dialog.FileName != null && dialog.FileName.Length > 0)
            {
                ResultHotkey.FilePath = dialog.FileName;
                programField.Text = ResultHotkey.FilePath.Substring(ResultHotkey.FilePath.LastIndexOf("\\") + 1);
            }

            combinationField.Focus();    
        }

        
      
        private void combinationField_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            combinationField.Text = "";
            tempBind.Key = e.Key;
            tempBind.Modifiers = Keyboard.Modifiers;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            ResultHotkey = null;
            this.Close();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ResultHotkey.Name == "" || ResultHotkey.FilePath == "" || ResultHotkey.Bind == null)
            {
                UnfilledFieldsMsgBox();
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void UnfilledFieldsMsgBox()
        {
            string message = "Deben llenarse los campos antes de continuar.";
            MessageBox.Show(message, "Campos invalidos", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void combinationField_KeyUp(object sender, KeyEventArgs e)
        {
            tempBind.Modifiers = Keyboard.Modifiers | tempBind.Modifiers;
            ResultHotkey.SwitchBind(tempBind);
            combinationField.Text = ResultHotkey.HotkeyText;
        }


        private void programField_TextChanged(object sender, TextChangedEventArgs e)
        {
            ResultHotkey.Icon = NHKeyController.GetIcon(ResultHotkey.FilePath);
            programField.Text = programField.Text.Substring(programField.Text.LastIndexOf("\\") + 1);
        }

    }
}
