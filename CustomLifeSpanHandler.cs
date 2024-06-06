using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forumcord
{
    public class CustomLifeSpanHandler : ILifeSpanHandler
    {
        public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            return false; // Do not allow closing
        }

        public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            // Handle after the browser is created
        }

        public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            // Handle before the browser is closed
        }

        public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl,
                                  string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture,
                                  IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings,
                                  ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            // Prevent new popup windows
            newBrowser = null;
            chromiumWebBrowser.Load(targetUrl); // Redirect to the same browser
            return true; // Cancel the popup creation
        }
    }


}