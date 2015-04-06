using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Dev4s.WebClient
{
	public interface ISite
	{
		Regex GetRegex { get; set; }
		Regex BreakOnRegex { get; set; }
		ObservableCollection<ISearchRange> SearchRanges { get; set; }
		Uri MainUri { get; set; }
		ReadOnlyCollection<Uri> Uris { get; }
		int UriParametersCount { get; }
	}
}