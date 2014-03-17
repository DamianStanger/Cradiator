using System;
using System.Net;

namespace Cradiator.Services
{
	public class HttpWebClient : IWebClient
	{
		readonly WebClient _webClient;

		public HttpWebClient()
		{
			_webClient = new WebClient();
		}

		public string DownloadString(string url, string userName, string password)
		{
            if (!string.IsNullOrEmpty(userName) || !string.IsNullOrEmpty(password))
            {
                _webClient.Credentials = new NetworkCredential(userName, password);
            } 
            return _webClient.DownloadString(new Uri(url));
		}
	}
}