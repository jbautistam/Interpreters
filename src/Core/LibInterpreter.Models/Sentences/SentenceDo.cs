using System;

namespace Bau.Libraries.LibInterpreter.Models.Sentences
{
	/// <summary>
	///		Sentencia para un bucle do ... while
	/// </summary>
	public class SentenceDo : SentenceBase
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
