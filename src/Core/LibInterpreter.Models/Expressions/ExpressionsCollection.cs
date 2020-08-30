using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibInterpreter.Models.Expressions
{
	/// <summary>
	///		Colección de <see cref="ExpressionBase"/>
	/// </summary>
	public class ExpressionsCollection : List<ExpressionBase>
	{
		/// <summary>
		///		Obtiene una cadena de depuración
		/// </summary>
		public string GetDebugInfo()
		{
			string debug = "";

				// Añade los datos a la cadena de depuración
				foreach (ExpressionBase expression in this)
				{
					if (!string.IsNullOrEmpty(debug))
						debug += " # ";
					debug += expression.GetDebugInfo();
				}
				// Devuelve la cadena de depuración
				return debug;
		}

		/// <summary>
		///		Clona la colección de expresiones
		/// </summary>
		public ExpressionsCollection Clone()
		{
			ExpressionsCollection expressions = new ExpressionsCollection();

				// Clona las expresiones
				foreach (ExpressionBase expression in this)
					expressions.Add(expression.Clone());
				// Devuelve la colección
				return expressions;
		}

		/// <summary>
		///		Indica si la lista de expresiones está vacía
		/// </summary>
		public bool Empty
		{
			get { return Count == 0; }
		}
	}
}
