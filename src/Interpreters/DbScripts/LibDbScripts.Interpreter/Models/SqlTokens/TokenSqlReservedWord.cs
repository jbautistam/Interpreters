using System;

namespace Bau.Libraries.LibDbScripts.Interpreter.Models.SqlTokens
{
	/// <summary>
	///		Palabra reservada
	/// </summary>
	internal class TokenSqlReservedWord : TokenSqlBase
	{
		/// <summary>
		///		Tipo de palabra reservada
		/// </summary>
		internal enum ReservedWordType
		{
			Unknown,
			If,
			Else,
			Declare,
			Set,
			Int,
			String,
			Date,
			Float,
			For,
			To,
			Step,
			While,
			Do,
			Sub,
			Call,
			Return
		}

		internal TokenSqlReservedWord(LibInterpreter.Models.Tokens.TokenBase token) : base(token) 
		{
			Type = GetType(token.Value);
		}

		/// <summary>
		///		Obtiene el tipo de palabra reservada a partir de la cadena del token
		/// </summary>
		private ReservedWordType GetType(string type)
		{
			if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse(type, true, out ReservedWordType result))
				return result;
			else
				return ReservedWordType.Unknown;
		}

		/// <summary>
		///		Tipo de palabra reservada
		/// </summary>
		internal ReservedWordType Type { get; }
	}
}
