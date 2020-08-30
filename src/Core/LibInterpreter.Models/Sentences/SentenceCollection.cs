using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibInterpreter.Models.Sentences
{
	/// <summary>
	///		Colección de sentencias
	/// </summary>
	public class SentenceCollection : List<SentenceBase>
	{
		/// <summary>
		///		Indica si la colección está vacía
		/// </summary>
		public bool Empty
		{
			get { return Count == 0; }
		}
	}
}
