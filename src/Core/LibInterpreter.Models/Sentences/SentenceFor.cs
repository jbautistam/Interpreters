using System;

namespace Bau.Libraries.LibInterpreter.Models.Sentences
{
	/// <summary>
	///		Sentencia de ejecución de un bucle 
	/// </summary>
	public class SentenceFor : SentenceBase
	{
		/// <summary>
		///		Nombre de variable
		/// </summary>
		public Symbols.SymbolModel Variable { get; set; }

		/// <summary>
		///		Expresión para el valor de inicio
		/// </summary>
		public Expressions.ExpressionsCollection StartExpression { get; } = new Expressions.ExpressionsCollection();

		/// <summary>
		///		Expresión para el valor de fin
		/// </summary>
		public Expressions.ExpressionsCollection EndExpression { get; } = new Expressions.ExpressionsCollection();

		/// <summary>
		///		Expresión para el valor del paso
		/// </summary>
		public Expressions.ExpressionsCollection StepExpression { get; } = new Expressions.ExpressionsCollection();

		/// <summary>
		///		Sentencias a ejecutar
		/// </summary>
		public SentenceCollection Sentences { get; } = new SentenceCollection();
	}
}
