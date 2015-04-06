using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Dev4s.WebClient
{
	public sealed class Site : ISite
	{
		#region Properties
		public Regex GetRegex { get; set; }
		public Regex BreakOnRegex { get; set; }
		public ObservableCollection<ISearchRange> SearchRanges
		{
			get { return _searchRanges; }
			set
			{
				value.CheckForNull().CheckForEmptyItems();

				_searchRanges = value;
				_searchRanges.CollectionChanged += SearchRangesCollectionChanged;

				ExecuteEvent();
			}
		}	
		public Uri MainUri
		{
			get { return _mainUri; }
			set
			{
				_mainUri = value.CheckForNull().CheckSpecialPreconditions(out _uriParametersCount);

				if (_searchRanges.Count != 0 && _searchRanges.Count == _uriParametersCount)
				{
					CreateUris();
				}
				if (_searchRanges.Count != _uriParametersCount)
				{
					_uris = new List<Uri>();
				}
			}
		}
		public ReadOnlyCollection<Uri> Uris
		{
			get { return new ReadOnlyCollection<Uri>(_uris); }
		}
		public int UriParametersCount
		{
			get { return _uriParametersCount; }
		}
		#endregion

		#region Private properties
		private ObservableCollection<ISearchRange> _searchRanges;
		private Uri _mainUri;
		private IList<Uri> _uris;
		private int _uriParametersCount;
		private readonly bool _fireOnConstructor = true;
		#endregion

		#region Constructors
		private Site()
		{
			_searchRanges = new ObservableCollection<ISearchRange>();
			_searchRanges.CollectionChanged += SearchRangesCollectionChanged;

			_mainUri = new Uri("http://www.dev4s.com");
			_uris = new List<Uri>();
		}

		public Site(Uri uri, params ISearchRange[] searchRanges)
			: this()
		{
			searchRanges.CheckForNull().CheckForEmptyItems();

			_mainUri = uri.CheckForNull().CheckSpecialPreconditions(out _uriParametersCount);

			_searchRanges.AddRange(searchRanges);
			
			if (searchRanges.Length != 0 && searchRanges.Length == _uriParametersCount)
			{
				CreateUris();
			}

			_fireOnConstructor = false;
		}
		#endregion

		#region Private methods
		private void CreateUris()
		{
			_uris = new List<Uri>(CreateStringUris().Select(x => new Uri(x)));
		}

		private IEnumerable<string> CreateStringUris(string item = "", int startAt = 0)
		{
			var tempList = startAt == 0
						   ? new List<string>(GetCapacityFromAllSearchRanges())
						   : new List<string>(_searchRanges[startAt].Items.Count);

			var tempUri = string.IsNullOrEmpty(item)
						  ? _mainUri.ToString()
						  : item;

			if (startAt != _searchRanges.Count - 1)
			{
				CreateTempListOfStringUris(tempUri, tempList, startAt, startAt + 1);
			}
			else
			{
				CreateTempListOfStringUris(tempUri, tempList, startAt);
			}

			return tempList;
		}

		private int GetCapacityFromAllSearchRanges()
		{
			return _searchRanges.Aggregate(1, (current, searchRange) => current * searchRange.Items.Count);
		}

		private void CreateTempListOfStringUris(string text, IList<string> tempList, int startAt, int next = 0)
		{
			if (next != 0)
			{
				foreach (var i in _searchRanges[startAt].Items)
				{
					tempList.AddRange(CreateStringUris(text.Replace("{" + startAt + "}", i), next));
				}
			}
			else
			{
				tempList.AddRange(_searchRanges[startAt].Items.Select(i => text.Replace("{" + startAt + "}", i)));
			}
		}

		private void ExecuteEvent()
		{
			var onCollectionChanged = _searchRanges.GetType().GetMethod("OnCollectionChanged",
																		BindingFlags.NonPublic | BindingFlags.Instance,
																		null,
																		new[] { typeof(NotifyCollectionChangedEventArgs) },
																		null);

			onCollectionChanged.Invoke(_searchRanges,
									   new object[]
			                           	{
			                           		new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new object(), -1)
			                           	});
		}
		#endregion

		#region Event
		private void SearchRangesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (_fireOnConstructor) return;

			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Replace:
				case NotifyCollectionChangedAction.Move:
					CreateUris();
					break;

				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Remove:
					if (_uriParametersCount == _searchRanges.Count)
					{
						CreateUris();
					}
					else
					{
						_uris.Clear();
					}
					break;

				case NotifyCollectionChangedAction.Reset:
					_uris.Clear();
					break;
			}
		}
		#endregion
	}
}