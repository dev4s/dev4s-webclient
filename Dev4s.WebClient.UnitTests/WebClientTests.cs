using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Dev4s.WebClient.UnitTests
{
	[TestFixture]
	public class WebClientTests
	{
		#region SetUp
		private ISite _s1;
		private ISite _s2;
		private ISite _sWrongSite;

		[SetUp]
		public void SetUp()
		{
			_s1 = new Global.FakeSite(new[] { Global.GoogleUri.CreateNewUriWithPath("/a1/b1"), Global.GoogleUri.CreateNewUriWithPath("/a1/b2"),
											  Global.GoogleUri.CreateNewUriWithPath("/a2/b1"), Global.GoogleUri.CreateNewUriWithPath("/a2/b2") }, 
											  2,
											  Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"));

			_s2 = new Global.FakeSite(new[] { Global.GoogleUri.CreateNewUriWithPath("/a1/b1"), Global.GoogleUri.CreateNewUriWithPath("/a1/b2"),
											  Global.GoogleUri.CreateNewUriWithPath("/a2/b1"), Global.GoogleUri.CreateNewUriWithPath("/a2/b2") }, 
											  2,
										 	  Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"));


			_sWrongSite = new Global.FakeSite(new List<Uri>(), 2, Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"));
		}

		[TearDown]
		public void TearDown()
		{
			_s1 = _s2 = _sWrongSite = null;
		}
		#endregion

		[Test]
		public void ThrowsExceptionWhenPassingNullInsteadOfSitesToConstructor()
		{
			//Arrange
			//Act
			//Assert
			Assert.Throws<ArgumentNullException>(() => new WebClient((ISite[]) null));
			Assert.Throws<ArgumentNullException>(() => new WebClient((IWebClientWrapper) null, (ISite) null));
			Assert.Throws<ArgumentNullException>(() => new WebClient((IWebClientWrapper) null, _s1));
		}

		[Test]
		public void CheckIfWebClientWrapperIsOurFakeOne()
		{
			//Arrange
			var fakeWebClient = new Global.FakeWebClientWrapper();

			//Act
			var result1 = new WebClient(fakeWebClient);
			var result2 = new WebClient();

			//Assert
			Assert.That(result1.Wrapper, Is.EqualTo(fakeWebClient));
			Assert.That(result2.Wrapper.GetType().FullName, Is.EqualTo("Dev4s.WebClient.WebClientWrapper"));
		}

		[Test]
		public void AddingSitesShouldWorkFromConstructor()
		{
			//Arrange
			//Act
			var result1 = Global.InitializeSimpleFakeWebClient(_s1);
			var result2 = Global.InitializeSimpleFakeWebClient(_s1, _s1);

			//Assert
			Assert.That(result1.Sites, Has.Count.EqualTo(1));
			Assert.That(result2.Sites, Has.Count.EqualTo(2));
		}

		[Test]
		public void AddingSitesShouldWorkFromSitesList()
		{
			//Arrange
			var result = Global.InitializeSimpleFakeWebClient();

			//Act
			var beforeAdding = result.Sites;
			result.Sites = new List<ISite> { _s1, _s2, _s1, _s2 };

			//Assert
			Assert.That(beforeAdding, Is.Empty);
			Assert.That(result.Sites, Is.Not.Empty.And.Count.EqualTo(4));
			Assert.That(result.Sites[0], Is.EqualTo(_s1));
			Assert.That(result.Sites[1], Is.EqualTo(_s2));
			Assert.That(result.Sites[2], Is.EqualTo(_s1));
			Assert.That(result.Sites[3], Is.EqualTo(_s2));
		}

		[Test]
		public void WhenThereAreNoSitesDownloadThrowsAnException()
		{
			//Arrange
			//Act
			//Assert
			Assert.Throws<Exceptions.WebClient.ArgumentSiteException>(() => Global.InitializeSimpleFakeWebClient().DownloadAsync());
		}

		[Test]
		public void WhenSitesUrisAreEmptyThrowsAnException()
		{
			//Arrange
			//Act
			//Assert
			Assert.Throws<Exceptions.WebClient.UrisEmptyException>(() => Global.InitializeSimpleFakeWebClient(_sWrongSite).DownloadAsync());
		}

		[Test, Ignore("To do methods implementation")]
		public void DownloadShouldOnDefaultDownloadAllSite()
		{
			//Arrange
			var manualResetEvent = new ManualResetEvent(false);
			var eventFired = false;
			var webClient = Global.InitializeSimpleFakeWebClient(_s1);

			//Act
			webClient.Completed += (sender, e) =>
								{
									Assert.That(e.Result, Is.Not.Empty);
									Assert.That(e.Result, Is.All.EqualTo("<html></html>"));
									eventFired = true;
									manualResetEvent.Set();
								};
			webClient.DownloadAsync();

			//Assert
			manualResetEvent.WaitOne(500, false);
			Assert.That(webClient.IsBusy, Is.False);
			Assert.That(eventFired, Is.True);
		}

		[Test, Ignore("It needs some repairs")]
		public void ShouldThrowExceptionWhenPassingNormalOptionAndChangesMaxDegree()
		{
			////Arrange
			//var webClient = Global.InitializeSimpleFakeWebClient(_s1);

			////Act
			////Assert
			//Assert.Throws<Exceptions.WebClient.DownloadOptionsException>(() => webClient.Download(DownloadOption.Normal, 12));
		}

		[Test, Ignore("It needs some repairs")]
		public void IfGetRegexGotGroupsItReturnsSpecificListOfString()
		{
			////Arrange
			//_s1.GetRegex = new Regex("(?:<a href=\")(?<Url>.*)(?:\">)(?<Name>.*)(?:</a>)");
			//_s2.GetRegex = new Regex("<a href=\"(?<Url>.*)\">(?<Name>.*)</a>");
			//var webClient1 = Global.InitializeComplexFakeWebClient(_s1);
			//var webClient2 = Global.InitializeComplexFakeWebClient(_s2);

			////Act
			//var result1 = webClient1.Download(DownloadOption.Normal).ToList();
			//var result2 = webClient2.Download(DownloadOption.Normal).ToList();

			////Assert
			//Assert.That(new[] {result1, result2}, Is.All.Not.Empty.And.All.Count.EqualTo(16));
			//Assert.That(new[] {result1[0], result2[0]}, Is.All.EqualTo("Url:http://google.pl;Name:Google"));
			//Assert.That(new[] {result1[1], result2[1]}, Is.All.EqualTo("Url:http://allegro.pl;Name:Allegro"));
		}

		[Test, Ignore("It needs some repairs")]
		public void IfGetRegexDoesNotHaveGroupsItReturnsListOfString()
		{
			////Arrange
			//_s1.GetRegex = new Regex("<a href=\".*\">.*</a>");
			//var webClient = Global.InitializeComplexFakeWebClient(_s1);

			////Act
			//var result = webClient.Download(DownloadOption.Normal).ToList();

			////Assert
			//Assert.That(result, Is.Not.Empty.And.Count.EqualTo(16));
			//Assert.That(result[0], Is.EqualTo("<a href=\"http://google.pl\">Google</a>"));
			//Assert.That(result[1], Is.EqualTo("<a href=\"http://allegro.pl\">Allegro</a>"));
		}

		[Test, Ignore("It needs some repairs")]
		public void GetRegexOnParallelDownload()
		{
			////Arrange
			//_s1.GetRegex = new Regex("<a href=\".*\">.*</a>");
			//var webClient = Global.InitializeComplexFakeWebClient(_s1, _s1, _s1, _s1, 
			//                                                      _s1, _s1, _s1, _s1);

			////Act
			//var result = webClient.Download().ToList();

			////Assert
			//Assert.That(result, Is.Not.Empty.And.Count.EqualTo(128));
			//Assert.That(result, Has.Exactly(32).EqualTo("<a href=\"http://google.pl\">Google</a>"));
			//Assert.That(result, Has.Exactly(32).EqualTo("<a href=\"http://google.pl\">Google</a>"));
			//Assert.That(result, Has.Exactly(32).EqualTo("<a href=\"http://facebook.com\">Facebook</a>"));
			//Assert.That(result, Has.Exactly(32).EqualTo("<a href=\"http://ishootyou.com\">I shoot you</a>"));
		}

		[Test, Ignore("It needs some repairs")]
		public void DownloadShouldBreakOnSpecificRegexAndReturnAllStringsWhenGetRegexIsNotSpecified()
		{
			////Arrange
			//_s1.BreakOnRegex = new Regex("<html></html>");
			//var webClient1 = Global.InitializeComplexFakeWebClientForBreakOn(_s1);
			//var webClient2 = Global.InitializeComplexFakeWebClientForBreakOn(_s1, _s1, _s1, _s1);

			////Act
			//var result1 = webClient1.Download(DownloadOption.Normal).ToList();
			//var result2 = webClient2.Download(DownloadOption.Normal).ToList();

			////Assert
			//Assert.That(result1, Is.Not.Empty.And.Count.EqualTo(2));
			//Assert.That(result2, Is.Not.Empty.And.Count.EqualTo(8));
		}

		[Test, Ignore("It needs some repairs")]
		public void ParallelDownloadShouldBreakOnSpecificRegexAndReturnAllStringsWhenGetRegexIsNotSpecified()
		{
			////Arrange
			//_s1.BreakOnRegex = new Regex("<html></html>");
			//var webClient1 = Global.InitializeComplexFakeWebClientForBreakOn(_s1);
			//var webClient2 = Global.InitializeComplexFakeWebClientForBreakOn(_s1, _s1, _s1, _s1);

			////Act
			//var result1 = webClient1.Download().ToList();
			//var result2 = webClient2.Download().ToList();

			////Assert
			//Assert.That(result1, Is.Not.Empty.And.Count.EqualTo(2));
			//Assert.That(result2, Is.Not.Empty.And.Count.EqualTo(8));
		}

		[Test, Ignore("It needs some repairs")]
		public void IfBreakOnAndGetIsSpecifedDownloadShouldReturnSpecifiedTextsAndBreakOn()
		{
			////Arrange
			//_s1.GetRegex = new Regex("(?:<a href=\")(?<Url>.*)(?:\">)(?<Name>.*)(?:</a>)");
			//_s2.GetRegex = new Regex("<a href=\".*\">.*</a>");
			//_s1.BreakOnRegex = _s2.BreakOnRegex = new Regex("<html></html>");
			//var webClient1 = Global.InitializeComplexFakeWebClientForBreakOn(_s1);
			//var webClient2 = Global.InitializeComplexFakeWebClientForBreakOn(_s2);
			//var webClient3 = Global.InitializeComplexFakeWebClientForBreakOn(_s1, _s1, _s1, _s1);
			//var webClient4 = Global.InitializeComplexFakeWebClientForBreakOn(_s2, _s2, _s2, _s2);
			//var webClient5 = Global.InitializeComplexFakeWebClientForBreakOn(_s1, _s1, _s2, _s2);

			////Act
			//var result1 = webClient1.Download(DownloadOption.Normal).ToList();
			//var result2 = webClient2.Download(DownloadOption.Normal).ToList();
			//var result3 = webClient3.Download(DownloadOption.Normal).ToList();
			//var result4 = webClient4.Download(DownloadOption.Normal).ToList();
			//var result5 = webClient5.Download(DownloadOption.Normal).ToList();
			
			////Assert
			//Assert.That(new[] {result1, result2, result3, result4, result5}, Has.All.Not.Empty);

			//Assert.That(new[] {result1, result2}, Has.All.Count.EqualTo(8));
			//Assert.That(new[] {result3, result4, result5}, Has.All.Count.EqualTo(32));

			//Assert.That(result1[0], Is.EqualTo("Url:http://google.pl;Name:Google"));
			//Assert.That(result1[1], Is.EqualTo("Url:http://allegro.pl;Name:Allegro"));

			//Assert.That(result2[0], Is.EqualTo("<a href=\"http://google.pl\">Google</a>"));
			//Assert.That(result2[1], Is.EqualTo("<a href=\"http://allegro.pl\">Allegro</a>"));

			//Assert.That(result3[0], Is.EqualTo("Url:http://google.pl;Name:Google"));
			//Assert.That(result3[31], Is.EqualTo("Url:http://ishootyou.com;Name:I shoot you"));

			//Assert.That(result4[0], Is.EqualTo("<a href=\"http://google.pl\">Google</a>"));
			//Assert.That(result4[31], Is.EqualTo("<a href=\"http://ishootyou.com\">I shoot you</a>"));

			//Assert.That(result5[0], Is.EqualTo("Url:http://google.pl;Name:Google"));
			//Assert.That(result5[31], Is.EqualTo("<a href=\"http://ishootyou.com\">I shoot you</a>"));
		}

		[Test, Ignore("It needs some repairs")]
		public void ParallelIfBreakOnAndGetIsSpecifedDownloadShouldReturnSpecifiedTextsAndBreakOn()
		{
			////Arrange
			//_s1.GetRegex = new Regex("(?:<a href=\")(?<Url>.*)(?:\">)(?<Name>.*)(?:</a>)");
			//_s2.GetRegex = new Regex("<a href=\".*\">.*</a>");
			//_s1.BreakOnRegex = _s2.BreakOnRegex = new Regex("<html></html>");
			//var webClient1 = Global.InitializeComplexFakeWebClientForBreakOn(_s1);
			//var webClient2 = Global.InitializeComplexFakeWebClientForBreakOn(_s2);
			//var webClient3 = Global.InitializeComplexFakeWebClientForBreakOn(_s1, _s1, _s1, _s1);
			//var webClient4 = Global.InitializeComplexFakeWebClientForBreakOn(_s2, _s2, _s2, _s2);
			//var webClient5 = Global.InitializeComplexFakeWebClientForBreakOn(_s1, _s1, _s2, _s2);

			////Act
			//var result1 = webClient1.Download().ToList();
			//var result2 = webClient2.Download().ToList();
			//var result3 = webClient3.Download().ToList();
			//var result4 = webClient4.Download().ToList();
			//var result5 = webClient5.Download().ToList();
			
			////Assert
			//Assert.That(new[] {result1, result2, result3, result4, result5}, Has.All.Not.Empty);

			//Assert.That(new[] {result1, result2}, Has.All.Count.EqualTo(8));
			//Assert.That(new[] {result3, result4, result5}, Has.All.Count.EqualTo(32));

			//Assert.That(result1[0], Is.EqualTo("Url:http://google.pl;Name:Google"));
			//Assert.That(result1[1], Is.EqualTo("Url:http://allegro.pl;Name:Allegro"));

			//Assert.That(result2[0], Is.EqualTo("<a href=\"http://google.pl\">Google</a>"));
			//Assert.That(result2[1], Is.EqualTo("<a href=\"http://allegro.pl\">Allegro</a>"));

			//Assert.That(result3[0], Is.EqualTo("Url:http://google.pl;Name:Google"));
			//Assert.That(result3[31], Is.EqualTo("Url:http://ishootyou.com;Name:I shoot you"));

			//Assert.That(result4[0], Is.EqualTo("<a href=\"http://google.pl\">Google</a>"));
			//Assert.That(result4[31], Is.EqualTo("<a href=\"http://ishootyou.com\">I shoot you</a>"));

			//Assert.That(result5[0], Is.EqualTo("Url:http://google.pl;Name:Google"));
			//Assert.That(result5[31], Is.EqualTo("<a href=\"http://ishootyou.com\">I shoot you</a>"));

			////Assert.That(result, Has.Exactly(32).EqualTo("<a href=\"http://google.pl\">Google</a>"));
			////Assert.That(result, Has.Exactly(32).EqualTo("<a href=\"http://google.pl\">Google</a>"));
			////Assert.That(result, Has.Exactly(32).EqualTo("<a href=\"http://facebook.com\">Facebook</a>"));
			////Assert.That(result, Has.Exactly(32).EqualTo("<a href=\"http://ishootyou.com\">I shoot you</a>"));
			////Assert.That(result1, Is.Not.Empty.And.Count.EqualTo(2));
			////Assert.That(result2, Is.Not.Empty.And.Count.EqualTo(8));
			//////Assert
			////Assert.That(new[] { result1, result2 }, Is.All.Not.Empty.And.All.Count.EqualTo(16));
			////Assert.That(new[] { result1[0], result2[0] }, Is.All.EqualTo("Url:http://google.pl;Name:Google"));
			////Assert.That(new[] { result1[1], result2[1] }, Is.All.EqualTo("Url:http://allegro.pl;Name:Allegro"));
		}

		[Test]
		public void CheckIfWeCanChangeEncoding()
		{
			//Arrange
			var fakeWrapper = new Global.FakeWebClientWrapper();
			var webClient = new WebClient(fakeWrapper);

			//Act
			var beforeChange = webClient.Encoding;
			webClient.Encoding = Encoding.ASCII;

			//Assert
			Assert.That(beforeChange, Is.Null);
			Assert.That(beforeChange, Is.Not.EqualTo(fakeWrapper.Encoding));
			Assert.That(webClient.Encoding, Is.EqualTo(Encoding.ASCII));
		}

		[Test, Ignore("It needs some repairs")]
		public void DownloadAsSiteShouldReturnOneSiteItemWithManyUris()
		{
			////Arrange
			//_s1.GetRegex = new Regex("(?:<a href=\")(?<Url>.*)(?:\">)(?<Name>.*)(?:</a>)");
			//_s2.GetRegex = new Regex("(?:<a href=\")(?<DifferentName>.*)(?:\">)(?<Name>.*)(?:</a>)");
			//var webClient1 = Global.InitializeComplexFakeWebClient(_s1);
			//var webClient2 = Global.InitializeComplexFakeWebClient(_s1, _s1, _s1, _s1);
			//var webClient3 = Global.InitializeComplexFakeWebClient(_s2);
			//var webClient4 = Global.InitializeComplexFakeWebClient(_s2, _s2, _s2, _s2);
			
			////Act
			//var result1 = webClient1.DownloadAsSite(DownloadOption.Normal);
			//var result2 = webClient2.DownloadAsSite(DownloadOption.Normal);
			//var result3 = webClient3.DownloadAsSite("DifferentName", DownloadOption.Normal);
			//var result4 = webClient4.DownloadAsSite("DifferentName", DownloadOption.Normal);

			////Assert
			//Assert.That(new[] {result1.Uris, result2.Uris, result3.Uris, result4.Uris}, Is.All.Not.Empty);
			//Assert.That(result1.Uris, Has.Count.EqualTo(16));
			//Assert.That(result2.Uris, Has.Count.EqualTo(64));
			//Assert.That(result3.Uris, Has.Count.EqualTo(16));
			//Assert.That(result4.Uris, Has.Count.EqualTo(64));
			//Assert.That(new[] {result1.Uris[0], result2.Uris[0], result3.Uris[0], result4.Uris[0]}, Is.All.EqualTo(new Uri("http://google.pl")));
		}

		[Test, Ignore("It needs some repairs")]
		public void ParallelDownloadAsSiteShouldReturnOneSiteItemWithManyUris()
		{
			////Arrange
			//_s1.GetRegex = new Regex("(?:<a href=\")(?<Url>.*)(?:\">)(?<Name>.*)(?:</a>)");
			//_s2.GetRegex = new Regex("(?:<a href=\")(?<DifferentName>.*)(?:\">)(?<Name>.*)(?:</a>)");
			//var webClient1 = Global.InitializeComplexFakeWebClient(_s1);
			//var webClient2 = Global.InitializeComplexFakeWebClient(_s1, _s1, _s1, _s1);
			//var webClient3 = Global.InitializeComplexFakeWebClient(_s2);
			//var webClient4 = Global.InitializeComplexFakeWebClient(_s2, _s2, _s2, _s2);
			
			////Act
			//var result1 = webClient1.DownloadAsSite();
			//var result2 = webClient2.DownloadAsSite();
			//var result3 = webClient3.DownloadAsSite("DifferentName");
			//var result4 = webClient4.DownloadAsSite("DifferentName");

			////Assert
			//Assert.That(new[] { result1.Uris, result2.Uris, result3.Uris, result4.Uris }, Is.All.Not.Empty);
			//Assert.That(result1.Uris, Has.Count.EqualTo(16));
			//Assert.That(result2.Uris, Has.Count.EqualTo(64));
			//Assert.That(result3.Uris, Has.Count.EqualTo(16));
			//Assert.That(result4.Uris, Has.Count.EqualTo(64));
			//Assert.That(new[] { result1.Uris[0], result2.Uris[0], result3.Uris[0], result4.Uris[0] }, Is.All.EqualTo(new Uri("http://google.pl")));
		}

		[Test, Ignore("It needs some repairs")]
		public void DownloadAsSiteShouldThrowExceptionWhenOneOfTheResultsIsNotConvertableToUri()
		{
			////Arrange
			//_s1.GetRegex = new Regex("(?:<a href=\")(?<Url>.*)(?:\">)(?<Name>.*)(?:</a>)");
			//_s2.GetRegex = new Regex("(?:<a href=\")(?<DifferentName>.*)(?:\">)(?<Name>.*)(?:</a>)");
			//var webClient = Global.InitializeComplexFakeWebClient(_s1, _s2);

			////Act
			////Assert
			//Assert.Throws<Exceptions.Extension.UriConversionException>(() => webClient.DownloadAsSite(DownloadOption.Normal));
		}

		[Test, Ignore("It needs some repairs")]
		public void ParallelDownloadAsSiteShouldThrowExceptionWhenOneOfTheResultsIsNotConvertableToUri()
		{
			////Arrange
			//_s1.GetRegex = new Regex("(?:<a href=\")(?<Url>.*)(?:\">)(?<Name>.*)(?:</a>)");
			//_s2.GetRegex = new Regex("(?:<a href=\")(?<DifferentName>.*)(?:\">)(?<Name>.*)(?:</a>)");
			//var webClient = Global.InitializeComplexFakeWebClient(_s1, _s2);

			////Act
			////Assert
			//Assert.Throws<Exceptions.Extension.UriConversionException>(() => webClient.DownloadAsSite());
		}

		[Test, Ignore]
		public void ReturningTypeShouldBeTheSameAsGroupsInGetRegex()
		{
			//Arrange

			//Act

			//Assert

		}

		[Test, Ignore]
		public void CheckIfThereIsSomeDataInEvents()
		{
			//Arrange

			//Act

			//Assert

		}

		[Test, Ignore]
		public void WhenWebClientInThreadsThrowsAnExceptionItShowsInErrors()
		{
			//WebException
			//NotSupportedException
			//Exception
		}
	}
}