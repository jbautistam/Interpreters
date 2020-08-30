using System;
using System.Collections.Generic;

using Bau.Libraries.LibInterpreter.Models.Sentences;

namespace Bau.Libraries.LibDbScripts.Interpreter.Models.Sentences
{
	/// <summary>
	///		Clase con las instrucciones de un programa
	/// </summary>
	public class ProgramModel
	{
		/// <summary>
		///		Instrucciones del programa
		/// </summary>
		public SentenceCollection Sentences { get; } = new SentenceCollection();

		/// <summary>
		///		Errores de interpretación
		/// </summary>
		public List<ParseErrorModel> Errors { get; } = new List<ParseErrorModel>();
	}
}
