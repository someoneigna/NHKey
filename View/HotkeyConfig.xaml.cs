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

namespace NHkey.View
{
    /// <summary>
    /// Handles the creation and edition of hotkeys.
    /// </summary>
    public partial class HotkeyConfigDialog : Window
    {
        private KeyBinding tempBind = new KeyBinding();

        public HotkeyAssociation ResultHotkey { get; protected set; }

        public HotkeyConfigDialog(HotkeyAssociation editHotkey = null)
        {
            InitializeComponent();

            ResultHotkey = (editHotkey != null) ? new HotkeyAssociation(editHotkey) : new HotkeyAssociation();

            DataContext = ResultHotkey;

            // Fill icon with hotkey path (in case of editing a valid hotkey)
            if (ResultHotkey.FilePath != null)
            {
                programField.Text = ResultHotkey.FilePath.Substring(ResultHotkey.FilePath.LastIndexOf("\\") + 1);
                if (ResultHotkey.Icon == null)
                    ResultHotkey.Icon = Helpers.BitmapHelper.GetIcon(ResultHotkey.FilePath);
                
            }
            combinationField.Text = ResultHotkey.ToString();
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
                ResultHotkey.FilePath = dialog.FileName;
                ResultHotkey.Icon = Helpers.BitmapHelper.GetIcon(ResultHotkey.FilePath);

                ResultHotkey.Orphaned = !File.Exists(ResultHotkey.FilePath);

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

        /// <summary>
        /// When finished inserting the hotkey combination.
        /// </summary>
        private void combinationField_KeyUp(object sender, KeyEventArgs e)
        {
            tempBind.Modifiers = Keyboard.Modifiers | tempBind.Modifiers;
            ResultHotkey.SetBind(tempBind);
            combinationField.Text = ResultHotkey.ToString();

            if (ResultHotkey.Invalid)
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
            if (string.IsNullOrEmpty(ResultHotkey.Name) || ResultHotkey.FilePath == "")
            {
                UnfilledFieldsMsgBox();
                return;
            }

            if (ResultHotkey.Invalid)
            {
                InvalidHotkeyMsgBox();
                return;
            }

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
