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
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace Forumcord
{
    /// <summary>
    /// Interaction logic for SourceEntryControl.xaml
    /// </summary>
    public partial class SourceEntryControl : UserControl
    {

        public Source source; 

        public SourceEntryControl(Source newSource, bool created)
        {
            SetSource(newSource);
            InitializeComponent();
            DataContext = this; // Set UserControl's DataContext to itself
            
            if(created)
            {
                OpenEditWindow();
            }
        }

        public static readonly DependencyProperty NameProp =
                    DependencyProperty.Register("Name", typeof(string), typeof(SourceEntryControl));

        public string ForumName
        {
            get { return (string)GetValue(NameProp); }
            set { SetValue(NameProp, value); }
        }

        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(string), typeof(SourceEntryControl));

        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        // Add other properties as needed

        public static readonly DependencyProperty ForumImageProperty =
            DependencyProperty.Register("ForumImage", typeof(BitmapImage), typeof(SourceEntryControl));

        public BitmapImage ForumImage
        {
            get { return (BitmapImage)GetValue(ForumImageProperty); }
            set { SetValue(ForumImageProperty, value); }
        }

        public static readonly DependencyProperty ForumImagePathProperty =
            DependencyProperty.Register("ForumImagePath", typeof(string), typeof(SourceEntryControl));

        public string ForumImagePath
        {
            get { return (string)GetValue(ForumImagePathProperty); }
            set { SetValue(ForumImagePathProperty, value); }
        }

        public void SetSource(Source newSource)
        {
            source = newSource;
            UpdateControl();
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            OpenEditWindow();
        }

        public void OpenEditWindow()
        {
            EditSourceWindow editWindow = new EditSourceWindow(this, EditWindowCallback);
            editWindow.ShowDialog();
        }

        private void EditWindowCallback(List<string> list)
        {
            if (list.Count > 0)
            {
                SetSourceValues(list[0],
                    list[1],
                    list[2]);

                MainWindow.Instance.SaveSources();
            } else
            {
                //Do nothing, the edit operation was canceled.
            }
        }

        public void SetSourceValues(string newName,
            string newUrl,
            string newImagePath)
        {
            source.Name = newName;
            source.Url = newUrl;
            source.IconPath = newImagePath;
        }

        public void UpdateControl()
        {
            ForumName = source.Name;
            string iconPath = source.IconPath;

            if (string.IsNullOrEmpty(iconPath) || !File.Exists(iconPath))
            {
                // Set a temporary default image while we fetch the actual favicon
                ForumImage = new BitmapImage(new Uri(Values.DefaultSourceIcon, UriKind.RelativeOrAbsolute));
                // Start the async process to download and set the favicon
                DownloadAndSetFaviconAsync(source.Url, source.Name, source);
            }
            else
            {
                ForumImage = new BitmapImage(new Uri(iconPath, UriKind.RelativeOrAbsolute));
            }
        }

        private void UserControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenContextMenu();
        }

        public void OpenContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();

            MenuItem editItem = new MenuItem { Header = "Edit" };
            editItem.Click += EditItem_Click;

            contextMenu.Items.Add(editItem);
            contextMenu.IsOpen = true;
        }

        private async void DownloadAndSetFaviconAsync(string url, string name, Source source)
        {
            string iconPath = await DownloadAndCacheFavicon(url, name);
            source.IconPath = iconPath;

            // Use the dispatcher to update the UI from the UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                ForumImage = new BitmapImage(new Uri(iconPath, UriKind.RelativeOrAbsolute));
            });
        }

        private async Task<string> DownloadAndCacheFavicon(string url, string name)
        {
            string faviconUrl = new Uri(new Uri(url), "/favicon.ico").AbsoluteUri;
            string cacheDir = Path.Combine(MainWindow.Instance.PersistentDir, "Cache");
            string iconFilePath = Path.Combine(cacheDir, $"{name}_favicon.ico");

            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
            }

            if (!File.Exists(iconFilePath))
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        byte[] faviconData = await client.GetByteArrayAsync(faviconUrl);
                        await File.WriteAllBytesAsync(iconFilePath, faviconData);
                    }
                    catch (Exception)
                    {
                        // Handle errors (e.g., favicon not found)
                        return Values.DefaultSourceIcon; // Default icon path
                    }
                }
            }

            return iconFilePath;
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Instance.TryNavigate(source.Url);
        }
    }
}
