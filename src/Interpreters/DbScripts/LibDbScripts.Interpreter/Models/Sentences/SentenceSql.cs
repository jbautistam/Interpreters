using System;

using Bau.Libraries.LibInterpreter.Models.Sentences;

namespace Bau.Libraries.LibDbScripts.Interpreter.Models.Sentences
{
	/// <summary>
	///		Sentencia de ejecución de un comando
	/// </summary>
	public class SentenceSql : SentenceBase
	{
		/// <summary>
		///		Comando SQL a ejecutar
		/// </summary>
		public string Command { get; set; }
	}
}
