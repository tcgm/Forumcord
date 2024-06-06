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
        public Action<List<string>> windowCallback;

        public EditSourceWindow(SourceEntryControl sourceControl,
            Action<List<string>> windowCallback)
        {
            InitializeComponent();
            sourceEntry = sourceControl;
            Source source = sourceControl.source;

            NameTextBox.Text = source.Name;
            UrlTextBox.Text = source.Url;
            IconPathTextBox.Text = source.IconPath;
            IconPathTextBox.CaretIndex = IconPathTextBox.Text.Length > 0 ? IconPathTextBox.Text.Length-1 : 0;
            this.windowCallback = windowCallback;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> results = new List<string>();
            results.Add(NameTextBox.Text);
            results.Add(UrlTextBox.Text);
            results.Add(IconPathTextBox.Text);

            windowCallback?.Invoke(results);

            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> results = new List<string>();

            windowCallback?.Invoke(results);

            this.Close();
        }
    }
}
