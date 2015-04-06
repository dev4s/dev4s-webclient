using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Dev4s.WebClient.UnitTests
{
	internal static class Global
	{
		internal class FakeWebClientWrapper : IWebClientWrapper
		{
			public Encoding Encoding { get; set; }

			public string DownloadString(Uri uri)
			{
				return "<html></html>";
			}
		}

		private class ComplexFakeWebClientWrapper : IWebClientWrapper
		{
			public Encoding Encoding { get; set; }

			public string DownloadString(Uri uri)
			{
				return "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">\r\n" +
						"<html lang=\"en\">\r\n" +
						"<head>\r\n" +
							"<meta http-equiv=\"Content-Type\" content=\"text/html;charset=UTF-8\">\r\n" +
							"<title></title>\r\n" +
						"</head>\r\n" +
						"<body>\r\n" +
							"<a href=\"http://google.pl\">Google</a>\r\n" +
							"<a href=\"http://allegro.pl\">Allegro</a>\r\n" +
							"<a href=\"http://facebook.com\">Facebook</a>\r\n" +
							"<a href=\"http://ishootyou.com\">I shoot you</a>\r\n" +
						"</body>\r\n" +
						"</html>\r\n";
			}
		}

		private class ComplexFakeWebClientWrapperForBreakOnRegex : IWebClientWrapper
		{
			public Encoding Encoding { get; set; }

			private int _counter;
			public string DownloadString(Uri uri)
			{
				while (Interlocked.CompareExchange(ref _counter, 0, 2) != 2)
				{
					Interlocked.Increment(ref _counter);
					return new ComplexFakeWebClientWrapper().DownloadString(uri);
				}
				return "<html></html>";
			}
		}

		internal class FakeSite : ISite
		{
			public Regex GetRegex { get; set; }
			public Regex BreakOnRegex { get; set; }
			public ObservableCollection<ISearchRange> SearchRanges { get; set; }
			public Uri MainUri { get; set; }

			private readonly IList<Uri> _uris; 
			public ReadOnlyCollection<Uri> Uris
			{
				get { return new ReadOnlyCollection<Uri>(_uris); }
			}

			private readonly int _uriParametersCount;
			public int UriParametersCount
			{
				get { return _uriParametersCount; }
			}

			public FakeSite(IList<Uri> uris, int uriParametersCount, Uri mainUri)
			{
				_uris = uris;
				_uriParametersCount = uriParametersCount;
				MainUri = mainUri;
			}
		}

		internal class FakeSearchRange : ISearchRange
		{
			public IList<string> Items { get; private set; }

			public FakeSearchRange(IList<string> items)
			{
				Items = items;
			}
		}

		internal static readonly Uri GoogleUri = new Uri("http://google.com");

		internal static readonly List<string> Letters = new List<string>
		                                                	{
			                  									"a", "b", "c", "d", "e", "f", "g", "h", "i", 
																"j", "k", "l", "m", "n", "o", "p", "q", "r", 
																"s", "t", "u", "v", "w", "x", "y", "z"
			                  								};

		internal static readonly List<string> PolishLetters = new List<string>
		                                                      	{
																	  "ą", "ć", "ę", "ł", "ń", "ó", "ś", "ż", "ź"
																};

		internal static WebClient InitializeComplexFakeWebClientForBreakOn(params ISite[] sites)
		{
			return new WebClient(new ComplexFakeWebClientWrapperForBreakOnRegex(), sites);
		}

		internal static WebClient InitializeComplexFakeWebClient(params ISite[] sites)
		{
			return new WebClient(new ComplexFakeWebClientWrapper(), sites);
		}

		internal static WebClient InitializeSimpleFakeWebClient(params ISite[] sites)
		{
			return new WebClient(new FakeWebClientWrapper(), sites);
		}

		internal static Uri CreateNewUriWithPath(this Uri uri, string path)
		{
			return new UriBuilder(uri) { Path = path }.Uri;
		}
	}
}