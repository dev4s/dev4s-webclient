using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Dev4s.WebClient
{
	public delegate void WebClientEventHandler(object sender, WebClientEventArgs e);

	public sealed class WebClientEventArgs : System.EventArgs
	{
		public IEnumerable<string> Result
		{
			get { return new ReadOnlyCollection<string>(_result); }
		}

		private readonly IList<string> _result;

		public WebClientEventArgs(IList<string> result)
		{
			_result = result;
		}
	}
}