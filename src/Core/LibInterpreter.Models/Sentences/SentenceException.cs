using System;

namespace Bau.Libraries.LibInterpreter.Models.Sentences
{
	/// <summary>
	///		Clase con los datos de una excepción
	/// </summary>
	public class SentenceException : SentenceBase
	{
		/// <summary>
		///		Mensaje de error
		/// </summary>
		public string Message { get; set; }
	}
}
