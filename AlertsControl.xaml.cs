using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Forumcord
{
    /// <summary>
    /// Interaction logic for AlertsWindow.xaml
    /// </summary>
    public partial class AlertsControl : UserControl
    {
        public AlertsControl()
        {
            InitializeComponent();
        }

        public void UpdateAlerts(List<string> alerts)
        {
            string htmlContent = "<html><body>";
            foreach (var alert in alerts)
            {
                htmlContent += alert;
            }
            htmlContent += "</body></html>";

            AlertsWebBrowser.NavigateToString(htmlContent);
        }
    }
}
