using System;

namespace Bau.Libraries.LibInterpreter.Models.Symbols
{
	/// <summary>
	///		Clase con los datos de un símbolo: variable, nombre de función, nombre de rutina...
	/// </summary>
	public class SymbolModel
	{
		/// <summary>
		///		Tipo de símbolo
		/// </summary>
		public enum SymbolType
		{
			/// <summary>Desconocida: se utiliza cuando el compilador infiere el tipo del símbolo</summary>
			Unknown,
			/// <summary>Cadena</summary>
			String,
			/// <summary>Valor numérico</summary>
			Numeric,
			/// <summary>Valor lógico</summary>
			Boolean,
			/// <summary>Valor de fecha</summary>
			Date
		}

		/// <summary>
		///		Nombre del símbolo
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Tipo de símbolo
		/// </summary>
		public SymbolType Type { get; set; }
	}
}
