using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Dev4s.WebClient
{
	public sealed partial class WebClient
	{
		#region Properties
		public bool IsBusy
		{
			get { return _isBusy; }
		}

		public bool UseParallel { get; set; }
		#endregion

		#region Private properties
		private bool _isBusy;
		#endregion

		#region Downloading
		public void DownloadAsync()
		{
			Preconditions.CheckSpecialPreconditions(Sites);
			_isBusy = true;

			Task.Factory.StartNew(Download);
		}

		public void DownloadAsSiteAsync()
		{
			throw new NotImplementedException();
		}

		public void StopDownload()
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Events
		public event WebClientEventHandler Completed;
		public event WebClientEventHandler Progress;
		public event WebClientEventHandler Stopped;

		private void OnCompleted(WebClientEventArgs e)
		{
			Completed.SafeInvoke(this, e);
		}

		internal void OnProgress(WebClientEventArgs e)
		{
			Progress.SafeInvoke(this, e);
		}

		internal void OnStopped(WebClientEventArgs e)
		{
			Stopped.SafeInvoke(this, e);
		}
		#endregion

		#region Private methods
		private void Download()
		{
			var siteCount = Sites.Count;
			var urisCount = Sites.Sum(x => x.Uris.Count);
			var moreSites = siteCount > urisCount;

			var result = new BlockingCollection<string>();

			if (UseParallel)
			{
				//Parallel
				var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = urisCount <= 8 ? 1 : 6 };

				if (moreSites)
				{
					Parallel.ForEach(Sites, parallelOptions, (site, loopState) =>
					{
						foreach (var uri in site.Uris)
						{
							//if (AddToResult(result, uri, site.GetRegex, site.BreakOnRegex, uriGroup, downloadOption))
							//{
							//    loopState.Break();
							//}
						}
					});
				}
				else
				{
					foreach (var site in Sites)
					{
						var tempSite = site;
						Parallel.ForEach(site.Uris, parallelOptions, (uri, loopState) =>
						{
							//if (AddToResult(result, uri, tempSite.GetRegex, tempSite.BreakOnRegex, uriGroup, downloadOption))
							//{
							//    loopState.Break();
							//}
						});
					}
				}
			}
			else
			{
				//foreach (var site in Sites)
				//{
				//    foreach (var uri in site.Uris)
				//    {
				//        if (AddToResult(result, uri, site.GetRegex, site.BreakOnRegex, uriGroup, downloadOption))
				//        {
				//            break;
				//        }
				//    }
				//}
			}

			_isBusy = false;
			OnCompleted(new WebClientEventArgs(result.ToList()));
		}
		#endregion

		//public IEnumerable<string> Download(DownloadOption downloadOption = DownloadOption.Parallel, int degreeOfParallelism = 6)
		//{
		//    return Download(null, downloadOption, degreeOfParallelism);
		//}

		//public ISite DownloadAsSite(DownloadOption downloadOption = DownloadOption.Parallel, int degreeOfParallelism = 6)
		//{
		//    return DownloadAsSite("Url", downloadOption, degreeOfParallelism);
		//}

		//public ISite DownloadAsSite(string uriGroup, DownloadOption downloadOption = DownloadOption.Parallel, int degreeOfParallelism = 6)
		//{
		//    return Download(uriGroup, downloadOption, degreeOfParallelism).ToList().AsSite();
		//}

		//public IEnumerable<T> Download<T>() where T : class
		//{
		//    throw new NotImplementedException();
		//}

		//NOTE: maybe we can do it as event?
		//public void DownloadToDatabase()
		//{
		//    throw new NotImplementedException();
		//}

		//NOTE: This will be made as a property
		//public void WaitBetweenDownloads()
		//{
		//    throw new NotImplementedException();
		//}


		//private IEnumerable<string> Download(string uriGroup, DownloadOption downloadOption, int degreeOfParallelism)
		//{
		//    Preconditions.CheckSpecialPreconditions(Sites, downloadOption, degreeOfParallelism);

		//    var siteCount = Sites.Count;
		//    var urisCount = Sites.Sum(x => x.Uris.Count);
		//    var moreSites = siteCount > urisCount;

		//    var result = new BlockingCollection<string>();

		//    switch (downloadOption)
		//    {
		//        case DownloadOption.Parallel:
		//            var parallelOptions = urisCount <= 8
		//                                ? new ParallelOptions { MaxDegreeOfParallelism = 1 }
		//                                : new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism };

		//            if (moreSites)
		//            {
		//                Parallel.ForEach(Sites, parallelOptions, (site, loopState) =>
		//                {
		//                    foreach (var uri in site.Uris)
		//                    {
		//                        if (AddToResult(result, uri, site.GetRegex, site.BreakOnRegex, uriGroup, downloadOption))
		//                        {
		//                            loopState.Break();
		//                        }
		//                    }
		//                });
		//            }
		//            else
		//            {
		//                foreach (var site in Sites)
		//                {
		//                    var tempSite = site;
		//                    Parallel.ForEach(site.Uris, parallelOptions, (uri, loopState) =>
		//                    {
		//                        if (AddToResult(result, uri, tempSite.GetRegex, tempSite.BreakOnRegex, uriGroup, downloadOption))
		//                        {
		//                            loopState.Break();
		//                        }
		//                    });
		//                }
		//            }
		//            break;

		//        case DownloadOption.Normal:
		//            foreach (var site in Sites)
		//            {
		//                foreach (var uri in site.Uris)
		//                {
		//                    if (AddToResult(result, uri, site.GetRegex, site.BreakOnRegex, uriGroup, downloadOption))
		//                    {
		//                        break;
		//                    }
		//                }
		//            }
		//            break;
		//    }

		//    return result;
		//} 

		//private bool AddToResult(BlockingCollection<string> result, Uri uri, Regex getRegex, Regex breakOnRegex, string uriGroup, DownloadOption downloadOption)
		//{
		//    var downloadedText = _webClientWrapper.DownloadString(uri);
		//    var resultText = new StringBuilder();
		//    var addFullText = true;

		//    if (breakOnRegex != null && breakOnRegex.IsMatch(downloadedText))
		//    {
		//        return true;
		//    }

		//    if (getRegex != null)
		//    {
		//        GenerateResultsFromGetRegex(result, resultText, getRegex, downloadedText, uriGroup);
		//        addFullText = false;
		//    }

		//    if (addFullText)
		//    {
		//        result.Add(downloadedText);
		//    }

		//    return false;
		//}

		//private static void GenerateResultsFromGetRegex(BlockingCollection<string> result, StringBuilder resultText, Regex getRegex,
		//                                                string downloadedText, string uriGroup)
		//{
		//    var moreThanOneGroup = getRegex.GetGroupNumbers().Length > 1;
		//    foreach (Match match in getRegex.Matches(downloadedText))
		//    {
		//        if (moreThanOneGroup)
		//        {
		//            AddValuesFromRegexGroups(resultText, getRegex, match, uriGroup);
		//        }
		//        else
		//        {
		//            resultText.Append(match.Value);
		//        }

		//        result.Add(resultText.ToString());
		//        resultText.Clear();
		//    }
		//}

		//private static void AddValuesFromRegexGroups(StringBuilder resultText, Regex getRegex, Match match, string uriGroup)
		//{
		//    var removeLastChar = true;

		//    for (var i = 0; i < match.Groups.Count; i++)
		//    {
		//        var groupName = getRegex.GroupNameFromNumber(i);
		//        int number;
		//        if (Int32.TryParse(groupName, out number)) continue;

		//        if (!String.IsNullOrEmpty(uriGroup) && groupName == uriGroup)
		//        {
		//            removeLastChar = false;
		//            resultText.Append(match.Groups[groupName].Value);
		//            break;
		//        }

		//        resultText.AppendFormat("{0}:{1};", groupName, match.Groups[i].Value);
		//    }

		//    if (removeLastChar)
		//    {
		//        resultText.Remove(resultText.Length - 1, 1);
		//    }
		//}
	}
}