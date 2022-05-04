using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSU_Keysounds
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if(args.Length == 0)
            {
                Application.Run(new TrayApplication(new string[] { Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\osu!" }));
            }
            else
            {
                Application.Run(new TrayApplication(args));
            }
        }
    }
}
