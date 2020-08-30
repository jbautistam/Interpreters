using System;

namespace Bau.Libraries.LibInterpreter.Lexer.Rules
{
	/// <summary>
	///		Regla de definición para una palabra
	/// </summary>
	public class RuleWord : RuleBase
	{
		public RuleWord(string tokenType, string[] words, string[] separators, bool toFirstSpace) : base(tokenType, toFirstSpace, true)
		{
			Words = words;
			Separators = separators;
		}

		/// <summary>
		///		Palabras que definen la regla
		/// </summary>
		public string[] Words { get; set; }

		/// <summary>
		///		Separadores
		/// </summary>
		public string[] Separators { get; set; }
	}
}
