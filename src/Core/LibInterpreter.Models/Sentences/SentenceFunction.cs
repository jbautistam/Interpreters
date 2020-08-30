using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibInterpreter.Models.Sentences
{
	/// <summary>
	///		Sentencia de declaración de una función
	/// </summary>
	public class SentenceFunction : SentenceBase
	{
		/// <summary>
		///		Nombre y tipo de la función
		/// </summary>
		public Symbols.SymbolModel Definition { get; } = new Symbols.SymbolModel();

		/// <summary>
		///		Argumentos de la función
		/// </summary>
		public List<Symbols.SymbolModel> Arguments { get; } = new List<Symbols.SymbolModel>();

		/// <summary>
		///		Contenido de la función
		/// </summary>
		public SentenceCollection Sentences { get; } = new SentenceCollection();
	}
}
