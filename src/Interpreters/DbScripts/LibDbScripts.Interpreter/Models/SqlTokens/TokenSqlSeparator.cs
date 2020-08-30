using System;

namespace Bau.Libraries.LibDbScripts.Interpreter.Models.SqlTokens
{
	/// <summary>
	///		Separador
	/// </summary>
	internal class TokenSqlSeparator : TokenSqlBase
	{
		/// <summary>
		///		Tipos de separador
		/// </summary>
		internal enum SeparatorType
		{
			Unknown,
			OpenParenthesis,
			CloseParenthesis,
			Sum,
			Minus,
			Multiply,
			Divide,
			Modulus,
			Less,
			Greater,
			LessOrEqual,
			GreaterOrEqual,
			Equal,
			Distinct,
			Or,
			And,
			Not,
			Assign,
			Comma,
			OpenBlock,
			CloseBlock,
			EndSentence
		}

		internal TokenSqlSeparator(LibInterpreter.Models.Tokens.TokenBase token) : base(token) 
		{
			Type = GetType(token.Value);
		}

		/// <summary>
		///		Obtiene el tipo de la cadena
		/// </summary>
		private SeparatorType GetType(string value)
		{
			// Obtiene el tipo a partir de la cadena
			if (!string.IsNullOrWhiteSpace(value))
				switch (value)
				{
					case "(":
						return SeparatorType.OpenParenthesis;
					case ")":
						return SeparatorType.CloseParenthesis;
					case "+":
						return SeparatorType.Sum;
					case "-":
						return SeparatorType.Minus;
					case "*":
						return SeparatorType.Multiply;
					case "/":
						return SeparatorType.Divide;
					case "%":
						return SeparatorType.Modulus;
					case "<":
						return SeparatorType.Less;
					case ">":
						return SeparatorType.Greater;
					case ">=":
						return SeparatorType.GreaterOrEqual;
					case "<=":
						return SeparatorType.LessOrEqual;
					case "==":
						return SeparatorType.Equal;
					case "!=":
						return SeparatorType.Distinct;
					case "||":
						return SeparatorType.Or;
					case "&&":
						return SeparatorType.And;
					case "!":
						return SeparatorType.Not;
					case "=":
						return SeparatorType.Assign;
					case ",":
						return SeparatorType.Comma;
					case "{":
						return SeparatorType.OpenBlock;
					case "}":
						return SeparatorType.CloseBlock;
					case ";":
						return SeparatorType.EndSentence;
				}
			// Si ha llegado hasta aquí es porque no ha encontrado ningún valor válido
			return SeparatorType.Unknown;
		}

		/// <summary>
		///		Comprueba si es un token asociado a expresiones
		/// </summary>
		internal bool IsOperator()
		{
			return Type == SeparatorType.OpenParenthesis || Type == SeparatorType.CloseParenthesis || 
				   Type == SeparatorType.Sum || Type == SeparatorType.Minus || Type == SeparatorType.Multiply || Type == SeparatorType.Divide || Type == SeparatorType.Modulus ||
				   Type == SeparatorType.Less || Type == SeparatorType.Greater || Type == SeparatorType.LessOrEqual || Type == SeparatorType.GreaterOrEqual ||
				   Type == SeparatorType.Distinct || 
				   Type == SeparatorType.Or || Type == SeparatorType.And || Type == SeparatorType.Not;
		}

		/// <summary>
		///		Tipo del serparador
		/// </summary>
		internal SeparatorType Type { get; }
	}
}
