using System;
using System.Text;

namespace Dev4s.WebClient
{
	public interface IWebClientWrapper
	{
		Encoding Encoding { get; set; }
		string DownloadString(Uri uri);
	}
}