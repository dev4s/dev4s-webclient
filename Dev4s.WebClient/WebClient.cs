using System.Collections.Generic;
using System.Text;

namespace Dev4s.WebClient
{
	//TODO: methods descriptions!!!!!!!
	//TODO: random waits between downloads
	//TODO: specific waits between downloads
	//TODO: async with events ;)
	//TODO: after finishing - make interface for this class (for further tests in some other libs/apps)
	//FEATURE: DownloadFiles
	//FEATURE: Download - Instead of returning common interface - return our interface with items, warnings, errors, etc.
	//TODO: EVENTS!
	public sealed partial class WebClient
	{
		#region Properties
		public IWebClientWrapper Wrapper
		{
			get { return _webClientWrapper; }
		}
		public Encoding Encoding
		{
			get { return _webClientWrapper.Encoding; }
			set { _webClientWrapper.Encoding = value.CheckForNull(); }
		}
		public IList<ISite> Sites { get; set; }
		#endregion

		#region Private properties
		private readonly IWebClientWrapper _webClientWrapper;
		#endregion

		#region Constructors
		private WebClient()
		{
			Sites = new List<ISite>();
		}

		public WebClient(params ISite[] sites) 
			: this()
		{
			Sites.AddRange(sites.CheckForNull());
			_webClientWrapper = new WebClientWrapper();
		}

		public WebClient(IWebClientWrapper webClientWrapper, params ISite[] sites) 
			: this(sites)
		{
			_webClientWrapper = webClientWrapper.CheckForNull();
		}
		#endregion
	}
}