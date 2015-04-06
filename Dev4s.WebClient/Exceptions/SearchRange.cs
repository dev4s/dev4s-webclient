using System;

namespace Dev4s.WebClient.Exceptions
{
	public static class SearchRange
	{
		public class EmptyStringListException : ArgumentException
		{
			public override string Message
			{
				get { return "SearchRange does not allow empty string list as parameter."; }
			}
		}

		public class NumbersException : ArgumentException
		{
			public override string Message
			{
				get { return "Check your arguments. You can not have a situation when 'from' is bigger than 'to'," +
				             "'step' is lower than 1, and also the 'step' is bigger than difference between 'to' and 'from'."; }
			}
		}

		public class DatesException : NumbersException { }
	}
}