using System;

namespace Bau.Libraries.LibInterpreter.Models.Expressions
{
	/// <summary>
	///		Expresión para un operador lógico
	/// </summary>
    public class ExpressionOperatorLogical : ExpressionOperatorBase
    {
		/// <summary>
		///		Tipo de operación
		/// </summary>
		public enum LogicalType
		{
			/// <summary>Igual</summary>
			Equal,
			/// <summary>Distinto</summary>
			Distinct,
			/// <summary>Mayor</summary>
			Greater,
			/// <summary>Menor</summary>
			Less,
			/// <summary>Mayor o igual</summary>
			GreaterOrEqual,
			/// <summary>Menor o igual</summary>
			LessOrEqual
		}

		public ExpressionOperatorLogical(LogicalType type)
		{
			Type = type;
		}

		/// <summary>
		///		Clona la expresión
		/// </summary>
		public override ExpressionBase Clone()
		{
			return new ExpressionOperatorLogical(Type);
		}

		/// <summary>
		///		Obtiene la información de depuración
		/// </summary>
		public override string GetDebugInfo()
		{
			return $"[{Type.ToString()}]";
		}

		/// <summary>
		///		Tipo de operación
		/// </summary>
		public LogicalType Type { get; }

		/// <summary>
		///		Obtiene la prioridad de la operación
		/// </summary>
		public override int Priority
		{
			get
			{
				switch (Type)
				{
					case LogicalType.Greater:
					case LogicalType.GreaterOrEqual:
					case LogicalType.Less:
					case LogicalType.LessOrEqual:
						return 18;
					case LogicalType.Equal:
					case LogicalType.Distinct:
						return 17;
					default:
						return 0;
				}
			}
		}
	}
}