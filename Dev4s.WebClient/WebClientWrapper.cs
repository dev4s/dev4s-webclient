using System;
using System.Net;
using System.Text;

namespace Dev4s.WebClient
{
	public sealed class WebClientWrapper : IWebClientWrapper
	{
		private readonly System.Net.WebClient _webClient;

		public Encoding Encoding
		{
			get { return _webClient.Encoding; }
			set { _webClient.Encoding = value; }
		}

		public WebClientWrapper()
		{
			_webClient = new System.Net.WebClient {Encoding = Encoding.UTF8};

			//NOTE: This one needs some special checkings...
			_webClient.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)");
		}

		public string DownloadString(Uri uri)
		{
			return _webClient.DownloadString(uri);
		}
	}
}