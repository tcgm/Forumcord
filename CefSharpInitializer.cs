using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.ConstrainedExecution;
using CefSharp.Wpf;
using CefSharp;

namespace Forumcord
{
    public static class CefSharpInitializer
    {
        public static void Initialize()
        {
            var settings = new CefSettings();
            settings.CefCommandLineArgs.Add("enable-media-stream", "1"); // Example: enable media stream

            // Example: Load extensions (if any)
            //settings.CefCommandLineArgs.Add("enable-media-stream", "1");
            settings.CefCommandLineArgs.Add("enable-npapi", "1");

            var extensionsPath = Path.Combine(Directory.GetCurrentDirectory(), "Extensions");
            if (Directory.Exists(extensionsPath))
            {
                foreach (var extensionDir in Directory.GetDirectories(extensionsPath))
                {
                    settings.CefCommandLineArgs.Add("load-extension", extensionDir);
                }
            }

            // Initialize CefSharp
            Cef.Initialize(settings);
        }
    }
}
