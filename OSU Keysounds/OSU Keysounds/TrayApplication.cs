using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Media;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace OSU_Keysounds
{
    public partial class TrayApplication : Form
    {
        private static Dictionary<Keys, UnmanagedMemoryStream> keySoundDict = new Dictionary<Keys, UnmanagedMemoryStream>();

        private readonly static List<Keys> key_delete = new List<Keys>() { Keys.Back, Keys.Delete };
        private readonly static List<Keys> key_movement = new List<Keys>() { Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Home, Keys.End, Keys.PageDown, Keys.PageUp };

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private readonly static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;


        private static Random r = new Random();
        private static SoundPlayer player = new SoundPlayer();
        public TrayApplication()
        {
            InitializeComponent();

            foreach (Keys key in key_delete)
            {
                keySoundDict.Add(key, sounds.key_delete);
            }
            foreach (Keys key in key_movement)
            {
                keySoundDict.Add(key, sounds.key_movement);
            }
            keySoundDict.Add(Keys.Enter, sounds.key_confirm);
            keySoundDict.Add(Keys.CapsLock, sounds.key_caps);

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
                Keys pressedKey = (Keys)Marshal.ReadInt32(lParam);

                if (!keySoundDict.ContainsKey(pressedKey))
                {
                    switch (r.Next(0, 5))
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
                else
                {
                    player.Stream = keySoundDict[pressedKey];
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
