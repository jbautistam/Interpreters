using System;

namespace Bau.Libraries.LibDbScripts.Interpreter.Models.SqlTokens
{
	/// <summary>
	///		Sentencia SQL
	/// </summary>
	internal class TokenSqlCommand : TokenSqlBase
	{
		internal TokenSqlCommand(LibInterpreter.Models.Tokens.TokenBase token) : base(token) {}
	}
}
