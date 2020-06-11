using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Media;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using OSU_Keysounds.Properties;

namespace OSU_Keysounds
{
    public partial class TrayApplication : Form
    {
        private readonly static List<Keys> key_delete = new List<Keys>() { Keys.Back, Keys.Delete };
        private readonly static List<Keys> key_movement = new List<Keys>() { Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Home, Keys.End, Keys.PageDown, Keys.PageUp };

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private readonly static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public TrayApplication()
        {
            InitializeComponent();
            _hookID = SetHook(_proc);
        }

        private void toolStripExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TrayApplication_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void TrayApplication_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnhookWindowsHookEx(_hookID);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                SoundPlayer player = new SoundPlayer();
                if(Keys.Enter == (Keys)vkCode)
                {
                    player.Stream = sounds.key_confirm;
                }
                else if(Keys.CapsLock == (Keys)vkCode)
                {
                    player.Stream = sounds.key_caps;
                }
                else if (key_delete.Contains((Keys)vkCode))
                {
                    player.Stream = sounds.key_delete;
                }
                else if (key_movement.Contains((Keys)vkCode))
                {
                    player.Stream = sounds.key_movement;
                }
                else
                {
                    Random r = new Random();
                    switch (r.Next(1, 4))
                    {
                        case 1:
                            player.Stream = sounds.key_press_1;
                            break;
                        case 2:
                            player.Stream = sounds.key_press_2;
                            break;
                        case 3:
                            player.Stream = sounds.key_press_3;
                            break;
                        default:
                            player.Stream = sounds.key_press_4;
                            break;
                    }
                }
                player.Play();
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

    }
}
