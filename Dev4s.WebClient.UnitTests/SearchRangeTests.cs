using System;
using Dev4s.WebClient.Enums;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

namespace Dev4s.WebClient.UnitTests
{
	[TestFixture]
	public class SearchRangeTests
	{
		[Test]
		public void PassingNotAllowedNumbersToConstructor()
		{
			//Arrange
			//Act
			//Assert
			Assert.Throws<Exceptions.SearchRange.NumbersException>(() => new SearchRange(30, 10));		//small to
			Assert.Throws<Exceptions.SearchRange.NumbersException>(() => new SearchRange(10, 10));		//same from and to
			Assert.Throws<Exceptions.SearchRange.NumbersException>(() => new SearchRange(10, 20, 20));	//step is too big
			Assert.Throws<Exceptions.SearchRange.NumbersException>(() => new SearchRange(20, 10, 2));	//small to
			Assert.Throws<Exceptions.SearchRange.NumbersException>(() => new SearchRange(10, 10, 2));	//same from and to
			Assert.Throws<Exceptions.SearchRange.NumbersException>(() => new SearchRange(10, 20, 0));	//step is 0
		}

		[Test]
		public void PassingAllowedNumbersToConstructor()
		{
			//Arrange
			//Act
			var searchRange1 = new SearchRange(2000);
			var searchRange2 = new SearchRange(20, 5000);
			var searchRange3 = new SearchRange(10, 20, 2);

			//Assert
			Assert.That(searchRange1.Items[0], Is.EqualTo("0"));
			Assert.That(searchRange1.Items[100], Is.EqualTo("100"));
			Assert.That(searchRange1.Items[2000], Is.EqualTo("2000"));

			Assert.That(searchRange2.Items[0], Is.EqualTo("20"));
			Assert.That(searchRange2.Items[100], Is.EqualTo("120"));
			Assert.That(searchRange2.Items[4980], Is.EqualTo("5000"));

			Assert.That(searchRange3.Items[0], Is.EqualTo("10"));
			Assert.That(searchRange3.Items[2], Is.EqualTo("14"));
			Assert.That(searchRange3.Items[5], Is.EqualTo("20"));
		}

		[Test]
		public void PassingSpecialAllowedNumberTypeToConstructor()
		{
			//Arrange
			//Act
			var searchRange1 = new SearchRange(1, 2000, 1, NumberOption.WithZeros);
			var searchRange2 = new SearchRange(2000, NumberOption.WithZeros);

			//Assert
			Assert.That(searchRange1.Items[0], Is.EqualTo("0001"));
			Assert.That(searchRange1.Items[99], Is.EqualTo("0100"));
			Assert.That(searchRange1.Items[1999], Is.EqualTo("2000"));

			Assert.That(searchRange2.Items[0], Is.EqualTo("0000"));
			Assert.That(searchRange2.Items[100], Is.EqualTo("0100"));
			Assert.That(searchRange2.Items[2000], Is.EqualTo("2000"));
		}

		[Test]
		public void PassingAsciiCharsEnumToConstructor()
		{
			//Arrange
			//Act
			var searchRange = new SearchRange(SearchRangeOption.AsciiChars);

			//Assert
			foreach (var ch in searchRange.Items.SelectMany(x => x))
			{
				Assert.That(char.IsControl(ch), Is.False);
				Assert.That(char.IsWhiteSpace(ch), Is.False);
			}
			Assert.That(searchRange.Items, Has.Count.EqualTo(94));
		}

		[Test]
		public void PassingLettersEnumToConstructor()
		{
			//Arrange
			//Act
			var searchRange = new SearchRange(SearchRangeOption.EnLetters);

			//Assert
			Assert.That(searchRange.Items, Is.EquivalentTo(Global.Letters).And.Count.EqualTo(Global.Letters.Count));
		}

		[Test]
		public void PassingPolishLettersEnumToConstructor()
		{
			//Arrange
			var tempLetters = Global.Letters;
			tempLetters.AddRange(Global.PolishLetters);

			//Act
			var searchRange = new SearchRange(SearchRangeOption.EnPlusPlLetters);

			//Assert
			Assert.That(searchRange.Items, Is.EquivalentTo(tempLetters).And.Count.EqualTo(tempLetters.Count));
		}

		[Test]
		public void PassingAsciiCharPolishLettersToConstructor()
		{
			//Arrange
			var tempLetters = new List<string>(new SearchRange(SearchRangeOption.AsciiChars).Items);
			tempLetters.AddRange(Global.PolishLetters);
			tempLetters.AddRange(Global.PolishLetters.ConvertAll(x => x.ToUpper()));

			//Act
			var searchRange = new SearchRange(SearchRangeOption.AsciiPlusPlChars);

			//Assert
			Assert.That(searchRange.Items, Is.EquivalentTo(tempLetters).And.Count.EqualTo(tempLetters.Count));
		}

		[Test]
		public void PassingManyTextsToConstructor()
		{
			//Arrange
			var texts = new[] { "text01", "text02", "text03", "text04", "text05" };

			//Act
			var searchRange1 = new SearchRange(texts);
			var searchRange2 = new SearchRange(texts[0], texts[1]);

			//Assert
			Assert.That(searchRange1.Items, Is.Not.Empty.And.Count.EqualTo(5));
			Assert.That(searchRange1.Items[0], Is.EqualTo(texts[0]));
			Assert.That(searchRange1.Items[1], Is.EqualTo(texts[1]));
			Assert.That(searchRange1.Items[2], Is.EqualTo(texts[2]));
			Assert.That(searchRange1.Items[3], Is.EqualTo(texts[3]));
			Assert.That(searchRange1.Items[4], Is.EqualTo(texts[4]));

			Assert.That(searchRange2.Items, Is.Not.Empty.And.Count.EqualTo(2));
			Assert.That(searchRange2.Items[0], Is.EqualTo(texts[0]));
			Assert.That(searchRange2.Items[1], Is.EqualTo(texts[1]));
		}

		[Test]
		public void PassingEmptyListToConstructor()
		{
			//Arrange
			var texts = new List<string>().ToArray();

			//Act
			//Assert
			Assert.Throws<Exceptions.SearchRange.EmptyStringListException>(() => new SearchRange(texts));
		}

		[Test]
		public void PassingDatesToConstructor()
		{
			//Arrange
			var date01 = new DateTime(2000, 1, 1);
			var date02 = new DateTime(2000, 2, 1);

			var date03 = new DateTime(2000, 1, 20);
			var date04 = new DateTime(2001, 12, 1);

			var date05 = new DateTime(2000, 5, 14);
			var date06 = new DateTime(2012, 2, 12);

			//Act
			var searchRange1 = new SearchRange(date01, date02);
			var searchRange2 = new SearchRange(date01, date02, 2);

			var searchRange3 = new SearchRange(date03, date04, 2, DateOption.Months);
			var searchRange4 = new SearchRange(date05, date06, 2, DateOption.Years);

			//Assert
			Assert.That(searchRange1.Items, Is.Not.Empty.And.Count.EqualTo(32));
			Assert.That(searchRange1.Items[0], Is.EqualTo("2000-01-01"));
			Assert.That(searchRange1.Items[1], Is.EqualTo("2000-01-02"));
			Assert.That(searchRange1.Items[31], Is.EqualTo("2000-02-01"));

			Assert.That(searchRange2.Items, Is.Not.Empty.And.Count.EqualTo(16));
			Assert.That(searchRange2.Items[0], Is.EqualTo("2000-01-01"));
			Assert.That(searchRange2.Items[1], Is.EqualTo("2000-01-03"));
			Assert.That(searchRange2.Items[15], Is.EqualTo("2000-01-31"));

			Assert.That(searchRange3.Items, Is.Not.Empty.And.Count.EqualTo(12));
			Assert.That(searchRange3.Items[0], Is.EqualTo("2000-01-01"));
			Assert.That(searchRange3.Items[1], Is.EqualTo("2000-03-01"));
			Assert.That(searchRange3.Items[11], Is.EqualTo("2001-11-01"));

			Assert.That(searchRange4.Items, Is.Not.Empty.And.Count.EqualTo(7));
			Assert.That(searchRange4.Items[0], Is.EqualTo("2000-01-01"));
			Assert.That(searchRange4.Items[1], Is.EqualTo("2002-01-01"));
			Assert.That(searchRange4.Items[6], Is.EqualTo("2012-01-01"));
		}

		[Test]
		public void PassingDatesWithDateFormatToConstructor()
		{
			//Arrange
			var date01 = new DateTime(2000, 1, 1);
			var date02 = new DateTime(2000, 2, 1);
			const string otherDateFormat = "dd-MM-yyyy";

			//Act
			var searchRange = new SearchRange(date01, date02, 1, DateOption.Days, otherDateFormat);

			//Assert
			Assert.That(searchRange.Items, Is.Not.Empty.And.Count.EqualTo(32));
			Assert.That(searchRange.Items[0], Is.EqualTo("01-01-2000"));
			Assert.That(searchRange.Items[1], Is.EqualTo("02-01-2000"));
			Assert.That(searchRange.Items[31], Is.EqualTo("01-02-2000"));
		}

		[Test]
		public void PassingDatesToOtherConstructors()
		{
			//Arrange
			var date01 = new DateTime(2000, 1, 1);
			var date02 = new DateTime(2000, 3, 1);
			const string otherDateFormat = "dd-MM-yyyy";

			//Act
			var searchRange1 = new SearchRange(date01, date02, DateOption.Months);
			var searchRange2 = new SearchRange(date01, date02, otherDateFormat);
			var searchRange3 = new SearchRange(date01, date02, DateOption.Months, otherDateFormat);

			//Assert
			Assert.That(searchRange1.Items, Is.Not.Empty.And.Count.EqualTo(3));
			Assert.That(searchRange1.Items[0], Is.EqualTo("2000-01-01"));
			Assert.That(searchRange1.Items[2], Is.EqualTo("2000-03-01"));

			Assert.That(searchRange2.Items, Is.Not.Empty.And.Count.EqualTo(61));
			Assert.That(searchRange2.Items[0], Is.EqualTo("01-01-2000"));
			Assert.That(searchRange2.Items[1], Is.EqualTo("02-01-2000"));
			Assert.That(searchRange2.Items[60], Is.EqualTo("01-03-2000"));

			Assert.That(searchRange3.Items, Is.Not.Empty.And.Count.EqualTo(3));
			Assert.That(searchRange3.Items[0], Is.EqualTo("01-01-2000"));
			Assert.That(searchRange3.Items[2], Is.EqualTo("01-03-2000"));
		}

		[Test]
		public void PassingNotAllowedDatesToConstructor()
		{
			//Arrange
			//Act
			//Assert
			Assert.Throws<Exceptions.SearchRange.DatesException>(() => new SearchRange(new DateTime(2000, 1, 1), new DateTime(1999, 1, 1)));
			Assert.Throws<Exceptions.SearchRange.DatesException>(() => new SearchRange(new DateTime(1999, 1, 1), new DateTime(1999, 1, 1)));
			Assert.Throws<Exceptions.SearchRange.DatesException>(() => new SearchRange(new DateTime(1999, 1, 1), new DateTime(1999, 1, 2), 10));
			Assert.Throws<Exceptions.SearchRange.DatesException>(() => new SearchRange(new DateTime(1999, 1, 1), new DateTime(1999, 1, 2), 0));

			Assert.Throws<Exceptions.SearchRange.DatesException>(() => new SearchRange(new DateTime(1999, 1, 1), new DateTime(1999, 1, 23), DateOption.Months));
			Assert.Throws<Exceptions.SearchRange.DatesException>(() => new SearchRange(new DateTime(1999, 1, 1), new DateTime(1999, 12, 1), DateOption.Years));
		}

		[Test]
		public void ItemsListModifications()
		{
			//Arrange
			var itemsList = new List<string> {"test01", "test02"};

			//Act
			var searchRange = new SearchRange("test03") {Items = itemsList};
			searchRange.Items.Add("test03");

			//Assert
			Assert.That(searchRange.Items, Is.Not.Empty.And.Count.EqualTo(3));
			Assert.That(searchRange.Items[0], Is.EqualTo(itemsList[0]));
			Assert.That(searchRange.Items[1], Is.EqualTo(itemsList[1]));
			Assert.That(searchRange.Items[2], Is.EqualTo("test03"));
		}

		[Test]
		public void PassingNullValuesToConstructors()
		{
			//Arrange
			//Act
			//Assert
			Assert.Throws<ArgumentNullException>(() => new SearchRange(null));
			//Assert.Throws<ArgumentNullException>(() => new SearchRange(new DateTime(2000, 1, 1), new DateTime(2000, 1, 2), null)); - this is not needed ;)
		}
	}
}