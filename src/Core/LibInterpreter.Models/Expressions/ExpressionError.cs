using System;

namespace Bau.Libraries.LibInterpreter.Models.Expressions
{
	/// <summary>
	///		Expresión indicando un error
	/// </summary>
    public class ExpressionError : ExpressionBase
    {
		public ExpressionError(string message)
		{
			Message = message;
		}

		/// <summary>
		///		Clona la expresión
		/// </summary>
		public override ExpressionBase Clone()
		{
			return new ExpressionError(Message);
		}

		/// <summary>
		///		Obtiene la información de depuración
		/// </summary>
		public override string GetDebugInfo()
		{
			return $"Error: {Message}]";
		}

		/// <summary>
		///		Mensaje de error
		/// </summary>
		public string Message { get; }
    }
}
