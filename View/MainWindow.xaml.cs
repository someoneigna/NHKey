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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Interop;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Runtime.Serialization;
using Microsoft.Win32;
using System.Windows.Shell;

using NHkey.Model;
using NHkey.ViewModel;
using GalaSoft.MvvmLight.Command;
using NHkey.Data;
using System.ComponentModel;

namespace NHkey.View
{
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon MyNotifyIcon;

        public static RelayCommand EditHotkey { get; protected set; }
        public static RelayCommand DeleteHotkey { get; protected set; }
        public static RelayCommand AddHotkey { get; protected set; }

        // Menu commands
        public static RelayCommand ExportHotkey { get; protected set; }
        public static RelayCommand ImportHotkey { get; protected set; }

        public static MainWindowViewModel ViewModel { get; protected set; }

        public MainWindow()
        {
            MyNotifyIcon = new System.Windows.Forms.NotifyIcon();

            /*Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/NHkey;component/View/Resources/taskbar_icon.ico")).Stream;
            MyNotifyIcon.Icon = new System.Drawing.Icon(iconStream);
            iconStream.Dispose();*/

            MyNotifyIcon.Icon = System.Drawing.SystemIcons.Application;
            MyNotifyIcon.BalloonTipTitle = "Minimizado";
            MyNotifyIcon.BalloonTipText = "Para reabrir haga doble clic.";
            
            MyNotifyIcon.DoubleClick +=
            delegate(object sender, EventArgs args)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };

            InitializeComponent();

            ViewModel = new MainWindowViewModel();

            DataContext = this;

            EditHotkey = new RelayCommand(() => EditSelectedItem(), () => hotkeyList.SelectedItem != null);
            editButton.Command = EditHotkey;

            DeleteHotkey = new RelayCommand(() => RemoveSelectedItem(), () => hotkeyList.SelectedItem != null);
            deleteButton.Command = DeleteHotkey;

            AddHotkey = new RelayCommand(() => AddItem());
            addButton.Command = AddHotkey;

            ImportHotkey = new RelayCommand(() => ImportHotkeyDialog());

            ExportHotkey = new RelayCommand(() => ExportHotkeyDialog());

            SwitchLanguage(ViewModel.CurrentOptions.LanguageFile);

        }

        /// <summary>
        /// Checks the program for hotkeys that still exist and marks their
        /// name if they dont.
        /// </summary>
        private void MarkOrphanedHotkeys()
        {
            string orphanedHotkeyLabel = FindResource("OrphanedHotkeyLabel") as string;
            foreach (var hotkey in ViewModel.Hotkeys.Values.ToList())
            {
                if (!File.Exists(hotkey.FilePath))
                {
                    // Then it's invalid (orphaned)
                    hotkey.Name += " - " + orphanedHotkeyLabel;
                }
            }
        }

        /// <summary>
        /// Changes the current language merged dictionaries for the chosen language.
        /// </summary>
        /// <param name="language">The ending indicating language of the resource dictionaries.</param>
        private void SwitchLanguage(string language)
        {
            App.Instance.SwitchLanguage(language);
        }

        /// <summary>
        /// Spawns a dialog to select hotkey export location.
        /// </summary>
        private void ExportHotkeyDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = FindResource("ExportHotkeyTitle") as string;
            dialog.InitialDirectory = Directory.GetCurrentDirectory();
            dialog.ShowDialog();

            if (!string.IsNullOrEmpty(dialog.FileName))
            {
                ViewModel.ExportHotkeys(dialog.FileName);
            }
        }

        /// <summary>
        /// Spawns a dialog to select hotkey import file.
        /// </summary>
        private void ImportHotkeyDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Title = FindResource("ImportHotkeyTitle") as string;
            dialog.InitialDirectory = Directory.GetCurrentDirectory();
            dialog.ShowDialog();

            if (!string.IsNullOrEmpty(dialog.FileName))
            {
                ViewModel.ImportHotkeys(dialog.FileName);

                MarkOrphanedHotkeys();

                hotkeyList.Items.Refresh();
                hotkeyList.InvalidateVisual();
            }
        }

        #region Command actions
        private void EditSelectedItem()
        {
            var hotkey = (hotkeyList.SelectedItem as KeyValuePair<int, HotkeyAssociation>?).Value.Value;
            SpawnHotkeyConfig(hotkey);
        }

        /// <summary>
        /// Called when Remove button pressed.
        /// </summary>
        private void RemoveSelectedItem()
        {
            var hotkey = GetSelectedHotkey(hotkeyList);
            ViewModel.RemoveHotkey(hotkey);
            hotkeyList.Items.Refresh();
        }

        /// <summary>
        /// Called when Add button pressed.
        /// </summary>
        private void AddItem()
        {
            SpawnHotkeyConfig();
        }
        #endregion

        /// <summary>
        /// Handles minimizing the window and the NotifyIcon state.
        /// </summary>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                MyNotifyIcon.Visible = true;
                MyNotifyIcon.ShowBalloonTip(300);

            }
            else if (this.WindowState == WindowState.Normal)
            {
                MyNotifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
        }

        /// <summary>
        /// Calls the hotkey creation/edition dialog and interacts with the ViewModel.
        /// </summary>
        /// <param name="param">A <see cref="HotkeyAssociation"/> or a filepath (dropped from a file into the window).</param>
        public void SpawnHotkeyConfig(object param = null)
        {
            bool success = false, drop = false;
            HotkeyConfigDialog dialog = null;
            HotkeyAssociation holder = null;
            string filepath = null;

            if (param is string)
            {
                filepath = param as string;
                holder = new HotkeyAssociation() { FilePath = filepath };
                drop = true;
            }
            else if (param is HotkeyAssociation)
            {
                holder = param as HotkeyAssociation;
            }

            while (success == false)
            {
                dialog = new HotkeyConfigDialog(holder);  // New copy done in HotkeyConfig

                dialog.Owner = this;
                dialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                dialog.ShowDialog();

                // Creation dialog returned a result
                if (dialog.DialogResult == true)
                {
                    HotkeyAssociation dialogResultHotkey = dialog.ResultHotkey;
                    if (drop == true) // Don't use old key it's just a container, this happens when dropping a file.
                    {
                        holder = null;
                    }
                    success = ViewModel.AddOrUpdateHotkey(dialogResultHotkey, holder);
                }
                else { success = true; } // Dialog canceled no need to check hotkey

                // Show a message box indicating error and retry with the dialog result.
                if (!success)
                {
                    System.Windows.MessageBox.Show("No se puede asignar la misma combinacion!");
                    holder = dialog.ResultHotkey;
                }
            }
            // The hotkey list is bind to a dictionary, so we have to refresh.
            hotkeyList.Items.Refresh();
        }

        /// <summary>
        /// Handles the ListBox Multiple selection mode, to made it single select but allow unselecting all.
        /// </summary>
        private void hotkeyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ListBox list = sender as System.Windows.Controls.ListBox;

            if (e.AddedItems.Count > 1)
            {
                var first = e.AddedItems[0];
                list.SelectedItems.Clear();
                list.SelectedItems.Add(first);
            }
            else if (e.AddedItems.Count > 0 && list.SelectedItems.Count > 1)
            {
                list.SelectedItems.Clear();
                list.SelectedItems.Add(e.AddedItems[0]);
            }
        }

        /// <summary>
        /// Executes the program associated with the current clicked <see cref="HotkeyAssociation"/>.
        /// </summary>
        private void stackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 )
            {
                if (hotkeyList.SelectedItem != null)
                {
                    HotkeyAssociation hk = GetSelectedHotkey(hotkeyList);
                    if (hk != null)
                    {
                        hotkeyList.SelectedItems.Clear();
                        ViewModel.Execute(hk);
                    }
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Creates the option dialog, handles the program starting options.
        /// </summary>
        private void options_Click(object sender, EventArgs args)
        {
            OptionDialog dialog = new OptionDialog(ViewModel.CurrentOptions);
            dialog.Owner = this;
            dialog.ShowDialog();

            if (dialog.DialogResult.Equals(true))
            {
                ViewModel.CurrentOptions = dialog.OptionViewModel;
            }

            dialog.Close();
        }

        #region Helpers
        private static HotkeyAssociation GetSelectedHotkey(System.Windows.Controls.ListBox list)
        {
            return (list != null && list.SelectedItem != null) ? ((KeyValuePair<int, HotkeyAssociation>)list.SelectedItem).Value : null;
        }

        private static int GetSelectedKey(System.Windows.Controls.ListBox list)
        {
            return (list != null && list.SelectedItem != null) ? ((KeyValuePair<int, HotkeyAssociation>)list.SelectedItem).Key : -1;
        }

        private object GetSelectedItem(System.Windows.Controls.ListBox list)
        {
            return (list != null && list.SelectedItem != null) ? list.SelectedItem : null;
        }
        #endregion

        /// <summary>
        /// Sets the WndProc message loop hook,
        /// sets the window handle for the hotkeys,
        /// and sets the window state for the saved options.
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);

            ViewModel.SetWindowHandle(new WindowInteropHelper(this).Handle);

            MarkOrphanedHotkeys();

            if (new Options().Hidden)
            {
                this.Visibility = Visibility.Hidden;
                this.WindowState = WindowState.Minimized;
                this.ShowInTaskbar = false;
                MyNotifyIcon.Visible = true;
                MyNotifyIcon.ShowBalloonTip(300);
            }
        }

        /// <summary>
        /// The window message loop. <see cref=" "/>
        /// we need to override it to get global key bind messages. 
        /// Otherwise hotkeys would work only with the WPF window being foreground.
        /// </summary>
        /// <param name="hwnd">The handle to the window receiving the message.</param>
        /// <param name="msg">The received message.</param>
        /// <param name="wParam">First message parameter.</param>
        /// <param name="lParam">Second message parameter.</param>
        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY_MSG_ID = 0x0312;

            if (msg == WM_HOTKEY_MSG_ID)
            {
                int id = wParam.ToInt32();
                if (ViewModel.Hotkeys.ContainsKey(id))
                {
                    ViewModel.Execute(ViewModel.Hotkeys[id]);
                }
            }
            return hwnd;
        }

        /// <summary>
        /// Handles the file drop, calls <see cref="SpawnHotkeyConfig"/> with the filepath string.
        /// </summary>
        private void hotkeyList_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, true))
            {
                string[] fileInfo = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, true);
                if (fileInfo != null && fileInfo.Length > 0)
                {
                    //Start hotkey editor with already set file
                    SpawnHotkeyConfig(fileInfo[0]);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModel.Close();
        }

        /// <summary>
        /// Handles language change from upper menu.
        /// </summary>
        private void languageChosen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SwitchLanguage(ViewModel.CurrentOptions.LanguageFile);
        }

    }
}
