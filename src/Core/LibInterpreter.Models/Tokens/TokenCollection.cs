using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibInterpreter.Models.Tokens
{
	/// <summary>
	///		Colección de <see cref="TokenBase"/>
	/// </summary>
	public class TokenCollection : List<TokenBase>
	{
		/// <summary>
		///		Añade una palabra a la colección
		/// </summary>
		public void Add(string type, int row, int column, string value)
		{
			Add(new TokenBase(type, row, column, value));
		}

		/// <summary>
		///		Imprime las líneas de depuración
		/// </summary>
		public void Debug()
		{
			foreach (TokenBase token in this)
				System.Diagnostics.Debug.WriteLine(token.GetDebugInfo());
		}
	}
}
