using CefSharp;
using CefSharp.Wpf;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.RightsManagement;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using Xceed.Wpf.AvalonDock.Properties;
using Formatting = Newtonsoft.Json.Formatting;
using Path = System.IO.Path;

namespace Forumcord
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        public DispatcherTimer TickTimer;

        public string currentURL = "";
        private ObservableCollection<Source> Sources { get; set; }
        private string dataFileName = "sources.json";

        public string PersistentDir;

        private const string CacheDir = "CefSharpCache";
        private string cachePath;

        private RequestContext requestContext;
        private RequestContextSettings settings;

        private DispatcherTimer alertTimer;
        private List<string> alerts;
        public int currentAlertCheck = 0;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            PersistentDir = Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData
                        ),
                    Application.ResourceAssembly.GetName().Name
                );
            //InitializeBrowser(XenForoBrowser);
            //InitializeBrowser(MyBBBrowser);
            //InitializeBrowser(PhpBBBrowser);
            InitializeRequestContext();
            InitializeBrowser(ChromeBrowser);
            InitializeHiddenBrowser();

            TickTimer = new DispatcherTimer();
            TickTimer.Interval 
                = new TimeSpan(0, 0, 0, 0, 250);
            TickTimer.Tick += TickTimer_Tick;
            TickTimer.Start();

            LoadSources();
            //SourceList.ItemsSource = Sources;


            // Load the local HTML file
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string localFilePath = Path.Combine(appPath, "ForumcordWeb", "index.html");
            TryNavigate(new Uri(localFilePath).AbsoluteUri);

            InitializeAlertsTimer();
            alerts = new List<string>();
        }

        private void TickTimer_Tick(object? sender, EventArgs e)
        {
            //URLBox.Text = currentURL;

            if(ChromeBrowser.CanGoBack)
            {
                ButtonBack.IsEnabled = true;
            } else
            {
                ButtonBack.IsEnabled = false;
            }

            if (ChromeBrowser.CanGoForward)
            {
                ButtonForward.IsEnabled = true;
            }
            else
            {
                ButtonForward.IsEnabled = false;
            }
        }

        private void InitializeRequestContext()
        {
            // Set up the cache path
            cachePath
                = Path.Combine(
                    PersistentDir,
                    CacheDir
                );
            Directory.CreateDirectory(cachePath); // Ensure the directory exists

            settings = new RequestContextSettings
            {
                CachePath = cachePath,
                PersistSessionCookies = true,
                PersistUserPreferences = true,
            };

            requestContext = new RequestContext(settings);

        }

        private void InitializeBrowser(ChromiumWebBrowser browser)
        {
            browser.RequestContext = requestContext;

            browser.FrameLoadEnd += Browser_FrameLoadEnd;
            browser.DownloadHandler = new CustomDownloadHandler();

            // Assign the custom LifeSpanHandler
            browser.LifeSpanHandler = new CustomLifeSpanHandler();
        }

        private void Browser_FrameLoadEnd(object? sender, FrameLoadEndEventArgs e)
        {
            var browser = sender as ChromiumWebBrowser;
            if (browser != null && e.Frame.IsMain)
            {
                // Example: Automatically log in when navigation completes
                /*var loginScript = @"
                    document.getElementById('login').value = 'username';
                    document.getElementById('password').value = 'password';
                    document.forms[0].submit();
                ";
                browser.ExecuteScriptAsync(loginScript);*/

                // Access the browser.Address on the UI thread
                Dispatcher.Invoke(() =>
                {
                    currentURL = browser.Address;
                    URLBox.Text = currentURL;
                });
                //Console.WriteLine(browser.);
            }
        }

        private void InitializeHiddenBrowser()
        {
            /*hiddenBrowser = new ChromiumWebBrowser
            {
                Visibility = Visibility.Visible
            };*/
            HiddenBrowser.RequestContext = requestContext;

            HiddenBrowser.FrameLoadEnd += OnHiddenBrowserFrameLoadEnd;
            HiddenBrowser.LoadingStateChanged += OnHiddenBrowserLoadingStateChanged;
        }

        private void InitializeAlertsTimer()
        {
            alertTimer = new DispatcherTimer(); // Check every 60 seconds
            alertTimer.Interval = new TimeSpan(0, 0, 5, 0);
            alertTimer.Tick += CheckForAlerts;
            alertTimer.Start();
        }

        private void CheckForAlerts(object? sender, EventArgs e)
        {
            if(currentAlertCheck >= Sources.Count)
            {
                currentAlertCheck = 0;
            }

            Source source = Sources[currentAlertCheck];

            string sourceURL = source.Url;
            string addonURL = "";

            if (sourceURL != null) {
                switch (source.ForumType)
                {
                    case SourceType.None:

                        break;
                    case SourceType.XenForo:
                        addonURL = "account/alerts?show_only=all";
                        break;
                }

                string checkURL = sourceURL + addonURL;

                // For each source, navigate to the alerts page
                CheckSourceAlerts(checkURL);
                // Add more sources as needed

                currentAlertCheck++;
            }
        }

        private void CheckSourceAlerts(string url)
        {
            Application.Current.Dispatcher.Invoke(() => HiddenBrowser.Load(url));
        }
        private void OnHiddenBrowserFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Frame.IsMain)
            {
                // Inject JavaScript to extract alert information
                string script = @"
                    var alerts = document.querySelectorAll('.structItem--alert'); // Adjust selector as needed
                    var alertsHtml = [];
                    alerts.forEach(alert => {
                        alertsHtml.push(alert.outerHTML);
                    });
                    return alertsHtml;
                ";
                e.Frame.EvaluateScriptAsync(script).ContinueWith(task =>
                {
                    var response = task.Result;
                    if (response.Success && response.Result != null)
                    {
                        var alertsHtml = response.Result as List<object>;
                        if (alertsHtml != null)
                        {
                            foreach (var alertHtml in alertsHtml)
                            {
                                string alertString = alertHtml.ToString();
                                if (!alerts.Contains(alertString))
                                {
                                    alerts.Add(alertString);
                                }
                            }

                            if (AlertsContentControl != null)
                            {
                                Application.Current.Dispatcher.Invoke(() => AlertsContentControl.UpdateAlerts(alerts));
                            }
                        }
                    }
                });
            }
        }

        private void OnHiddenBrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                // Optionally perform actions when the browser finishes loading
            }
        }

        private void NavigateToUrl(string url)
        {
            ChromeBrowser.LoadUrlAsync(url);
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            ChromeBrowser.Back();
        }

        private void ButtonForward_Click(object sender, RoutedEventArgs e)
        {
            ChromeBrowser.Forward();
        }

        private void ButtonGo_Click(object sender, RoutedEventArgs e)
        {
            string url = URLBox.Text;

            TryNavigate(url);
        }

        private void URLBox_KeyUp(object sender, KeyEventArgs e)
        {
            string url = URLBox.Text;

            if (e.Key == Key.Enter)
            {
                TryNavigate(url);
            }
        }

        public void TryNavigate(string url)
        {
            if (IsValidUrl(url))
            {
                NavigateToUrl(url);
            }
            else
            {
                NavigateToUrl("https://duckduckgo.com/?q="
                    + url);
            }
        }

        public static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp
                || uriResult.Scheme == Uri.UriSchemeHttps
                || uriResult.Scheme == Uri.UriSchemeFile);
        }

        private void ButtonAddSource_Click(object sender, RoutedEventArgs e)
        {
            AddSource("New Forum", "https://newforum.com", "forum.png");
        }


        public void AddSource(Source source)
        {
            AddSource(source.Name, source.Url, source.IconPath);
        }

        private void AddSource(string name, string url, string iconPath)
        {

            // Get the default icon path from resources
            var defaultIconPath = "/forum.png"; // Adjust the path as needed

            // Load the default icon image
            //var defaultIcon = LoadEmbeddedImage(defaultIconPath);

            // Show dialog to get Source info (for simplicity, use hardcoded values)
            Source newSource = new Source
            {
                Name = "New Forum",
                Url = "https://newforum.com",
                IconPath = defaultIconPath,
                Folder = "General",
                ForumType = SourceType.None
            };

            Sources.Add(newSource);
            SaveSources();

            CreateSourceEntry(newSource);
        }

        private void CreateSourceEntry(Source newSource)
        {
            var sourceControl = new SourceEntryControl(newSource);
            //sourceControl.SetSource(newSource);

            SourceList.Children.Add(sourceControl);
        }

        private void LoadSources()
        {
            string sourcesFilePath
                = Path.Combine(PersistentDir, dataFileName);

            if (File.Exists(sourcesFilePath))
            {
                var json = File.ReadAllText(sourcesFilePath);
                Sources = JsonConvert.DeserializeObject<ObservableCollection<Source>>(json);

                if (Sources == null)
                {
                    Sources = new ObservableCollection<Source>();
                }
            }
            else
            {
                Sources = new ObservableCollection<Source>();
            }
        }

        public void SaveSources()
        {
            string sourcesFilePath
                = Path.Combine(PersistentDir, dataFileName);

            var json 
                = JsonConvert.SerializeObject(Sources, Formatting.Indented);
            File.WriteAllText(sourcesFilePath, json);
        }

        private void Source_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        private void Source_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Source)))
            {
                var droppedData = e.Data.GetData(typeof(Source)) as Source;
                var target = ((SourceEntryControl)sender).DataContext as Source;

                int removedIdx = Sources.IndexOf(droppedData);
                int targetIdx = Sources.IndexOf(target);

                if (removedIdx != targetIdx)
                {
                    Sources.Move(removedIdx, targetIdx);
                    SaveSources();
                }
            }
        }

        private void Source_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var source = (sender as SourceEntryControl).DataContext as Source;
            DragDrop.DoDragDrop((DependencyObject)sender, source, DragDropEffects.Move);
        }

        // Attach the event handler to each item
        private void SourceList_Loaded(object sender, RoutedEventArgs e)
        {
            /*foreach (var item in SourceList.Children)
            {
                var container = SourceList.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                if (container != null)
                {
                    var stackPanel = container.FindName("SourceItemPanel") as StackPanel;
                    if (stackPanel != null)
                    {
                        stackPanel.MouseDown += Source_MouseDown;
                    }
                }
            }*/

            foreach (var source in Sources.ToList<Source>()) {
                CreateSourceEntry(source);
            }
        }

        private BitmapImage LoadEmbeddedImage(string imagePath)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceStream = assembly.GetManifestResourceStream(imagePath);

            if (resourceStream == null)
                throw new Exception($"Resource '{imagePath}' not found.");

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = resourceStream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze(); // To make it cross-thread accessible

            return bitmap;
        }

        private void ShowAlertsButton_Click(object sender, RoutedEventArgs e)
        {
            SidebarGrid.Visibility = Visibility.Visible;
            AlertsContentControl.UpdateAlerts(alerts);
        }
    }
}