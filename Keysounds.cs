using Gma.System.MouseKeyHook;
using NAudio.Wave;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Keysounds
{
    internal class Keysounds : Form //ugly way to do this, but hey, this is all hacked together anyways
    {
        private IKeyboardMouseEvents keyboardHook;
        private readonly string[] keypaths = {"key-caps","key-confirm","key-delete","key-movement","key-press-1", "key-press-2", "key-press-3", "key-press-4" };
        private readonly NotifyIcon notifyIcon;
        private float playbackVolume = 0.2f;

        public Keysounds()
        {
            EventHandler menuClickHandler = new EventHandler(this.MenuHandler);
            MenuItem[] menuItems = new MenuItem[] {
                new MenuItem("End", menuClickHandler),
                new MenuItem("Volume 10", menuClickHandler),
                new MenuItem("Volume 0.9", menuClickHandler),
                new MenuItem("Volume 0.8", menuClickHandler),
                new MenuItem("Volume 0.7", menuClickHandler),
                new MenuItem("Volume 0.6", menuClickHandler),
                new MenuItem("Volume 0.5", menuClickHandler),
                new MenuItem("Volume 0.4", menuClickHandler),
                new MenuItem("Volume 0.3", menuClickHandler),
                new MenuItem("Volume 0.2", menuClickHandler),
                new MenuItem("Volume 0.1", menuClickHandler),
                new MenuItem("Pause", menuClickHandler)
            };
            ContextMenu notifyIconMenu = new ContextMenu(menuItems);
            this.notifyIcon = new NotifyIcon()
            {
                Visible = false,
                Icon = new Icon("Resources/osulogo.ico"),
                ContextMenu = notifyIconMenu,
                Text = "Osu Keysounds"
            };
            this.Icon = new Icon("Resources/osulogo.ico");
            this.Subscribe();
            this.Height = 0;
            this.Width = 300;
            this.Text = "Osu Keysounds";
            this.MaximizeBox = false;
            this.Resize += this.FrmMain_Resize;
            this.FormClosing += this.Keysounds_FormClosing;
        }


        /// <summary>
        /// Handles the Selection of the ToolTipMenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandler(object sender, EventArgs e)
        {
            switch (((MenuItem)sender).Text)
            {
                case "Pause":
                    this.playbackVolume = 0f;
                    break;
                case "End":
                    this.FindForm().Close();
                    break;
                default:
                    this.playbackVolume = Convert.ToSingle(((MenuItem)sender).Text.Split(' ')[1])/10;
                    break;
            }
        }

        /// <summary>
        /// Called when Form-size is changed in any way (for example minimized)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.notifyIcon.Visible = true;
                this.Hide();
            }
        }

        /// <summary>
        /// Called when exiting the Program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Keysounds_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Unsubscribe();
        }

        /// <summary>
        /// Creates the KeyboardHook
        /// </summary>
        private void Subscribe()
        {
            this.keyboardHook = Hook.GlobalEvents();
            this.keyboardHook.KeyDown += this.GlobalHookKeyDown;
        }

        /// <summary>
        /// Callback-Method for the Keyboard-Hook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlobalHookKeyDown(object sender, KeyEventArgs e)
        {
            Thread t = new Thread(this.Player); //Asynchronous playing
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
                //Arrow-Keys play movement sound
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
                //Next few lines are ugly, please scroll past. thank you.
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
                    t.Start(this.keypaths[new Random().Next(4,7)]); //Play one of the four Key-typing sounds
                    break;
            }
            Console.WriteLine(e.KeyCode);
        }

        private void Player(object path)
        {
            Mp3FileReader mp3 = new Mp3FileReader("Resources/"+(string)path+".mp3");
            WaveOut wo = new WaveOut();
            wo.Init(mp3);
            wo.Volume = this.playbackVolume;
            wo.Play();
            while (wo.PlaybackState != PlaybackState.Stopped)
                Thread.Sleep(20);
            wo.Dispose();
            Thread.CurrentThread.Abort();
        }

        private void Unsubscribe()
        {
            this.keyboardHook.KeyDown -= this.GlobalHookKeyDown;
            this.keyboardHook.Dispose();
        }

        public static void Main(string[] args)
        {
            Application.Run(new Keysounds());
        }
    }
}
