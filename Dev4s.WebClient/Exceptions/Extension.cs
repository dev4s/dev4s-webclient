using System;

namespace Dev4s.WebClient.Exceptions
{
	public static class Extension
	{
		public class UriConversionException : Exception
		{
			public override string Message
			{
				get
				{
					return "Some strings are not proper for Uri (ex.: empty ones, they have lack of scheme " +
					       "[http, ftp, and so on], etc.). Please check them!";
				}
			}
		}
	}
}