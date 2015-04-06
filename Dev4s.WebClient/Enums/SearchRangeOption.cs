namespace Dev4s.WebClient.Enums
{
	public enum SearchRangeOption
	{
		/// <summary>
		/// From 0 to 127 (int => char). Does not take control chars and non printable ones.
		/// </summary>
		AsciiChars,

		/// <summary>
		/// English small letters (from a to z).
		/// </summary>
		EnLetters,

		/// <summary>
		/// English small letters plus Polish small letters (from a to z, and specific small Polish ones)
		/// </summary>
		EnPlusPlLetters,

		/// <summary>
		/// From 0 to 127 (int => char) plus Polish small and big letters. 
		/// Does not take control chars and non printable ones.
		/// </summary>
		AsciiPlusPlChars
	}
}