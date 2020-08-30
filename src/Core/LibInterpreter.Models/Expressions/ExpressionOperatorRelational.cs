using System;

namespace Bau.Libraries.LibInterpreter.Models.Expressions
{
	/// <summary>
	///		Expresión para un operador relacional
	/// </summary>
    public class ExpressionOperatorRelational : ExpressionOperatorBase
    {
		/// <summary>
		///		Tipo de operación
		/// </summary>
		public enum RelationalType
		{
			/// <summary>And</summary>
			And,
			/// <summary>Or</summary>
			Or,
			/// <summary>Not</summary>
			Not
		}

		public ExpressionOperatorRelational(RelationalType type)
		{
			Type = type;
		}

		/// <summary>
		///		Clona la expresión
		/// </summary>
		public override ExpressionBase Clone()
		{
			return new ExpressionOperatorRelational(Type);
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
		public RelationalType Type { get; }

		/// <summary>
		///		Obtiene la prioridad de la operación
		/// </summary>
		public override int Priority
		{
			get
			{
				switch (Type)
				{
					case RelationalType.And:
						return 16;
					case RelationalType.Or:
						return 15;
					default:
						return 0;
				}
			}
		}
	}
}