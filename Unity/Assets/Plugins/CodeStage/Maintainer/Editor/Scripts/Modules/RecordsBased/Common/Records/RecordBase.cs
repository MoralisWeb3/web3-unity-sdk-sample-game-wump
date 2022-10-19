#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer
{
	using System;
	using System.Text;
	using Core;
	using Core.Scan;

	/// <summary>
	/// Base class for all record-based modules results items.
	/// </summary>
	[Serializable]
	public abstract class RecordBase
	{
		/// <summary>
		/// LocationGroup of the item.
		/// </summary>
		public LocationGroup LocationGroup { get; private set; }

		internal bool compactMode = true;
		internal bool selected = true;

		public string HeaderPostfix { get; set; }
		public string BodyPostfix { get; set; }

		protected StringBuilder cachedHeader;
		protected StringBuilder cachedCompactLine;
		protected StringBuilder cachedBody;

		protected RecordBase(LocationGroup locationGroup)
		{
			LocationGroup = locationGroup;
		}

		/// <summary>
		/// Gets item label in a single line compact form.
		/// </summary>
		/// <returns>Compacted item label.</returns>
		public string GetCompactLine()
		{
			if (cachedCompactLine != null) return cachedCompactLine.ToString();

			cachedCompactLine = new StringBuilder();
			ConstructCompactLine(cachedCompactLine);
			return cachedCompactLine.ToString();
		}

		/// <summary>
		/// Gets item header.
		/// </summary>
		/// <returns>Full header of the item, includes RichText tags..</returns>
		public string GetHeader()
		{
			if (cachedHeader != null) 
				return cachedHeader.ToString();

			cachedHeader = new StringBuilder();
			cachedHeader.Append("<b><size=14>");

			ConstructHeader(cachedHeader);

			if (!string.IsNullOrEmpty(HeaderPostfix))
			{
				cachedHeader.Append(' ').Append(HeaderPostfix);
			}

			cachedHeader.Append("</size></b>");

			return cachedHeader.ToString();
		}

		/// <summary>
		/// Gets item body, may be multiline.
		/// </summary>
		/// <returns>Full item body.</returns>
		public string GetBody()
		{
			if (cachedBody != null) return cachedBody.ToString();

			cachedBody = new StringBuilder();

			ConstructBody(cachedBody);

			if (!string.IsNullOrEmpty(BodyPostfix))
			{
				cachedBody.Append("\n").Append(BodyPostfix);
			}

			return cachedBody.ToString();
		}

		/// <summary>
		/// Gets full multiline item description constructed from header and body.
		/// </summary>
		/// <returns>Full item description.</returns>
		public override string ToString()
		{
			return GetHeader() + "\n" + GetBody();
		}

		/// <summary>
		/// Same as parameterless version but clears RichText (html) tags out.
		/// </summary>
		/// <param name="clearHtml">Clears all RichText (html) tags when true, and leaves them untouched otherwise.</param>
		/// <returns>Full item description without RichText (html) tags.</returns>
		public string ToString(bool clearHtml)
		{
			return clearHtml ? StripTagsCharArray(ToString()) : ToString();
		}

		internal abstract bool MatchesFilter(FilterItem newFilter);

		protected abstract void ConstructCompactLine(StringBuilder text);
		protected abstract void ConstructHeader(StringBuilder text);
		protected abstract void ConstructBody(StringBuilder text);

		// source: http://www.dotnetperls.com/remove-html-tags
		private static string StripTagsCharArray(string input)
		{
			var arrayIndex = 0;
			var inside = false;
			var len = input.Length;

			var array = new char[len];

			for (var i = 0; i < len; i++)
			{
				var let = input[i];

				if (let == '<')
				{
					inside = true;
					continue;
				}
				if (let == '>')
				{
					inside = false;
					continue;
				}

				if (inside) continue;

				array[arrayIndex] = @let;
				arrayIndex++;
			}
			return new string(array, 0, arrayIndex);
		}
	}
}