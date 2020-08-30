using System;

namespace Bau.Libraries.LibInterpreter.Lexer.Rules
{
	/// <summary>
	///		Regla definida por un patrón
	/// </summary>
	public class RulePattern : RuleBase
	{
		public RulePattern(string tokenType, string patternStart, string patternContent) : base(tokenType, false, true)
		{
			PatternStart = patternStart;
			PatternContent = patternContent;
		}

		/// <summary>
		///		Patrón de inicio
		/// </summary>
		public string PatternStart { get; set; }

		/// <summary>
		///		Patrón de contenido
		/// </summary>
		public string PatternContent { get; set; }
	}
}
