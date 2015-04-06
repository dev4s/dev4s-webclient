using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Dev4s.WebClient.Enums;

namespace Dev4s.WebClient
{
	public static class Preconditions
	{
		public static Encoding CheckForNull(this Encoding value)
		{
			return CheckForNull(value, "Encoding can not be null");
		}

		public static Uri CheckForNull(this Uri value)
		{
			return CheckForNull(value, "Uri can not be null");
		}

		public static IEnumerable<ISite> CheckForNull(this IEnumerable<ISite> sites)
		{
			return CheckForNull(sites, "Sites can not be null.");
		} 

		public static IEnumerable<ISearchRange> CheckForNull(this IEnumerable<ISearchRange> searchRanges)
		{
			return CheckForNull(searchRanges, "SearchRanges can not be null.");
		}

		public static IWebClientWrapper CheckForNull(this IWebClientWrapper webClientWrapper)
		{
			return CheckForNull(webClientWrapper, "WebClientWrapper can not be null.");
		}

		public static string[] CheckForNull(this string[] texts)
		{
			return CheckForNull(texts, "Passed texts can not be null.");
		}
		
		private static T CheckForNull<T>(T value, string exceptionMsg) where T : class
		{
			if (value == null)
			{
				throw new ArgumentNullException(exceptionMsg + " Please check your arguments.");
			}
			return value;
		}

		public static IEnumerable<string> CheckLength(this string[] texts)
		{
			if (texts.Length == 0)
			{
				throw new Exceptions.SearchRange.EmptyStringListException();
			}
			return texts;
		}

		public static void CheckForEmptyItems(this IEnumerable<ISearchRange> searchRanges)
		{
			var tempSearchRange = searchRanges;
			if (tempSearchRange.Any(range => range.Items.Count == 0))
			{
				throw new Exceptions.Site.SearchRangeEmptyListException();
			}
		}

		public static void CheckSpecialPreconditions(IEnumerable<ISite> sites)
		{
			var tempSite = sites.ToList();
			if (!tempSite.Any())
			{
				throw new Exceptions.WebClient.ArgumentSiteException();
			}

			if (tempSite.Any(x => x.Uris.Count == 0))
			{
				throw new Exceptions.WebClient.UrisEmptyException();
			}
		}

		public static void CheckSpecialPreconditions(int step, DateOption dateOption, int yearsBetweenDates, int monthsBetweenDates,
													 int daysBetweenDates)
		{
			var tempStep = step;
			var dateTimePrecondition = 0;

			switch (dateOption)
			{
				case DateOption.Days:
					dateTimePrecondition = daysBetweenDates;
					break;

				case DateOption.Months:
					dateTimePrecondition = monthsBetweenDates;
					break;

				case DateOption.Years:
					dateTimePrecondition = yearsBetweenDates;
					break;
			}

			if (dateTimePrecondition < tempStep || dateTimePrecondition <= 0 || step < 1)
			{
				throw new Exceptions.SearchRange.DatesException();
			}
		}

		public static void CheckSpecialPreconditions(uint @from, uint to, uint step)
		{
			var tempFrom = @from;
			var tempTo = to;
			var tempStep = step;

			if ((tempTo - tempFrom) < tempStep || tempFrom >= tempTo || tempStep < 1)
			{
				throw new Exceptions.SearchRange.NumbersException();
			}
		}

		public static Uri CheckSpecialPreconditions(this Uri uri, out int uriParametersCount)
		{
			return CheckParametersOrder(CheckBrackets(uri), out uriParametersCount);
		}

		private static Uri CheckBrackets(Uri uri)
		{
			var leftBracketCount = uri.LocalPath.Count(x => x == '{');
			var rightBracketCount = uri.LocalPath.Count(x => x == '}');

			if (leftBracketCount != rightBracketCount)
			{
				throw new Exceptions.Site.CurlyBracketException();
			}

			return uri;
		}

		private static Uri CheckParametersOrder(Uri uri, out int uriParametersCount)
		{
			var matches = new Regex(@"(?:\{)(?<Number>\d*)(?:\})").Matches(uri.LocalPath);

			for (var i = 0; i < matches.Count; i++)
			{
				if (Convert.ToInt32(matches[i].Groups["Number"].Value) != i)
				{
					throw new Exceptions.Site.UriParameterOrderException();
				}
			}

			uriParametersCount = matches.Count;
			return uri;
		}
	}
}