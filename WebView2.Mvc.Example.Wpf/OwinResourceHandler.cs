using Chromium.AspNetCore.Bridge;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WebView2.Mvc.Example.Wpf
{
    //Shorthand for Owin pipeline func
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class OwinResourceHandler
    {
        private readonly AppFunc _appFunc;

        public OwinResourceHandler(AppFunc appFunc)
        {
            _appFunc = appFunc;
        }

        public virtual void ProcessRequest(ResourceRequest request, CoreWebView2Deferral deferral, Action<ResourceResponse, CoreWebView2Deferral> callback)
        {
            Task.Run(async () =>
            {
                try
                {

                    ResourceResponse response = null;
                    response = await RequestInterceptor.ProcessRequest(_appFunc, request);

                    while (IsRedirected((HttpStatusCode)response.StatusCode))
                    {
                        string redirectUrl = FormatRedirectUrl(request.Url, response.Headers);
                        var newRequest = new Chromium.AspNetCore.Bridge.ResourceRequest(redirectUrl, "GET", request.Headers, null);
                        response = await RequestInterceptor.ProcessRequest(_appFunc, newRequest);
                    }

                    if (IsBadRequest((HttpStatusCode)response.StatusCode) || IsRouteNotFound((HttpStatusCode)response.StatusCode))
                    {
                        string redirectUrl = GetHomeUrl(request.Url);
                        var newRequest = new Chromium.AspNetCore.Bridge.ResourceRequest(redirectUrl, "GET", request.Headers, null);
                        response = await RequestInterceptor.ProcessRequest(_appFunc, newRequest);
                    }

                     ((App)Application.Current).Dispatcher.Invoke(
                      DispatcherPriority.Background,
                      new Action(() => 
                      {
                          // Callback
                          callback.Invoke(response, deferral);
                      }));
                  }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    Console.WriteLine(exception.StackTrace);

                    deferral?.Complete();
                    deferral = null;
                }
            });
        }

        private bool IsRedirected(HttpStatusCode statusCode)
        {
            if (statusCode == HttpStatusCode.MovedPermanently
                 || statusCode == HttpStatusCode.Moved
                 || statusCode == HttpStatusCode.Redirect
                 || statusCode == HttpStatusCode.RedirectMethod
                 || statusCode == HttpStatusCode.TemporaryRedirect)
            {
                return true;
            }

            return false;
        }

        private bool IsRouteNotFound(HttpStatusCode statusCode)
        {
            if (statusCode == HttpStatusCode.NotFound)
            {
                return true;
            }

            return false;
        }

        private bool IsBadRequest(HttpStatusCode statusCode)
        {
            if (statusCode == HttpStatusCode.BadRequest)
            {
                return true;
            }

            return false;
        }

        private string GetHomeUrl(string url)
        {
            var refererUri = CreateUri(url);
            return $"{refererUri?.Scheme}{Uri.SchemeDelimiter}{refererUri?.Host}{refererUri?.Port}";
        }

        private string FormatRedirectUrl(string url, IDictionary<string, string[]> headers)
        {
            var refererUri = CreateUri(url);

            if (headers.ContainsKey("Location"))
            {
                var location = headers["Location"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(location))
                {
                    Uri uri = CreateUri(location);
                    if (uri != null)
                    {
                        return location;
                    }

                    return $"{refererUri?.Scheme}{Uri.SchemeDelimiter}{refererUri?.Host}{refererUri?.Port}{location}";
                }
            }

            return $"{refererUri?.Scheme}{Uri.SchemeDelimiter}{refererUri?.Host}{refererUri?.Port}";
        }

        private Uri CreateUri(string url)
        {
            try
            {
                return new Uri(url);
            }
            catch { }

            return null;
        }
    }
}
