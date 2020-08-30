using System;

namespace Bau.Libraries.LibInterpreter.Models.Expressions
{
	/// <summary>
	///		Expresión que identifica un paréntesis de apertura o cierre
	/// </summary>
    public class ExpressionParenthesis : ExpressionBase
    {
		public ExpressionParenthesis(bool open)
		{
			Open = open;
		}

		/// <summary>
		///		Clona la expresión
		/// </summary>
		public override ExpressionBase Clone()
		{
			return new ExpressionParenthesis(Open);
		}

		/// <summary>
		///		Obtiene la información de depuración
		/// </summary>
		public override string GetDebugInfo()
		{
			return $"[{(Open ? '(' : ')')}]";
		}

		/// <summary>
		///		Indica si es un paréntesis de apertura
		/// </summary>
		public bool Open { get; }
    }
}
