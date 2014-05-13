using NHkey.NHotkeyAPI;
using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Forms = System.Windows.Forms;
using Xunit;
using System.Collections;

namespace NHotkeyAPI.Tests
{
    public class HotkeyFixture
    {
        // Used to get a test handle to register the hotkeys
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        static extern IntPtr GetDesktopWindow();

        // Taken from http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx
        static int TestKey = 0x54; // T key
        static int TestModifier = 0x11; // Ctrl key
        static int TestMultipleModifier = 0x11 | 0x10; // Ctrl and Shift
        static Tuple<int, int> TestBind = new Tuple<int,int>(TestKey, TestModifier);
        static Tuple<int, int> TestSwitchBind = new Tuple<int, int>(TestKey | 0x01, TestModifier); // U + Ctrl

        readonly Forms.Form Window;
        readonly IntPtr TestWindowHandle;

        public HotkeyFixture()
        {
            Window = new Forms.Form();
            Window.Visible = false;

            TestWindowHandle = Window.Handle;
        }

        [Fact]
        public void CanCreateWithoutParameters()
        {
            Assert.DoesNotThrow(() => new Hotkey());
        }

        [Fact]
        public void CanChangeBind()
        {
            var hotkey = new Hotkey(TestKey, TestModifier);
            
            hotkey.SwitchBind(TestSwitchBind);

            Assert.True(hotkey.Equals(TestSwitchBind));
        }

        [Fact]
        public void UnregistersCorrectlyOnDispose()
        {
            var hotkey = new Hotkey(TestKey, TestModifier, TestWindowHandle);
            
            hotkey.Register();
            Assert.True(hotkey.Registered);

            hotkey.Dispose();

            Assert.False(hotkey.Registered);
        }

        [Fact]
        public void CanReloadHotkey()
        {
            var hotkey = new Hotkey(TestBind, TestWindowHandle);
            hotkey.Register();

            var temporalWindowForm = new Forms.Form();

            hotkey.Reload(temporalWindowForm.Handle);
            
            Assert.True(hotkey.Registered && hotkey.Handle != TestWindowHandle);

            hotkey.Dispose();

            temporalWindowForm.Dispose();
        }

    }
}
