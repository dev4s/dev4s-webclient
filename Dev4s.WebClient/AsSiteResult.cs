using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Dev4s.WebClient
{
	internal class AsSiteResult : ISite
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

		public int UriParametersCount
		{
			get { return -1; }
		}

		public AsSiteResult(ICollection<Uri> uris)
		{
			_uris = new List<Uri>(uris.Count);
			_uris.AddRange(uris);
		}
	}
}