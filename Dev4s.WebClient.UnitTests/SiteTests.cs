using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Dev4s.WebClient.UnitTests
{
	[TestFixture]
	public class SiteTests
	{
		#region SetUp
		private ISearchRange _sr1;
		private ISearchRange _sr2;
		private ISearchRange _sr3;
		private ISearchRange _srEmpty;

		[TestFixtureSetUp]
		public void SetUp()
		{
			_sr1 = new Global.FakeSearchRange(new[] { "a1", "a2", "a3", "a4" });
			_sr2 = new Global.FakeSearchRange(new[] { "b1", "b2", "b3", "b4" });
			_sr3 = new Global.FakeSearchRange(new[] { "c1", "c2", "c3", "c4" });
			_srEmpty = new Global.FakeSearchRange(new List<string>());
		}
		#endregion

		[Test]
		public void DoesUriIsTheSame()
		{
			//Arrange
			//Act
			var result = new Site(Global.GoogleUri);

			//Assert
			Assert.That(result.MainUri, Is.EqualTo(Global.GoogleUri));
		}

		[Test]
		public void PassingNullValuesToAllConstructors()
		{
			//Arrange
			//Act
			//Assert
			Assert.Throws<ArgumentNullException>(() => new Site(null, null));
		}

		[Test]
		public void AddingRangesShouldWorkFromConstructor()
		{
			//Arrange
			//Act
			var result1 = new Site(Global.GoogleUri, _sr1);
			var result2 = new Site(Global.GoogleUri, _sr1, _sr2);

			//Assert
			Assert.That(result1.SearchRanges, Has.Count.EqualTo(1));
			Assert.That(result2.SearchRanges, Has.Count.EqualTo(2));
			Assert.That(result1.SearchRanges[0], Is.EqualTo(_sr1));
			Assert.That(result2.SearchRanges[0], Is.EqualTo(_sr1));
			Assert.That(result2.SearchRanges[1], Is.EqualTo(_sr2));
		}
		
		[Test]
		public void ThrowsExceptionWhenNumberOfLeftAndRightBracketIsDifferent()
		{
			//Arrange
			var siteUri = Global.GoogleUri.CreateNewUriWithPath("/{0}/{1");

			//Act
			//Assert
			Assert.Throws<Exceptions.Site.CurlyBracketException>(() => new Site(siteUri));
		}

		[Test]
		public void CheckIfBracketsArePassedInProperOrder()
		{
			//Arrange
			var siteUriCase1 = Global.GoogleUri.CreateNewUriWithPath("/{1}/{0}");
			var siteUriCase2 = Global.GoogleUri.CreateNewUriWithPath("/{5}/{9}");
			var siteUriCase3 = Global.GoogleUri.CreateNewUriWithPath("/{20}");
			var siteUriCase4 = Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}");

			//Act
			//Assert
			Assert.Throws<Exceptions.Site.UriParameterOrderException>(() => new Site(siteUriCase1));
			Assert.Throws<Exceptions.Site.UriParameterOrderException>(() => new Site(siteUriCase2));
			Assert.Throws<Exceptions.Site.UriParameterOrderException>(() => new Site(siteUriCase3));
			Assert.DoesNotThrow(() => new Site(siteUriCase4));
		}

		[Test]
		public void CheckIfNumberOfUriParametersAndSearchRangesAreTheSame()
		{
			//Arrange
			var siteUrl = Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}");

			//Act
			var result1 = new Site(siteUrl, _sr1, _sr2);
			var result2 = new Site(siteUrl, _sr1);

			//Assert
			Assert.That(result1.SearchRanges.Count == result1.UriParametersCount, Is.True);
			Assert.That(result2.SearchRanges.Count == result2.UriParametersCount, Is.False);
		}

		[Test]
		public void AddingBreakOnAndGetRegexFromConstructor()
		{
			//Arrange
			var regexGet = new Regex("\\d+");
			var regexBreakOn = new Regex(".*");
			var srl = new List<ISearchRange> { _sr1, _sr1, _sr1, _sr1}.ToArray();

			//Act
			var result = new Site(Global.GoogleUri, srl) {BreakOnRegex = regexBreakOn, GetRegex = regexGet};

			//Assert
			Assert.That(result.BreakOnRegex, Is.Not.Null);
			Assert.That(result.BreakOnRegex.ToString(), Is.EqualTo(".*"));
			Assert.That(result.GetRegex, Is.Not.Null);
			Assert.That(result.GetRegex.ToString(), Is.EqualTo("\\d+"));
			Assert.That(result.SearchRanges, Is.Not.Empty.And.Count.EqualTo(4));
			Assert.That(result.SearchRanges, Has.All.EqualTo(_sr1));
		}
		
		[Test]
		public void CreatingListOfUrisFromConstructor()
		{
			//Arrange
			//Act
			var result1 = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}"), _sr1);
			var result2 = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);
			var result3 = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}/{2}"), _sr1, _sr2, _sr3);

			//Assert
			Assert.That(result1.Uris, Is.Not.Empty.And.Count.EqualTo(_sr1.Items.Count));
			Assert.That(result1.Uris[0], Is.EqualTo(Global.GoogleUri.CreateNewUriWithPath("/a1")));

			Assert.That(result2.Uris, Is.Not.Empty.And.Count.EqualTo(_sr1.Items.Count * _sr2.Items.Count));
			Assert.That(result2.Uris[0], Is.EqualTo(Global.GoogleUri.CreateNewUriWithPath("/a1/b1")));

			Assert.That(result3.Uris, Is.Not.Empty.And.Count.EqualTo(_sr1.Items.Count * _sr2.Items.Count * _sr3.Items.Count));
			Assert.That(result3.Uris[0], Is.EqualTo(Global.GoogleUri.CreateNewUriWithPath("/a1/b1/c1")));
		}

		[Test]
		public void CreatingListOfUrisWhenSearchRangesAreEmpty()
		{
			//Arrange
			//Act
			//Assert
			Assert.DoesNotThrow(() => new Site(Global.GoogleUri));
			Assert.That((new Site(Global.GoogleUri)).Uris, Is.Empty);
			Assert.That((new Site(Global.GoogleUri)).SearchRanges, Is.Empty);
		}

		[Test]
		public void ShouldNotCreateUrisWhenThereAreNotEnoughParametersInConstructor()
		{
			//Arrange
			//Act
			var site = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1);

			//Assert
			Assert.That(site.Uris, Is.Empty);
		}

		[Test]
		public void AddingNullToSearchRangesInsteadOfListShouldThrowAnException()
		{
			//Arrange
			var site = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);

			//Act
			//Assert
			Assert.Throws<ArgumentNullException>(() => site.SearchRanges = null);
		}

		[Test]
		public void MovingAndChangingSearchRangeItemsShouldRebuildUris()
		{
			//Arrange
			var eventfired1 = false;
			var eventfired2 = false;
			var site1 = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);
			var site2 = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);

			site1.SearchRanges.CollectionChanged += (sender, args) => eventfired1 = true;
			site2.SearchRanges.CollectionChanged += (sender, args) => eventfired2 = true;

			//Act
			var uriBeforeMove1 = new Uri(site1.Uris[0].ToString());
			site1.SearchRanges.Move(1, 0);
			var uriBeforeMove2 = new Uri(site2.Uris[0].ToString());
			site2.SearchRanges[1] = _sr3;
			
			//Assert
			Assert.That(new[] {eventfired1, eventfired2}, Is.All.True);

			Assert.That(site1.Uris[0], Is.Not.EqualTo(uriBeforeMove1));
			Assert.That(site2.Uris[0], Is.Not.EqualTo(uriBeforeMove2));

			Assert.That(site1.Uris[0], Is.EqualTo(Global.GoogleUri.CreateNewUriWithPath("/b1/a1")));
			Assert.That(site2.Uris[0], Is.EqualTo(Global.GoogleUri.CreateNewUriWithPath("/a1/c1")));
		}

		[Test]
		public void RemovingOrAddingSearchRangeItemShouldClearUrisWhenThereIsNotEnoughOrTooMuchParametersInMainUri()
		{
			//Arrange
			bool eventfired1, eventfired2, eventfired3;
			eventfired1 = eventfired2 = eventfired3 = false;
			var site1 = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);
			var site2 = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);
			var site3 = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);

			site1.SearchRanges.CollectionChanged += (sender, args) => eventfired1 = true;
			site2.SearchRanges.CollectionChanged += (sender, args) => eventfired2 = true;
			site3.SearchRanges.CollectionChanged += (sender, args) => eventfired3 = true;

			//Act
			site1.SearchRanges.Remove(_sr1);
			site2.SearchRanges.RemoveAt(0);
			site3.SearchRanges.Add(_sr3);

			//Assert
			Assert.That(new[] {eventfired1, eventfired2, eventfired3}, Is.All.True);

			Assert.That(site1.Uris, Is.Empty);
			Assert.That(site2.Uris, Is.Empty);
			Assert.That(site3.Uris, Is.Empty);
		}

		[Test]
		public void AddingAndRemovingSearchRangeItemsShouldRebuildUrisWhenThereIsEnoughParametersInMainUri()
		{
			//Arrange
			bool eventfired1, eventfired2, eventfired3, eventfired4;
			eventfired1 = eventfired2 = eventfired3 = eventfired4 = false;
			var site1 = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);
			var site2 = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);
			var site3 = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);
			var site4 = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);

			site1.SearchRanges.CollectionChanged += (sender, args) => eventfired1 = true;
			site2.SearchRanges.CollectionChanged += (sender, args) => eventfired2 = true;
			site3.SearchRanges.CollectionChanged += (sender, args) => eventfired3 = true;
			site4.SearchRanges.CollectionChanged += (sender, args) => eventfired4 = true;

			//Act
			var uriBeforeAddRemove = new Uri(site1.Uris[0].ToString());
			site1.SearchRanges.Add(_sr3);
			site1.SearchRanges.RemoveAt(1);

			var uriBeforeRemoveAdd = new Uri(site2.Uris[0].ToString());
			site2.SearchRanges.RemoveAt(1);
			site2.SearchRanges.Add(_sr3);

			var uriBeforeRemoveAddRange = new Uri(site3.Uris[0].ToString());
			site3.SearchRanges.RemoveAt(1);
			site3.SearchRanges.RemoveAt(0);
			site3.SearchRanges.Add(_sr1);
			site3.SearchRanges.Add(_sr3);

			var uriBeforeAddRangeRemove = new Uri(site4.Uris[0].ToString());
			site4.SearchRanges.Add(_sr1);
			site4.SearchRanges.Add(_sr3);
			site4.SearchRanges.RemoveAt(1);
			site4.SearchRanges.RemoveAt(0);

			//Assert
			Assert.That(new[] { eventfired1, eventfired2, eventfired3, eventfired4 }, Is.All.True);

			Assert.That(site1.Uris[0], Is.Not.EqualTo(uriBeforeAddRemove));
			Assert.That(site2.Uris[0], Is.Not.EqualTo(uriBeforeRemoveAdd));
			Assert.That(site3.Uris[0], Is.Not.EqualTo(uriBeforeRemoveAddRange));
			Assert.That(site4.Uris[0], Is.Not.EqualTo(uriBeforeAddRangeRemove));

			Assert.That(new[] {site1.Uris[0], site2.Uris[0], site3.Uris[0], site4.Uris[0]}, Is.All.EqualTo(Global.GoogleUri.CreateNewUriWithPath("/a1/c1")));
		}

		[Test]
		public void ClearingSearchRangeItemsShouldClearUris()
		{
			//Arrange
			var eventfired = false;
			var site = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);

			site.SearchRanges.CollectionChanged += (sender, args) => eventfired = true;

			//Act
			site.SearchRanges.Clear();

			//Assert
			Assert.That(eventfired, Is.True);
			Assert.That(site.Uris, Is.Empty);
		}
		
		[Test]
		public void InsertingSearchRangeShouldClearUris()
		{
			//Arrange
			var eventfired = false;
			var site = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);

			site.SearchRanges.CollectionChanged += (sender, args) => eventfired = true;

			//Act
			site.SearchRanges.Insert(1, _sr3);

			//Assert
			Assert.That(eventfired, Is.True);
			Assert.That(site.Uris, Is.Empty);
		}

		[Test]
		public void CreatingNewObservableCollectionWithEnoughAndNotEnoughParameters()
		{
			//Arrange
			var eventfired1 = false;
			var eventfired2 = false;
			var site1 = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);
			var site2 = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);
			
			var searchRanges1 = new ObservableCollection<ISearchRange> {_sr2};
			searchRanges1.CollectionChanged += (sender, args) => eventfired1 = true;
			var searchRanges2 = new ObservableCollection<ISearchRange> {_sr1, _sr3};
			searchRanges2.CollectionChanged += (sender, args) => eventfired2 = true;

			//Act
			site1.SearchRanges = searchRanges1;
			site2.SearchRanges = searchRanges2;

			//Assert
			Assert.That(new[] { eventfired1, eventfired2 }, Is.All.True);

			Assert.That(site1.Uris, Is.Empty);
			Assert.That(site2.Uris[0], Is.EqualTo(Global.GoogleUri.CreateNewUriWithPath("/a1/c1")));
		}

		[Test]
		public void ThrowsAnExceptionWhenAddingASearchRangeWithEmptyItems()
		{
			//Arrange
			var searchRanges = new ObservableCollection<ISearchRange> { _sr1, _srEmpty };

			//Act
			//Assert
			Assert.Throws<Exceptions.Site.SearchRangeEmptyListException>(() => new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}"), _srEmpty));
			Assert.Throws<Exceptions.Site.SearchRangeEmptyListException>(() => new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}")).SearchRanges = searchRanges);
		}
		
		[Test]
		public void MainUriShouldCheckDefaultPreconditionsTheSameFromConstructor()
		{
			//Arrange
			var site = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);

			var siteUrlCase1 = Global.GoogleUri.CreateNewUriWithPath("/{1}/{0}");
			var siteUrlCase2 = Global.GoogleUri.CreateNewUriWithPath("/{5}/{9}");
			var siteUrlCase3 = Global.GoogleUri.CreateNewUriWithPath("/{20}");
			var siteUrlCase4 = Global.GoogleUri.CreateNewUriWithPath("/{0}/{1");

			var properOne = Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}");

			//Act
			//Assert
			Assert.Throws<Exceptions.Site.UriParameterOrderException>(() => site.MainUri = siteUrlCase1);
			Assert.Throws<Exceptions.Site.UriParameterOrderException>(() => site.MainUri = siteUrlCase2);
			Assert.Throws<Exceptions.Site.UriParameterOrderException>(() => site.MainUri = siteUrlCase3);
			Assert.Throws<Exceptions.Site.CurlyBracketException>(() => site.MainUri = siteUrlCase4);
			Assert.DoesNotThrow(() => site.MainUri = properOne);
		}

		[Test]
		public void MainUriShouldThrowExceptionWhenAddingNullValue()
		{
			//Arrange
			var site = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);

			//Act
			//Assert
			Assert.Throws<ArgumentNullException>(() => site.MainUri = null);
		}

		[Test]
		public void WhenMainUriChangesItsParametersNumberUriParametersShouldChangeToo()
		{
			//Arrange
			var site = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);

			//Act
			site.MainUri = Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}/{2}");

			//Assert
			Assert.That(site.UriParametersCount, Is.EqualTo(3));
		}

		[Test]
		public void RebuilUrisWhenMainUriChangesAndThereAreApropiateNumberOfSearchRanges()
		{
			//Arrange
			var site = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"), _sr1, _sr2);

			//Act
			site.MainUri = Global.GoogleUri.CreateNewUriWithPath("?q={0}&p={1}");

			//Assert
			Assert.That(site.Uris, Is.Not.Empty);
			Assert.That(site.Uris[0], Is.EqualTo(Global.GoogleUri.CreateNewUriWithPath("?q=a1&p=b1")));
		}

		[Test]
		public void ClearUrisWhenMainUriChangesAndThereAreNotEnoughNumberOfSearchRangesOrSearchRangesAreEmpty()
		{
			//Arrange
			var site1 = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"));
			var site2 = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}"));

			//Act
			site1.SearchRanges.Add(_sr1);
			site1.MainUri = Global.GoogleUri.CreateNewUriWithPath("?q={0}&p={1}");

			site2.SearchRanges.Add(_sr1);
			site2.SearchRanges.Add(_sr2);
			site2.MainUri = Global.GoogleUri.CreateNewUriWithPath("/{0}/{1}/{2}");

			//Assert
			Assert.That(site1.Uris, Is.Empty);
			Assert.That(site2.Uris, Is.Empty);
		}

		[Test]
		public void ChangingBreakOnAndGetRegexFromObject()
		{
			//Arrange
			var site = new Site(Global.GoogleUri.CreateNewUriWithPath("/{0}"), _sr1);
			var breakOnRegex = new Regex(".*");
			var getRegex = new Regex(".");

			//Act
			site.BreakOnRegex = breakOnRegex;
			site.GetRegex = getRegex;

			//Assert
			Assert.That(site.BreakOnRegex, Is.EqualTo(breakOnRegex));
			Assert.That(site.GetRegex, Is.EqualTo(getRegex));
		}
	}
}