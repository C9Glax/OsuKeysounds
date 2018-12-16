using Gma.System.MouseKeyHook;
using NAudio.Wave;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Keysounds
{
    internal class Keysounds : Form
    {
        private IKeyboardMouseEvents GlobalHook;
        private readonly string[] keypaths = {"key-caps","key-confirm","key-delete","key-movement","key-press-1", "key-press-2", "key-press-3", "key-press-4" };
        private readonly NotifyIcon icon;
        private readonly ContextMenu menu;
        private float volume = 0.2f;

        public Keysounds()
        {
            MenuItem[] items = new MenuItem[] {
                new MenuItem("End", new EventHandler(this.MenuHandler)),
                new MenuItem("Volume 10", new EventHandler(this.MenuHandler)),
                new MenuItem("Volume 0.9", new EventHandler(this.MenuHandler)),
                new MenuItem("Volume 0.8", new EventHandler(this.MenuHandler)),
                new MenuItem("Volume 0.7", new EventHandler(this.MenuHandler)),
                new MenuItem("Volume 0.6", new EventHandler(this.MenuHandler)),
                new MenuItem("Volume 0.5", new EventHandler(this.MenuHandler)),
                new MenuItem("Volume 0.4", new EventHandler(this.MenuHandler)),
                new MenuItem("Volume 0.3", new EventHandler(this.MenuHandler)),
                new MenuItem("Volume 0.2", new EventHandler(this.MenuHandler)),
                new MenuItem("Volume 0.1", new EventHandler(this.MenuHandler)),
                new MenuItem("Pause", new EventHandler(this.MenuHandler))
            };
            this.menu = new ContextMenu(items);
            this.icon = new NotifyIcon()
            {
                BalloonTipText = "Find me here!",
                BalloonTipTitle = "Osu Keyboard sounds",
                Text = "Osu Keyboard sounds",
                Visible = false,
                Icon = new Icon("Resources/osulogo.ico"),
                ContextMenu = this.menu
            };
            this.Icon = new Icon("Resources/osulogo.ico");
            this.Subscribe();
            this.Height = 0;
            this.Width = 500;
            this.MaximizeBox = false;
            this.Resize += this.FrmMain_Resize;
            this.FormClosing += this.Keysounds_FormClosing;
        }

        private void MenuHandler(object sender, EventArgs e)
        {
            switch (((MenuItem)sender).Text)
            {
                case "Pause":
                    this.volume = 0f;
                    break;
                case "End":
                    this.FindForm().Close();
                    break;
                default:
                    this.volume = Convert.ToSingle(((MenuItem)sender).Text.Split(' ')[1])/10;
                    Console.WriteLine(this.volume);
                    break;
            }
        }

        private void Icon_Click(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
            this.BringToFront();
            this.FindForm().Focus();
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.icon.Visible = true;
                this.icon.ShowBalloonTip(500);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                this.icon.Visible = false;
            }
        }

        private void Keysounds_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Unsubscribe();
        }

        public void Subscribe()
        {
            this.GlobalHook = Hook.GlobalEvents();
            this.GlobalHook.KeyDown += this.GlobalHookKeyDown;
        }

        private void GlobalHookKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            Thread t = new Thread(this.Player);
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    t.Start(this.keypaths[2]);
                    break;
                case Keys.Back:
                    t.Start(this.keypaths[1]);
                    break;
                case Keys.Enter:
                    t.Start(this.keypaths[1]);
                    break;
                case Keys.CapsLock:
                    t.Start(this.keypaths[0]);
                    break;
                case Keys.Left:
                    t.Start(this.keypaths[3]);
                    break;
                case Keys.Right:
                    t.Start(this.keypaths[3]);
                    break;
                case Keys.Down:
                    t.Start(this.keypaths[3]);
                    break;
                case Keys.Up:
                    t.Start(this.keypaths[3]);
                    break;
                case Keys.LWin: break;
                case Keys.RWin: break;
                case Keys.LControlKey: break;
                case Keys.RControlKey: break;
                case Keys.LShiftKey: break;
                case Keys.RShiftKey: break;
                case Keys.End: break;
                case Keys.F1: break;
                case Keys.F2: break;
                case Keys.F3: break;
                case Keys.F4: break;
                case Keys.F5: break;
                case Keys.F6: break;
                case Keys.F7: break;
                case Keys.F8: break;
                case Keys.F9: break;
                case Keys.F10: break;
                case Keys.F11: break;
                case Keys.F12: break;
                case Keys.F13: break;
                case Keys.F14: break;
                case Keys.F15: break;
                case Keys.F16: break;
                case Keys.F17: break;
                case Keys.F18: break;
                case Keys.F19: break;
                case Keys.F20: break;
                case Keys.F21: break;
                case Keys.F22: break;
                case Keys.F23: break;
                case Keys.F24: break;
                case Keys.Home: break;
                case Keys.Insert: break;
                case Keys.LMenu: break;
                case Keys.NumLock: break;
                case Keys.PageDown: break;
                case Keys.PageUp: break;
                case Keys.Print: break;
                case Keys.PrintScreen: break;
                case Keys.RMenu: break;
                case Keys.Pause: break;
                case Keys.Delete: break;
                case Keys.Apps: break;
                case Keys.Scroll: break;
                default:
                    t.Start(this.keypaths[new Random().Next(4,7)]);
                    break;
            }
            Console.WriteLine(e.KeyCode);
        }

        private void Player(object path)
        {
            Mp3FileReader mp3 = new Mp3FileReader("Resources/"+(string)path+".mp3");
            WaveOut wo = new WaveOut();
            wo.Init(mp3);
            wo.Volume = this.volume;
            wo.Play();
            while (wo.PlaybackState != PlaybackState.Stopped)
                Thread.Sleep(20);
            wo.Dispose();
            Thread.CurrentThread.Abort();
        }

        public void Unsubscribe()
        {
            this.GlobalHook.KeyDown -= this.GlobalHookKeyDown;
            this.GlobalHook.Dispose();
        }

        static void Main(string[] args)
        {
            Application.Run(new Keysounds());
        }
    }
}
