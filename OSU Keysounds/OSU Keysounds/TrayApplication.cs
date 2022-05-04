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
        private static Dictionary<Keys, Stream> keySoundDict = new Dictionary<Keys, Stream>();

        private readonly static List<Keys> key_delete = new List<Keys>() { Keys.Back, Keys.Delete };
        private readonly static List<Keys> key_movement = new List<Keys>() { Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Home, Keys.End, Keys.PageDown, Keys.PageUp };
        private static Stream[] randomSounds = new Stream[4];

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private readonly static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;


        private static Random r = new Random();
        public TrayApplication(string[] args)
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

            randomSounds[0] = sounds.key_press_1;
            randomSounds[1] = sounds.key_press_2;
            randomSounds[2] = sounds.key_press_3;
            randomSounds[3] = sounds.key_press_4;


            string skinsDirectoryPath = args[0] + @"\Skins";
            if (!Directory.Exists(skinsDirectoryPath))
            {
                DialogResult r = MessageBox.Show(String.Format("Could not find skins at location: \"{0}\"\nOnly standard sounds will be available. Continue?", skinsDirectoryPath), "Skins not found", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (r == DialogResult.No)
                    this.Close();
            }
            else
            {
                foreach (DirectoryInfo skinDirectory in new DirectoryInfo(skinsDirectoryPath).GetDirectories())
                {
                    ToolStripMenuItem skinItem = new ToolStripMenuItem()
                    {
                        Text = skinDirectory.Name
                    };
                    skinItem.Click += delegate
                    {
                        this.changeSkin(skinDirectory.FullName + "\\");
                    };
                    this.contextMenuStrip.Items.Add(skinItem);
                }
            }

            _hookID = SetHook(_proc);
        }

        private void changeSkin(string path)
        {
            foreach (Stream s in keySoundDict.Values)
                s.Dispose();
            keySoundDict = new Dictionary<Keys, Stream>();
            FileStream deleteKeyStream = new FileStream(path + "key-delete.wav", FileMode.Open);
            FileStream movementKeyStream = new FileStream(path + "menu-direct-hover.wav", FileMode.Open);
            foreach (Keys key in key_delete)
            {
                keySoundDict.Add(key, deleteKeyStream);
            }
            foreach (Keys key in key_movement)
            {
                keySoundDict.Add(key, movementKeyStream);
            }
            keySoundDict.Add(Keys.Enter, new FileStream(path + "menuclick.wav", FileMode.Open));
            keySoundDict.Add(Keys.CapsLock, new FileStream(path + "menu-edit-hover.wav", FileMode.Open));
            keySoundDict.Add(Keys.Escape, new FileStream(path + "click-close.wav", FileMode.Open));

            randomSounds[0] = new FileStream(path + "key-press-1.wav", FileMode.Open);
            randomSounds[1] = new FileStream(path + "key-press-2.wav", FileMode.Open);
            randomSounds[2] = new FileStream(path + "key-press-3.wav", FileMode.Open);
            randomSounds[3] = new FileStream(path + "key-press-4.wav", FileMode.Open);
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
            SoundPlayer player = new SoundPlayer();
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                Keys pressedKey = (Keys)Marshal.ReadInt32(lParam);

                if (!keySoundDict.ContainsKey(pressedKey))
                {
                    player.Stream = randomSounds[r.Next(0, 4)];
                }
                else
                {
                    player.Stream = keySoundDict[pressedKey];
                }
                player.Stream.Position = 0;
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
