using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibInterpreter.Models.Sentences
{
	/// <summary>
	///		Sentencia de llamada a una subrutina
	/// </summary>
	public class SentenceCallFunction : SentenceBase
	{
		/// <summary>
		///		Nombre de la función a la que se llama
		/// </summary>
		public string Function { get; set; }

		/// <summary>
		///		Parámetros de llamada
		/// </summary>
		public List<Expressions.ExpressionsCollection> Arguments { get; } = new List<Expressions.ExpressionsCollection>();
	}
}
