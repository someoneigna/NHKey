using NHkey.NHotkeyAPI;
using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Forms = System.Windows.Forms;
using Xunit;
using System.Collections;
using System.Windows;

namespace NHotkeyAPI.Tests
{
    class TestNativeMethods : NHotkeyAPI.INativeMethods
    {
        private bool registered;
        private IntPtr lastHandle;

        public bool Register(IntPtr hWnd, int id, int fsModifiers, int vk)
        {
            lastHandle = hWnd;
            registered = !registered;
            return (registered && hWnd != IntPtr.Zero);
        }

        public bool Unregister(IntPtr hWnd, int id)
        {
            if (lastHandle != hWnd)
            {
                return false;
            }
            else
            {
                lastHandle = IntPtr.Zero;
            }
            registered = !registered;

            return !registered;
        }
    }

    public class HotkeyFixture
    {
        // Used to get a test handle to register the hotkeys
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        static extern IntPtr GetDesktopWindow();

        // Taken from http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx
        static int TestKey = 0x51; // P key
        static int TestModifier = 0x11; // Ctrl key
        static int TestMultipleModifier = 0x11 | 0x10; // Ctrl and Shift
        static Tuple<int, int> TestBind = new Tuple<int,int>(TestKey, TestModifier);
        static Tuple<int, int> TestSwitchBind = new Tuple<int, int>(TestKey | 0x01, TestModifier); // Q + Ctrl
        static IntPtr TestHandle = IntPtr.Add(IntPtr.Zero, 1);

        private Hotkey TestHotkeyHolder;

        private Hotkey MakeTestHotkey(int key, int modifiers, IntPtr handle)
        {
            return new Hotkey(new TestNativeMethods(), key, modifiers, handle);
        }

        public HotkeyFixture()
        {
            TestHotkeyHolder = MakeTestHotkey(TestKey, TestModifier, TestHandle);
        }

        [Fact]
        public void CanCreateWithoutParameters()
        {
            Assert.DoesNotThrow(() => new Hotkey());
        }

        [Fact]
        public void CanChangeBind()
        {
            TestHotkeyHolder.SwitchBind(TestBind);
            
            TestHotkeyHolder.SwitchBind(TestSwitchBind);

            Assert.True(TestHotkeyHolder.Equals(TestSwitchBind));
        }

        [Fact]
        public void UnregistersCorrectlyOnDispose()
        {
            TestHotkeyHolder.Register();
            Assert.True(TestHotkeyHolder.Registered);

            TestHotkeyHolder.Dispose();

            Assert.False(TestHotkeyHolder.Registered);
        }

        [Fact]
        public void CanReloadHotkey()
        {
            TestHotkeyHolder.Register();
            TestHotkeyHolder.Unregister();

            TestHotkeyHolder.Reload(TestHandle);
            
            Assert.True(TestHotkeyHolder.Registered);

            TestHotkeyHolder.Dispose();

        }
    }
}
