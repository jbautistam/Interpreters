using System;

namespace Bau.Libraries.LibInterpreter.Models.Expressions
{
	/// <summary>
	///		Expresión con los datos de una constante
	/// </summary>
    public class ExpressionConstant : ExpressionBase
    {
		public ExpressionConstant(Symbols.SymbolModel.SymbolType type, object value)
		{
			Type = type;
			Value = value;
		}

		/// <summary>
		///		Clona la expresión
		/// </summary>
		public override ExpressionBase Clone()
		{
			return new ExpressionConstant(Type, Value);
		}

		/// <summary>
		///		Obtiene la información de depuración
		/// </summary>
		public override string GetDebugInfo()
		{
			return $"[{Type.ToString()} - {Value}]";
		}

		/// <summary>
		///		Tipo de la constante
		/// </summary>
		public Symbols.SymbolModel.SymbolType Type { get; }

		/// <summary>
		///		Valor de la constante
		/// </summary>
		public object Value { get; }
	}
}
