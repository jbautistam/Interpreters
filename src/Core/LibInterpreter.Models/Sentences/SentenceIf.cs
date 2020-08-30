using System;

namespace Bau.Libraries.LibInterpreter.Models.Sentences
{
	/// <summary>
	///		Sentencia de ejecución de una condición
	/// </summary>
	public class SentenceIf : SentenceBase
	{
		/// <summary>
		///		Condición
		/// </summary>
		public Expressions.ExpressionsCollection Condition { get; } = new Expressions.ExpressionsCollection();

		/// <summary>
		///		Sentencias a ejecutar si el resultado de la condición es verdadero
		/// </summary>
		public SentenceCollection SentencesThen { get; } = new SentenceCollection();

		/// <summary>
		///		Sentencias a ejecutar si el resultado de la condición es false
		/// </summary>
		public SentenceCollection SentencesElse { get; } = new SentenceCollection();
	}
}
