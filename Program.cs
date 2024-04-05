using System;
using Gtk;

namespace oobe
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            var app = new Application("net.ospio.oobe", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);
            var win = new MainWindow();
            win.Maximize();
            app.AddWindow(win);
            win.Show();
            Application.Run();
        }
    }
}
