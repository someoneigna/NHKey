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

using NHkey.Model;
using NHkey.ViewModel;
using GalaSoft.MvvmLight.Command;

namespace NHkey.View
{
    /// <summary>
    /// Handles the creation and edition of hotkeys.
    /// </summary>
    public partial class HotkeyConfigDialog : Window
    {

        /// <summary>
        /// Holds the resulting <see cref="HotkeyAssociation"/>
        /// from the dialog.
        /// </summary>
        public HotkeyAssociation ResultHotkey { get; protected set; }

        public HotkeyConfigViewModel ViewModel { get; private set; }

        public HotkeyConfigDialog(HotkeyAssociation editHotkey = null)
        {
            InitializeComponent();

            if (editHotkey != null)
            {
                editHotkey = new HotkeyAssociation(editHotkey);
            }
            ViewModel = new HotkeyConfigViewModel(editHotkey);

            DataContext = ViewModel;

            combinationField.Text = ViewModel.Model.ToString();
        }

        /// <summary>
        /// Spawns a open file dialog to choose a program.
        /// </summary>
        private void searchProgramButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Title = FindResource("SearchProgramTitle") as string;
            dialog.Multiselect = false;
            dialog.ShowDialog();

            if (!string.IsNullOrEmpty(dialog.FileName))
            {
                ViewModel.ProgramFile = dialog.FileName;
            }

            combinationField.Focus();
        }

        private void combinationField_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            combinationField.Text = FindResource("SettingKeyCombinationLabel") as string; ;
            ViewModel.Bind.Key = e.Key;
            ViewModel.Bind.Modifiers = Keyboard.Modifiers;
        }

        /// <summary>
        /// When finished inserting the hotkey combination.
        /// </summary>
        private void combinationField_KeyUp(object sender, KeyEventArgs e)
        {
            ViewModel.Bind.Modifiers = Keyboard.Modifiers | ViewModel.Bind.Modifiers;
            ViewModel.Model.SetBind(ViewModel.Bind);
            combinationField.Text = ViewModel.Model.ToString();

            if (ViewModel.Model.Invalid)
            {
                InvalidHotkeyMsgBox();
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            ResultHotkey = null;
            this.Close();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ViewModel.ProgramFile))
            {
                UnfilledFieldMsgBox(FindResource("UnfilledHotkeyNameErrorMessage") as string);
                nameField.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(ViewModel.ProgramFile))
            {
                UnfilledFieldMsgBox(FindResource("UnfilledProgramPathErrorMessage") as string);
                searchProgramButton.Focus();
                return;
            }

            if (ViewModel.Model.Invalid)
            {
                InvalidHotkeyMsgBox();
                return;
            }

            ResultHotkey = ViewModel.Model;
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Show a messagebox indicating that required fields are empty.
        /// </summary>
        private void UnfilledFieldsMsgBox()
        {
            string message = FindResource("UnfilledFieldsMsgBoxMessage") as string;
            string title = FindResource("UnfilledFieldsMsgBoxTitle") as string;
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }


        /// <summary>
        /// Show a messagebox indicating that specified
        /// field is empty.
        /// </summary>
        private void UnfilledFieldMsgBox(string message)
        {
            string title = FindResource("UnfilledFieldsMsgBoxTitle") as string;
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        /// <summary>
        /// Show a messagebox indicating that key combination is invalid.
        /// </summary>
        private void InvalidHotkeyMsgBox()
        {
            string message = FindResource("InvalidHotkeyMsgBoxMessage") as string;
            string title = FindResource("InvalidHotkeyMsgBoxTitle") as string;
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

    }
}
