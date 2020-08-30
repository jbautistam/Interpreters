using System;

namespace Bau.Libraries.LibInterpreter.Lexer.Rules
{
	/// <summary>
	///		Definición de una regla
	/// </summary>
	public abstract class RuleBase
	{
		protected RuleBase(string tokenType, bool toFirstSpace, bool mustTrim)
		{
			TokenType = tokenType;
			ToFirstSpace = toFirstSpace;
			MustTrim = mustTrim;
		}

		/// <summary>
		///		Tipo de token
		/// </summary>
		public string TokenType { get; set; }

		/// <summary>
		///		Indica si se debe hacer un trim del texto
		/// </summary>
		public bool MustTrim { get; set; }

		/// <summary>
		///		Indica si se debe leer hasta el primer espacio
		/// </summary>
		public bool ToFirstSpace { get; set; }
	}
}
