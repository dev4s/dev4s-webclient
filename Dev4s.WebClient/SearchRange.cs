using System;
using System.Collections.Generic;
using System.Globalization;
using Dev4s.WebClient.Enums;

namespace Dev4s.WebClient
{
	public sealed class SearchRange : ISearchRange
	{
		#region Properties
		private readonly string[] _polishLetters = new[] { "ą", "ć", "ę", "ł", "ń", "ó", "ś", "ż", "ź" };

		public IList<string> Items { get; set; }
		#endregion

		#region Constructors
		private SearchRange()
		{
			Items = new List<string>();
		}

		#region Dates
		public SearchRange(DateTime @from, DateTime to, DateOption dateOption, string dateFormat) 
			: this(@from, to, 1, dateOption, dateFormat) {}

		public SearchRange(DateTime @from, DateTime to, DateOption dateOption) 
			: this(@from, to, 1, dateOption) { }

		public SearchRange(DateTime @from, DateTime to, string dateFormat) 
			: this(@from, to, 1, DateOption.Days, dateFormat) { }

		public SearchRange(DateTime @from, DateTime to, int step = 1, DateOption dateOption = DateOption.Days, string dateFormat = null)
			: this()
		{
			var daysBetweenDates = (to - @from).Days;
			var yearsBetweenDates = to.Year - @from.Year;
			var monthsBetweenDates = yearsBetweenDates * 12 + (to.Month - @from.Month);
			var tempDateFormat = string.IsNullOrEmpty(dateFormat) ? "yyyy-MM-dd" : dateFormat;

			Preconditions.CheckSpecialPreconditions(step, dateOption, yearsBetweenDates, monthsBetweenDates, daysBetweenDates);

			switch (dateOption)
			{
				case DateOption.Days:
					AddDatesToItems(@from, daysBetweenDates, step, DateOption.Days, tempDateFormat);
					break;

				case DateOption.Months:
					AddDatesToItems(@from, monthsBetweenDates, step, DateOption.Months, tempDateFormat);
					break;

				case DateOption.Years:
					AddDatesToItems(@from, yearsBetweenDates, step, DateOption.Years, tempDateFormat);
					break;
			}
		}
		#endregion

		#region Texts
		public SearchRange(params string[] texts)
			: this()
		{
			Items.AddRange(texts.CheckForNull().CheckLength());
		}
		#endregion

		#region Numbers
		public SearchRange(uint to, NumberOption numberOption = NumberOption.Normal)
			: this(0, to, 1, numberOption) { }

		public SearchRange(uint @from, uint to, uint step = 1, NumberOption numberOption = NumberOption.Normal)
			: this()
		{
			Preconditions.CheckSpecialPreconditions(@from, to, step);

			var maxLength = to.ToString(CultureInfo.InvariantCulture).Length;
			for (var i = @from; i <= to; i += step)
			{
				var tempNumber = i.ToString(CultureInfo.InvariantCulture);
				switch (numberOption)
				{
					case NumberOption.Normal:
						Items.Add(tempNumber);
						break;

					case NumberOption.WithZeros:
						Items.Add(CreateZeros(tempNumber.Length, maxLength) + tempNumber);
						break;
				}
			}
		}
		#endregion

		#region Types
		public SearchRange(SearchRangeOption type)
			: this()
		{
			switch (type)
			{
				case SearchRangeOption.AsciiChars:
					AddCharsToItems(0, 127);
					break;

				case SearchRangeOption.EnLetters:
					AddCharsToItems(97, 122);
					break;

				case SearchRangeOption.EnPlusPlLetters:
					AddCharsToItems(97, 122);
					Items.AddRange(_polishLetters);
					break;

				case SearchRangeOption.AsciiPlusPlChars:
					AddCharsToItems(0, 127);
					Items.AddRange(_polishLetters);
					Items.AddRange(Array.ConvertAll(_polishLetters, x => x.ToUpper()));
					break;
			}
		}
		#endregion
		#endregion

		#region Private methods
		private string CreateZeros(int numberLength, int maxLength)
		{
			var zeros = string.Empty;
			for (var i = 0; i < maxLength; i++)
			{
				if (i == maxLength - numberLength)
				{
					break;
				}
				zeros += 0;
			}
			return zeros;
		}

		private void AddCharsToItems(int @from, int to)
		{
			for (var i = @from; i <= to; i++)
			{
				var tempChar = (char) i;
				if (!char.IsControl(tempChar) && !char.IsWhiteSpace(tempChar))
				{
					Items.Add(tempChar.ToString(CultureInfo.InvariantCulture));
				}
			}
		}

		private void AddDatesToItems(DateTime date, int maxCount, int step, DateOption dateOption, string dateFormat)
		{
			var monthDate = new DateTime(date.Year, date.Month, 1);
			var yearDate = new DateTime(date.Year, 1, 1);

			for (var i = 0; i <= maxCount; i += step)
			{
				switch (dateOption)
				{
					case DateOption.Days:
						Items.Add(date.Date.AddDays(i).ToString(dateFormat));
						break;

					case DateOption.Months:
						Items.Add(monthDate.Date.AddMonths(i).ToString(dateFormat));
						break;

					case DateOption.Years:
						Items.Add(yearDate.Date.AddYears(i).ToString(dateFormat));
						break;
				}
			}
		}
		#endregion
	}
}