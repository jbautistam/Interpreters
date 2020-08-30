using System;

namespace Bau.Libraries.LibDbScripts.Interpreter.Models.Sentences
{
	/// <summary>
	///		Clase con los datos de un error de interpretación
	/// </summary>
	public class ParseErrorModel
	{
		public ParseErrorModel(int line, string message)
		{
			Line = line;
			Message = message;
		}

		/// <summary>
		///		línea donde se ha producido el error
		/// </summary>
		public int Line { get; }

		/// <summary>
		///		Mensaje de error
		/// </summary>
		public string Message { get; }
	}
}
