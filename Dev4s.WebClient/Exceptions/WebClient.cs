using System;

namespace Dev4s.WebClient.Exceptions
{
	public static class WebClient
	{
		public class ArgumentSiteException : ArgumentNullException
		{
			public override string Message
			{
				get { return "WebClient should have at least one site to allow download."; }
			}
		}

		public class UrisEmptyException : Exception
		{
			public override string Message
			{
				get
				{
					return "The Uris list is empty. Check your Site parameter.";
				}
			}
		}
	}
}