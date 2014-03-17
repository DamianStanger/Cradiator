using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Cradiator.Config;
using Cradiator.Services;

namespace Cradiator.Model
{
	public class BuildDataFetcher : IConfigObserver
	{
		readonly ViewUrl _viewUrl;
	    private string buildUsername;
	    private string buildPassword;
		readonly IWebClientFactory _webClientFactory;
		IWebClient _webClient;

		public BuildDataFetcher(ViewUrl viewUrl, IConfigSettings configSettings,
								IWebClientFactory webClientFactory)
		{
			_viewUrl = viewUrl;
			_webClientFactory = webClientFactory;
			_webClient = webClientFactory.GetWebClient(configSettings.URL);
		    buildUsername = configSettings.BuildAgentUsername;
		    buildPassword = configSettings.BuildAgentPassword;
			configSettings.AddObserver(this);
		}

		public IEnumerable<string> Fetch()
		{
		    var enumerable = new List<string>();
		    foreach (var url in _viewUrl.UriList)
		    {
                enumerable.Add(_webClient.DownloadString(url, buildUsername, buildPassword));
		    }
		    return enumerable;
		}

	    public void ConfigUpdated(ConfigSettings newSettings)
		{
			_viewUrl.Url = newSettings.URL;
		    buildUsername = newSettings.BuildAgentUsername;
		    buildPassword = newSettings.BuildAgentPassword;
			_webClient = _webClientFactory.GetWebClient(newSettings.URL);
		}
	}
}