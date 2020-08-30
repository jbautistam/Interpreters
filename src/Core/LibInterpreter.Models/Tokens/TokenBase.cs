using System;

namespace Bau.Libraries.LibInterpreter.Models.Tokens
{
	/// <summary>
	///		Clase base con los datos de un token
	/// </summary>
	public class TokenBase
	{
		public TokenBase(string type, int row, int column, string value)
		{
			Type = type;
			Row = row;
			Column = column;
			Indent = 0;
			Value = value;
		}

		/// <summary>
		///		Obtiene la información de depuración del token
		/// </summary>
		public virtual string GetDebugInfo()
		{
			return $"{Type} (R {Row} - C {Column} - I {Indent}) : #{Value}#";
		}

		/// <summary>
		///		Tipo del token
		/// </summary>
		public string Type { get; }

		/// <summary>
		///		Fila
		/// </summary>
		public int Row { get; set; }

		/// <summary>
		///		Columna
		/// </summary>
		public int Column { get; set; }

		/// <summary>
		///		Indentación
		/// </summary>
		public int Indent { get; set; }

		/// <summary>
		///		Contenido del token
		/// </summary>
		public string Value { get; set; }
	}
}