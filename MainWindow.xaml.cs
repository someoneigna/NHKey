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


namespace NHkey
{

    public partial class MainWindow : Window
    {
        
        private System.Windows.Forms.NotifyIcon MyNotifyIcon;
        private NHKeyController appController;

        public MainWindow()
        {
            
            MyNotifyIcon = new System.Windows.Forms.NotifyIcon();

            // MyNotifyIcon.Icon = new System.Drawing.Icon(@"file:\icon.png");
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
        }

        private void agregarButton_Click(object sender, RoutedEventArgs e)
        {
            SpawnHotkeyConfig();
        }

        private void eliminarButton_Click(object sender, RoutedEventArgs e)
        {
            if (hotkeyList.SelectedItems.Count > 0)
            {
                ((Dictionary<int, Hotkey>)appController.GetHotkeySource()).Remove(GetSelectedKey(hotkeyList));
                
                hotkeyList.Items.Refresh();
            }
        }

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
        protected override void OnSourceInitialized(EventArgs e)
        {
            //base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);


            /*********** This shouldnt be here, but I need the damn Window handle to activate the Hotkeys **************/
            appController = new NHKeyController(new WindowInteropHelper(this).Handle);
            appController.OnStart();

            appController.OnLoad();
            Resources["Hotkeys"] = appController.GetHotkeySource();

            if (appController.Hidden)
            {
                this.Visibility = Visibility.Hidden;
                this.WindowState = WindowState.Minimized;
                this.ShowInTaskbar = false;
                MyNotifyIcon.Visible = true;
                MyNotifyIcon.ShowBalloonTip(300);
            }
            /***********************************************************************************************************/
        }

        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY_MSG_ID = 0x0312;
            
            if (msg == WM_HOTKEY_MSG_ID)
            {
                int id = wParam.ToInt32();
                appController.ActivateHotkey(id);
                /*foreach(var hotkey in appController.RecentlyUsed)
                {
                    JumpTask task = new JumpTask();
                    task.ApplicationPath = hotkey.FilePath;
                    task.Arguments = hotkey.Parameters;
                    task.Title = hotkey.Name;

                    jumpList.JumpItems.Add(task);
                }*/
            }

            return hwnd;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            appController.OnClose();
        }
       

        private void SpawnHotkeyConfig(string filename = null, Hotkey editHotkey = null)
        {
            bool success = false;
            HotkeyConfig dialog = null;

            Hotkey holder = (editHotkey != null) ? new Hotkey(editHotkey) : new Hotkey() {FilePath=filename};

            while (success == false)
            {
                    
                dialog = new HotkeyConfig(holder);

                dialog.Owner = this;
                dialog.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                dialog.ShowDialog();

                if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                {
                    Hotkey hk = dialog.ResultHotkey;
                    if (hk != null)
                    {
                        if (editHotkey != null)
                            success = appController.ReplaceHotkey(editHotkey, hk);
                        else

                            success = appController.AddNewHotkey(hk);
                    }
                    if (!success)
                        System.Windows.MessageBox.Show("No se puede asignar la misma combinacion!");
                    else
                        hotkeyList.Items.Refresh();
                }
                else
                {
                    success = true;
                    if (editHotkey != null)
                        editHotkey.Register();
                }
            }
            dialog.Close();
        }

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

        private void editarButton_Click(object sender, RoutedEventArgs e)
        {
            if (hotkeyList.SelectedItems.Count > 0)
            {
                KeyValuePair<int, Hotkey> entry = ((KeyValuePair<int, Hotkey>)hotkeyList.SelectedItem);
                SpawnHotkeyConfig(null, entry.Value);
            }
        }

        private void opcionesButton_Click(object sender, RoutedEventArgs e)
        {
            Opciones dialog = new Opciones(appController.GetOptions());
            dialog.Owner = this;
            dialog.ShowDialog();

            if(dialog.DialogResult.Equals(true))
                appController.SetOptions(dialog.GetOptions());
            dialog.Close();
            
        }

        private void hotkeyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ListBox list = sender as System.Windows.Controls.ListBox;
            if (e.AddedItems.Count > 0)
            {
                var last = e.AddedItems[0];
                foreach (var item in new ArrayList(list.SelectedItems))
                    if (item != last) list.SelectedItems.Remove(item);
                list.Items.Refresh();
            }
        }

        private void stackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2)
            {
                if(hotkeyList.SelectedItem != null)
                {
                    Hotkey hk = GetSelectedHotkey(hotkeyList);
                    if (hk != null) 
                        hk.Execute();
                }
            }
        }

        private static Hotkey GetSelectedHotkey(System.Windows.Controls.ListBox list)
        {
            return (list != null && list.SelectedItem != null) ? ((KeyValuePair<int, Hotkey>)list.SelectedItem).Value : null;
        }

        private static int GetSelectedKey(System.Windows.Controls.ListBox list)
        {
            return (list != null && list.SelectedItem != null) ? ((KeyValuePair<int, Hotkey>)list.SelectedItem).Key : -1;
        }

        private object GetSelectedItem(System.Windows.Controls.ListBox list)
        {
            return (list != null && list.SelectedItem != null) ? list.SelectedItem : null;
        }

     
    }
}
