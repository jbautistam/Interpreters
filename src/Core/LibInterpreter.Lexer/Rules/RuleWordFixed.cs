using System;

namespace Bau.Libraries.LibInterpreter.Lexer.Rules
{
	/// <summary>
	///		Regla con los datos de una palabra con longitud fija (sin separadores)
	/// </summary>
	public class RuleWordFixed : RuleBase
	{
		public RuleWordFixed(string tokenType, string word) : this(tokenType, new string[] { word }) {}

		public RuleWordFixed(string tokenType, string[] words) : base(tokenType, false, false)
		{
			Words = words;
		}

		/// <summary>
		///		Palabras clave
		/// </summary>
		public string[] Words { get; set; }
	}
}
