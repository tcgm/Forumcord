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
using System.Windows.Shapes;

namespace Forumcord
{
    /// <summary>
    /// Interaction logic for EditSourceWindow.xaml
    /// </summary>
    public partial class EditSourceWindow : Window
    {
        private SourceEntryControl sourceEntry;

        public EditSourceWindow(SourceEntryControl sourceControl)
        {
            InitializeComponent();
            sourceEntry = sourceControl;

            NameTextBox.Text = sourceControl.ForumName;
            UrlTextBox.Text = sourceControl.Url;
            IconPathTextBox.Text = sourceControl.ForumImagePath;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            sourceEntry.SetSourceValues(NameTextBox.Text, UrlTextBox.Text, IconPathTextBox.Text);
            MainWindow.Instance.SaveSources();
            this.Close();
        }
    }
}
