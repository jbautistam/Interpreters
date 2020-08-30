using System;

namespace Bau.Libraries.LibInterpreter.Models.Expressions
{
	/// <summary>
	///		Clase con los datos de una expresión
	/// </summary>
	public abstract class ExpressionBase
	{
		/// <summary>
		///		Información de depuración
		/// </summary>
		public abstract string GetDebugInfo();

		/// <summary>
		///		Clona una expresión
		/// </summary>
		public abstract ExpressionBase Clone();
	}
}
