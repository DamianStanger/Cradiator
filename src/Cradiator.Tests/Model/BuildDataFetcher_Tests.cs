using System;
using System.Collections.Generic;
using System.Linq;
using Cradiator.Config;
using Cradiator.Model;
using Cradiator.Services;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace Cradiator.Tests.Model
{
	[TestFixture]
	public class BuildDataFetcher_Tests
	{
		IWebClientFactory _webClientFactory;
		IWebClient _webClient;

		[SetUp]
		public void SetUp()
		{
			_webClientFactory = Create.Stub<IWebClientFactory>();
			_webClient = Create.Mock<IWebClient>();
			_webClientFactory.Stub(w => w.GetWebClient(Arg<string>.Is.Anything)).Return(_webClient);
		}

		[Test]
		public void CanFetch()
		{
		    const string Hello = "hello";
		    _webClient.Expect(w => w.DownloadString(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(Hello);

		    var configSettings = new ConfigSettings {BuildAgentUsername = "buildAgentuser", BuildAgentPassword = "password"};
		    var fetcher = new BuildDataFetcher(new ViewUrl("http://test"), configSettings, _webClientFactory);
		    var fetchValue = fetcher.Fetch();

		    fetchValue.First().ShouldBe(Hello);
		}

		[Test]
		public void CanUpdateSettings()
		{
            _webClient.Expect(w => w.DownloadString(Arg<string>.Is.Equal("http://bla"), Arg<string>.Is.Equal("buildAgentuser"), Arg<string>.Is.Equal("password"))).Return("CanUpdateSettings1").Repeat.Once();
            _webClient.Expect(w => w.DownloadString(Arg<string>.Is.Equal("http://new"), Arg<string>.Is.Equal("buildAgentuser1"), Arg<string>.Is.Equal("password2"))).Return("CanUpdateSettings2").Repeat.Once();

			var fetcher = new BuildDataFetcher(new ViewUrl("http://bla"), new ConfigSettings
				{
					URL = "http://bla",
                    BuildAgentUsername = "buildAgentuser",
                    BuildAgentPassword = "password"
				}, 
                _webClientFactory);

		    IEnumerable<string> results = fetcher.Fetch();
            results.ToList()[0].ShouldBe("CanUpdateSettings1");

            fetcher.ConfigUpdated(new ConfigSettings
            {
                URL = "http://new",
                BuildAgentUsername = "buildAgentuser1",
                BuildAgentPassword = "password2"
            });
		    IEnumerable<string> enumerable = fetcher.Fetch();
            enumerable.ToList()[0].ShouldBe("CanUpdateSettings2");
		}

		[Test]
		public void can_fetch_multiple_urls()
		{
            _webClient.Expect(w => w.DownloadString(Arg<string>.Is.Equal("http://url1"), Arg<string>.Is.Equal("buildAgentuser"), Arg<string>.Is.Equal("password"))).Return("url1").Repeat.Once();
            _webClient.Expect(w => w.DownloadString(Arg<string>.Is.Equal("http://url2"), Arg<string>.Is.Equal("buildAgentuser"), Arg<string>.Is.Equal("password"))).Return("url2").Repeat.Once();

			var fetcher = new BuildDataFetcher(new ViewUrl("http://url1 http://url2"), new ConfigSettings(){BuildAgentUsername = "buildAgentuser", BuildAgentPassword = "password"}, _webClientFactory);

			var xmlResults = fetcher.Fetch().ToList();

			xmlResults.Count.ShouldBe(2);
			xmlResults[0].ShouldBe("url1");
			xmlResults[1].ShouldBe("url2");
		}

	}
}