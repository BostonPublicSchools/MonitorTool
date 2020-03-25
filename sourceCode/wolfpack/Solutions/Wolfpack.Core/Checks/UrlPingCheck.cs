using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using System.Linq;
using Wolfpack.Core.Notification;
using Wolfpack.Core.Notification.Filters.Request;
using Magnum.Extensions;

namespace Wolfpack.Core.Checks
{
    public class UrlPingCheckConfig : PluginConfigBase, ISupportNotificationMode, ISupportNotificationThreshold
    {
        /// <summary>
        /// A regex that the content returned from the ping should match - leave 
        /// blank to omit this check
        /// </summary>
        public string ContentRegex { get; set; }

        /// <summary>
        /// Set to true to replicate a call from XMLHttpRequest
        /// </summary>
        public bool IsAjaxCall { get; set; }

        /// <summary>
        /// Set to true to allows Wolfpack to attach it's default service account credentials
        /// to the request or false for an anonymous unauthenticated request
        /// </summary>
        public bool UseDefaultCredentials { get; set; }

        /// <summary>
        /// A list of Urls to ping
        /// </summary>
        public List<string> Urls { get; set;}

        /// <summary>
        /// Set this to insure that you get the correct version of the URL Data that you want.
        /// in some cases you want the Moble version or maybe the version for the ipad. Some sites
        /// and network firewalls/proxies will not allow a requet through if the User agent is not 
        /// a know type.
        /// </summary>
        public string UserAgentString { get; set; }

        /// <summary>
        /// This allows you to set headers for the call(s)
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// default ctor
        /// </summary>
        public UrlPingCheckConfig()
        {
            UserAgentString = "Wolfpack.Agent";
        }

        /// <summary>
        /// If you need to make the ping a POST request then set this to the value you want to POST to the Url.
        /// Leave blank to ensure the ping is a GET.
        /// </summary>
        public string PostData { get; set; }

        public double? NotificationThreshold { get; set; }
        public string NotificationMode { get; set; }        
    }

    public class UrlPingConfigurationAdvertiser : HealthCheckDiscoveryBase<UrlPingCheckConfig, UrlPingCheck>
    {
        protected override UrlPingCheckConfig GetConfiguration()
        {
            return new UrlPingCheckConfig
                       {
                           Enabled = true,
                           FriendlyId = "CHANGEME!",
                           ContentRegex = "Content Regex or blank",
                           NotificationMode = StateChangeNotificationFilter.FilterName,
                           NotificationThreshold = 5000,
                           UseDefaultCredentials = false,
                           IsAjaxCall = false,
                           PostData = "The data to POST or blank to do a GET ping",
                           Headers = new Dictionary<string, string>
                                         {
                                             {"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"}
                                         },
                           Urls = new List<string>
                                      {
                                          "http://wolfpack.codeplex.com",
                                          "http://jimblogdog.blogspot.com"
                                      },
                           UserAgentString = "CHANGEME or leave blank"
                       };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "UrlPingCheck";
            entry.Description = "This check will ping a set of urls (including data apis; use Headers property to control Accept type) and " + 
                "raise notifications if the ping fails or is too slow. You can control the failure threshold using the 'NotificationThreshold' " + 
                "property of the configuration; set it to the number of milliseconds that the endpoint must respond by.";
            entry.Tags.AddIfMissing("Url", "Ping", "Threshold");
        }
    }

    /// <summary>
    /// This will make a call to a set of Urls specified in the <see cref="UrlPingCheckConfig"/>
    /// configuration. If the call fails it will publish a failed result. If you have set the optional
    /// "FailIfResponseMillisecondsOver" property in the configuration and the response takes longer
    /// than this value a failed result is also published. NOTE: Each url is treated separately and you
    /// will receive a result for EACH ONE that fails.
    /// </summary>
    public class UrlPingCheck : ThresholdCheckBase<UrlPingCheckConfig>
    {
        /// <summary>
        /// default ctor
        /// </summary>
        public UrlPingCheck(UrlPingCheckConfig config) 
            : base(config)
        {
        }
       
        public override void Initialise()
        {
            Logger.Debug("Initialising UrlPingCheck check for...");
            _config.Urls.ForEach(url => Logger.Debug("\t{0}", url));
        }

        public override void Execute()
        {
            Logger.Debug("UrlPingCheck is pinging urls...");
            _config.Urls.ToList().ForEach(
                url =>
                    {
                        var wc = (HttpWebRequest)WebRequest.Create(url);

                        {
                            try
                            {
                                wc.AllowAutoRedirect = true;
                                wc.UseDefaultCredentials = _config.UseDefaultCredentials;

                                if (!string.IsNullOrWhiteSpace(_config.UserAgentString))
                                    wc.UserAgent = _config.UserAgentString;

                                if (_config.Headers != null)
                                {
                                    foreach (var header in _config.Headers)
                                    {
                                        if (!HandledAsProtectedHeader(wc, header))
                                            wc.Headers.Add(header.Key, header.Value);
                                    }                                    
                                }

                                if (_config.IsAjaxCall)
                                    wc.Headers.Add("X-Requested-With", "XMLHttpRequest");

                                string response;
                                var timer = Stopwatch.StartNew();

                                if (string.IsNullOrWhiteSpace(_config.PostData))
                                {
                                    Logger.Debug("Timing ping to '{0}'...", url);

                                    wc.Method = "GET";
                                    response = DownloadString(wc);
                                }
                                else
                                {
                                    Logger.Debug("Timing ping (POST) to '{0}'...", url);

                                    wc.Method = "POST";
                                    wc.ContentType = "application/x-www-form-urlencoded";
                                    
                                    var streamUp = wc.GetRequestStream();                                   
                                    var dataUp = Encoding.UTF8.GetBytes(_config.PostData);
                                    wc.ContentLength = dataUp.Length;

                                    streamUp.Write(dataUp, 0, dataUp.Length);
                                    response = DownloadString(wc);
                                }                                                                
                                timer.Stop();
                                Logger.Debug("Ping '{0}' = {1}ms", url, timer.ElapsedMilliseconds);

                                var result = true;
                                var regexMsg = string.Empty;

                                if (!string.IsNullOrWhiteSpace(_config.ContentRegex))
                                {
                                    result = Regex.IsMatch(response, _config.ContentRegex);
                                    regexMsg = " - **Failed, Response did not match Regex**";
                                }

                                var sizeInBytes = Encoding.UTF8.GetByteCount(response).ToString(CultureInfo.InvariantCulture);

                                Publish(NotificationRequestBuilder.For(_config.NotificationMode, HealthCheckData.For(Identity,
                                    "Pinged url '{0}' (Threshold is {1}ms; returned {2}bytes){3}", 
                                        url, _config.NotificationThreshold ?? 0, sizeInBytes, regexMsg)
                                    .ResultIs(result)
                                    .DisplayUnitIs("ms")
                                    .AddProperty("url", url)
                                    .AddProperty("bytes", sizeInBytes)
                                    .ResultCountIs(timer.ElapsedMilliseconds), BuildKeyFromUrl)
                                    .Build());
                            }
                            catch (WebException wex)
                            {
                                var extraInfo = string.Empty;

                                if (wex.Status == WebExceptionStatus.ProtocolError)
                                {
                                    extraInfo = string.Format(", Http state: {0}, '{1}'",
                                        (int) ((HttpWebResponse) wex.Response).StatusCode,
                                        ((HttpWebResponse) wex.Response).StatusDescription);
                                }

                                Publish(NotificationRequestBuilder.For(_config.NotificationMode, HealthCheckData.For(Identity,
                                    "Url '{0}' failed with code '{1}'{2}", url, wex.Status, extraInfo)
                                    .Failed()
                                    .AddProperty("url", url), BuildKeyFromUrl)
                                    .Build());
                            }
                        }
                    });
        }

        private static bool HandledAsProtectedHeader(HttpWebRequest wc, KeyValuePair<string, string> header)
        {
            var name = header.Key.ToLower();

            switch (name)
            {
                case "Date":
                case "Range":
                case "Connection":
                case "Transfer-Encoding":
                case "Content-Length":
                case "If-Modified-Since": 
                    Logger.Warning("{0} header not supported!", name);
                    break;

                case "accept":
                    wc.Accept = header.Value;
                    break;
                case "content-type":
                    wc.ContentType = header.Value;
                    break;
                case "expect":
                    wc.Expect = header.Value;
                    break;
                case "host":
                    wc.Host = header.Value;
                    break;
                case "referer":
                    wc.Referer= header.Value;
                    break;
                case "user-agent":
                    wc.UserAgent = header.Value;
                    break;

                default:
                    // not protected
                    return false;
            }

            // handled as protected
            return true;
        }

        private static string DownloadString(WebRequest wc)
        {
            using (var streamDown = wc.GetResponse().GetResponseStream())
            {
                return streamDown.ReadToEndAsText();
            }
        }


        private static void BuildKeyFromUrl(NotificationRequest request)
        {
            request.DataKeyGenerator = (message => string.Format("{0}_{1}", message.CheckId,
                message.Properties["url"]));
        }

        protected override PluginDescriptor BuildIdentity()
        {
            return new PluginDescriptor
                       {
                           Description = "Pings a url and reports Http code and response time",
                           TypeId = new Guid("54246DEB-36F2-4e9b-9BFA-B75BF40A8B7A"),
                           Name = _config.FriendlyId
                       };
        }
    }
}
