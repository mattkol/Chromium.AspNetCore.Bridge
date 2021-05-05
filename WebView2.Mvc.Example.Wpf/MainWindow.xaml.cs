using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Chromium.AspNetCore.Bridge;
using System.IO;

namespace WebView2.Mvc.Example.Wpf
{
    //Shorthand for Owin pipeline func
    using AppFunc = Func<IDictionary<string, object>, Task>;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppFunc _appFunc;
        private OwinResourceHandler _owinResourceHandler;
        public MainWindow()
        {
            InitializeComponent();

            Browser.CoreWebView2InitializationCompleted += Browser_CoreWebView2InitializationCompleted;
        }

        private void Browser_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                _appFunc = ((App)Application.Current).AppFunc;
                Browser.CoreWebView2.WebResourceRequested += BrowserWebResourceRequestedAsync;
                Browser.NavigationStarting += Browser_NavigationStarting;
                Browser.SourceChanged += Browser_SourceChanged;
                Browser.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);

                _owinResourceHandler = new OwinResourceHandler(_appFunc);
            }
        }

        private void Browser_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
        }

        private void Browser_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
        }

        private void BrowserWebResourceRequestedAsync(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            var deferral = e.GetDeferral();
            var coreWebView2 = (CoreWebView2)sender;

            Action<ResourceResponse> callback = (ResourceResponse resourceResponse) =>
            {
                var response = coreWebView2.Environment
                                          .CreateWebResourceResponse(resourceResponse.Stream, 
                                          resourceResponse.StatusCode, 
                                          resourceResponse.ReasonPhrase, 
                                          resourceResponse.GetHeaderString());
                if (response != null)
                {
                    e.Response = response;
                }

                deferral?.Complete();
            };


            var request = new ResourceRequest(e.Request.Uri, e.Request.Method, e.Request.Headers, e.Request.Content);
            _owinResourceHandler.ProcessRequest(request, callback);
        }

    }
}
