using System;

namespace Bau.Libraries.LibInterpreter.Models.Expressions
{
	/// <summary>
	///		Expresión para un operador matemático
	/// </summary>
    public class ExpressionOperatorMath : ExpressionOperatorBase
    {
		/// <summary>
		///		Tipo de operación
		/// </summary>
		public enum MathType
		{
			/// <summary>Suma</summary>
			Sum,
			/// <summary>Resta</summary>
			Substract,
			/// <summary>Multiplicación</summary>
			Multiply,
			/// <summary>División</summary>
			Divide,
			/// <summary>Módulo</summary>
			Modulus
		}

		public ExpressionOperatorMath(MathType type)
		{
			Type = type;
		}

		/// <summary>
		///		Clona la expresión
		/// </summary>
		public override ExpressionBase Clone()
		{
			return new ExpressionOperatorMath(Type);
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
		public MathType Type { get; }

		/// <summary>
		///		Obtiene la prioridad de la operación
		/// </summary>
		public override int Priority
		{
			get
			{
				switch (Type)
				{
					case MathType.Multiply:
					case MathType.Divide:
					case MathType.Modulus:
						return 20;
					case MathType.Sum:
					case MathType.Substract:
						return 19;
					default:
						return 0;
				}
			}
		}
	}
}
