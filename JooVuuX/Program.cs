using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;

namespace JooVuuX
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            VlcContext.LibVlcDllsPath = AppDomain.CurrentDomain.BaseDirectory + "VLC Dlls"; // CommonStrings.LIBVLC_DLLS_PATH_DEFAULT_VALUE_AMD64;
            VlcContext.LibVlcPluginsPath = AppDomain.CurrentDomain.BaseDirectory + "VLC Dlls/plugins"; //CommonStrings.PLUGINS_PATH_DEFAULT_VALUE_AMD64;

            // Ignore the VLC configuration file
            VlcContext.StartupOptions.IgnoreConfig = true;
            // Enable file based logging
            VlcContext.StartupOptions.LogOptions.LogInFile = false;
            // Shows the VLC log console (in addition to the applications window)
            VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = false;
            // Set the log level for the VLC instance
            //VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.Debug;
            // Initialize the VlcContext
            VlcContext.Initialize();
            // Close the VlcContext
            VlcContext.CloseAll();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FirstNotification());
        }
    }
}
