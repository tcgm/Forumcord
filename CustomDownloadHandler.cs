using CefSharp;

public class CustomDownloadHandler : IDownloadHandler
{

    public void OnDownloadUpdated(IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
    {
        // Implement if needed
    }

    private void HandleCrxDownload(DownloadItem downloadItem)
    {
        // Manually download the CRX file and install it
        string downloadUrl = downloadItem.OriginalUrl;
        string downloadPath = "path_to_save_crx_file";
        DownloadCrxFile(downloadUrl, downloadPath);
        InstallExtension(downloadPath);
    }

    private void DownloadCrxFile(string url, string filePath)
    {
        // Implement file download using HttpClient or WebClient
    }

    private void InstallExtension(string crxFilePath)
    {
        // Implement extension installation using C# code
    }

    public bool CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod)
    {
        return true;
    }

    public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
    {
        // Intercept the download of the CRX file
        if (downloadItem.SuggestedFileName.EndsWith(".crx"))
        {
            // Cancel the default download behavior
            callback.Dispose();
            // Manually handle the download and installation
            HandleCrxDownload(downloadItem);
        }
        else
        {
            // Allow other downloads to proceed normally
            callback.Continue(downloadItem.SuggestedFileName, showDialog: true);
        }
    }

    public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
    {
        //throw new NotImplementedException();
    }
}
