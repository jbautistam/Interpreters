using System;

namespace Bau.Libraries.LibInterpreter.Models.Expressions
{
	/// <summary>
	///		Base para las expresiones de operación
	/// </summary>
    public abstract class ExpressionOperatorBase : ExpressionBase
    {
		/// <summary>
		///		Prioridad
		/// </summary>
		public abstract int Priority { get; }
    }
}
