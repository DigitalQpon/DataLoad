using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

namespace QponCollector
{
    class MyWebClient : WebClient
    {
        int _timeOutInMinutes = 0;
        public int TimeOutInMinutes { set { _timeOutInMinutes = value * 1000 * 60; } }
        protected override WebRequest GetWebRequest(Uri address)
        {
            var _webReq = base.GetWebRequest(address);
            _webReq.Timeout = _timeOutInMinutes;
            return _webReq;
        }
    }

    class BaseWebConn
    {

        public static String doFetchJsonData(Uri pForUrl)
        {
            using (WebClient _web = new WebClient())
            {
                return _web.DownloadString(pForUrl);
            }
        }

        public static void doFetchJsonDataAsync(Uri pForUrl, DownloadStringCompletedEventHandler pHandler)
        {
            using (MyWebClient _web = new MyWebClient())
            {
                _web.TimeOutInMinutes = 25; // todo: timeout in minutes...change according to use
                _web.DownloadStringCompleted += pHandler;
                Console.WriteLine("Download from " + pForUrl.ToString());
                _web.DownloadStringAsync(pForUrl, _web);
            }
        }

        public static Uri BuildUriFrom(String pBaseUrl, Dictionary<String, Object> pQuery)
        {
            UriBuilder _url = new UriBuilder(pBaseUrl);
            if (pQuery.Count > 0)
            {
                foreach (KeyValuePair<String, Object> _kp in pQuery)
                {
                    if (_kp.Value != null && !String.IsNullOrEmpty(_kp.ToString())) {
                        if (_url.Query != null && _url.Query.Length > 1) 
                            _url.Query = String.Format("{0}&{1}={2}", _url.Query.Substring(1), _kp.Key, _kp.Value.ToString());
                        else
                            _url.Query = String.Format("{0}={1}", _kp.Key, _kp.Value.ToString());
                    }
                }
            }
            return _url.Uri;
        }
    }
}
