using System;

namespace Bau.Libraries.LibInterpreter.Models.Expressions
{
	/// <summary>
	///		Expresión de tipo variable
	/// </summary>
	public class ExpressionVariableIdentifier : ExpressionBase
	{
		public ExpressionVariableIdentifier(string name)
		{
			Name = name;
		}

		/// <summary>
		///		Clona el identificador de variable
		/// </summary>
		public override ExpressionBase Clone()
		{
			ExpressionVariableIdentifier variable = new ExpressionVariableIdentifier(Name);

				// Clona las expresiones
				variable.IndexExpressions = IndexExpressions.Clone();
				variable.IndexExpressionsRPN = IndexExpressionsRPN.Clone();
				if (Member != null)
					variable.Member = Member.Clone() as ExpressionVariableIdentifier;
				// Devuelve el objeto clonado
				return variable;
		}

		/// <summary>
		///		Obtiene la información de depuración
		/// </summary>
		public override string GetDebugInfo()
		{
			string debug = Name;

				// Añade el índice
				if (IndexExpressions?.Count > 0)
					debug += "[" + IndexExpressions.GetDebugInfo() + "]";
				// Añade el índice en formato RPN
				if (IndexExpressionsRPN?.Count > 0)
					debug += " (RPN: " + IndexExpressionsRPN.GetDebugInfo() + ")";
				// Añade el miembro
				if (Member != null)
					debug += "->" + Member.GetDebugInfo();
				// Devuelve la información de depuración
				return debug;
		}

		/// <summary>
		///		Nombre de la variable
		/// </summary>
		public string Name { get; }

		/// <summary>
		///		Expresiones de índice
		/// </summary>
		public ExpressionsCollection IndexExpressions { get; set; } = new ExpressionsCollection();

		/// <summary>
		///		Expresiones del índice en formato RPN
		/// </summary>
		public ExpressionsCollection IndexExpressionsRPN { get; set; } = new ExpressionsCollection();

		/// <summary>
		///		Identificador de variable
		/// </summary>
		public ExpressionVariableIdentifier Member { get; set; }
	}
}
