using System;
using System.Collections.Generic;

namespace Dev4s.WebClient
{
	public static class Extensions
	{
		/// <summary>
		/// Adds range items.
		/// </summary>
		internal static void AddRange<T>(this IList<T> itemsList, IEnumerable<T> newItems)
		{
			foreach (var newItem in newItems)
			{
				itemsList.Add(newItem);
			}
		}

		/// <summary>
		/// Creates ISite with Uris. Uris should be absolute (with scheme - http, ftp, etc).
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		public static ISite AsSite(this ICollection<string> collection)
		{
			var tempList = new List<Uri>(collection.Count);

			foreach (var item in collection)
			{
				if (string.IsNullOrEmpty(item))
				{
					throw new Exceptions.Extension.UriConversionException();
				}
				
				if (!Uri.IsWellFormedUriString(item, UriKind.Absolute))
				{
					throw new Exceptions.Extension.UriConversionException();
				}

				tempList.Add(new Uri(item));
			}

			return new AsSiteResult(tempList);
		}

		/// <summary>
		/// Safely invokes event.
		/// </summary>
		public static void SafeInvoke(this WebClientEventHandler evt, object sender, WebClientEventArgs e)
		{
			if (evt != null)
			{
				evt(sender, e);
			}
		}
	}
}