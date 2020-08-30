using System;

namespace Bau.Libraries.LibInterpreter.Models.Sentences
{
	/// <summary>
	///		Sentencia para ejecución de una expresión
	/// </summary>
	public class SentenceLet : SentenceBase
	{
		/// <summary>
		///		Nombre de variable
		/// </summary>
		public string VariableName { get; set; }

		/// <summary>
		///		Expresión a ejecutar
		/// </summary>
		public Expressions.ExpressionsCollection Expressions { get; } = new Expressions.ExpressionsCollection();
	}
}
