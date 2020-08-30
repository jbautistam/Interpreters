using System;

namespace Bau.Libraries.LibDbScripts.Interpreter.Models.SqlTokens
{
	/// <summary>
	///		Token de un script SQL
	/// </summary>
	internal class TokenSqlBase
	{
		internal TokenSqlBase(LibInterpreter.Models.Tokens.TokenBase token)
		{
			ReadToken = token;
		}

		/// <summary>
		///		Token leido
		/// </summary>
		internal LibInterpreter.Models.Tokens.TokenBase ReadToken { get; }
	}
}
