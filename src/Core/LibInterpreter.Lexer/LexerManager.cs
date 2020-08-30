using System;

using Bau.Libraries.LibInterpreter.Models.Tokens;

namespace Bau.Libraries.LibInterpreter.Lexer
{
	/// <summary>
	///		Manager para el proceso de obtener los tokens de un texto
	/// </summary>
	public class LexerManager
	{
		/// <summary>
		///		Interpreta un texto
		/// </summary>
		public TokenCollection Parse(string source)
		{
			return new Parser.StringTokenSeparator(source, Rules).Parse();
		}

		/// <summary>
		///		Reglas para obtener tokens
		/// </summary>
		public Rules.RuleCollection Rules { get; } = new Rules.RuleCollection();
	}
}
