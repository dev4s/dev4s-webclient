using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Dev4s.WebClient.UnitTests
{
	[TestFixture]
	public class ExtensionsTests
	{
		[Test]
		public void AsSiteShouldReturnISiteWhenThereAreProperUris()
		{
			//Arrange
			var texts = new List<string> { "http://google.pl", "http://allegro.pl" };

			//Act
			var site = texts.AsSite();

			//Assert
			Assert.That(site.UriParametersCount, Is.EqualTo(-1));
			Assert.That(site.MainUri, Is.Null);
			Assert.That(new[] {site.BreakOnRegex, site.GetRegex}, Has.All.Null);
			Assert.That(site.Uris, Has.Count.EqualTo(2));
			Assert.That(site.Uris[0], Is.EqualTo(new Uri(texts[0])));
			Assert.That(site.Uris[1], Is.EqualTo(new Uri(texts[1])));
		}

		[Test]
		public void AsSiteShouldThrowExceptionWhenThereAreUnproperUris()
		{
			//Arrange
			var texts1 = new List<string> { "", "testtest" };
			var texts2 = new List<string> { "testtest", "" };
			var texts3 = new List<string> { "testtest", "", "http://google.pl" };

			//Act
			//Assert
			Assert.Throws<Exceptions.Extension.UriConversionException>(() => texts1.AsSite());
			Assert.Throws<Exceptions.Extension.UriConversionException>(() => texts2.AsSite());
			Assert.Throws<Exceptions.Extension.UriConversionException>(() => texts3.AsSite());
		}
	}
}