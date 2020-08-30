using System;

namespace Bau.Libraries.LibInterpreter.Models.Sentences
{
	/// <summary>
	///		Sentencia de retorno de una expresión para una función
	/// </summary>
	public class SentenceReturn : SentenceBase
	{
		/// <summary>
		///		Expresión
		/// </summary>
		public Expressions.ExpressionsCollection Expression { get; } = new Expressions.ExpressionsCollection();
	}
}