using System;

namespace Dev4s.WebClient.Exceptions
{
	public static class Site
	{
		public class CurlyBracketException : Exception
		{
			public override string Message
			{
				get { return "Number of right and left curly bracket is different. Please check them!"; }
			}
		}

		public class UriParameterOrderException : Exception
		{
			public override string Message
			{
				get
				{
					return "Order of 'Uri' paremeters is not proper.\r\n" +
						   "Proper one: \r\n" +
						   "\tvar site = new Site(new Uri(\"http://google.com/#q={0}\"));";
				}
			}
		}

		public class SearchRangeEmptyListException : Exception
		{
			public override string Message
			{
				get { return "One of the Search Ranges items list is empty. Please check them!"; }
			}
		}
	}
}