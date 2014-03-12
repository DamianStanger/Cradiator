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

		public string DownloadString(string url)
		{
            _webClient.Credentials = new NetworkCredential("goagent", "password");
			return _webClient.DownloadString(new Uri(url));
		}
	}
}