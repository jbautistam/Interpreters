using System;

namespace Bau.Libraries.LibInterpreter.Models.Sentences
{
	/// <summary>
	///		Sentencia para un bucle While
	/// </summary>
	public class SentenceWhile : SentenceBase
	{
		/// <summary>
		///		Condición
		/// </summary>
		public Expressions.ExpressionsCollection Condition { get; } = new Expressions.ExpressionsCollection();

		/// <summary>
		///		Sentencias
		/// </summary>
		public SentenceCollection Sentences { get; } = new SentenceCollection();
	}
}
