using System;
using System.Collections.Generic;

namespace Compilers.Testing.Processors
{
	/// <summary>
	///		Ejecutor de sentencias SQL para pruebas
	/// </summary>
	internal class DbScriptFakeExecutor : Bau.Libraries.LibDbScripts.Interpreter.Interfaces.IDbScriptExecutor
	{
		/// <summary>
		///		Ejecuta una sentencia SQL
		/// </summary>
		public bool Execute(string sql, Dictionary<string, object> parameters)
		{
			Dictionary<string, string> normalized = Normalize(parameters);

				// Añade la sentencia
				Results.Add(new Models.ResultSqlCommandModel(sql, normalized));
				// Indica que se ha ejecutado
				return true;
		}

		/// <summary>
		///		Normaliza el diccionario con cadenas
		/// </summary>
		private Dictionary<string, string> Normalize(Dictionary<string, object> parameters)
		{
			Dictionary<string, string> converted = new Dictionary<string, string>();

				// Convierte los parámetros a cadenas
				foreach (KeyValuePair<string, object> parameter in parameters)
					if (parameter.Value == null)
						converted.Add(parameter.Key, string.Empty);
					else
						converted.Add(parameter.Key, parameter.Value.ToString());
				// Devuelve la colección de parámetros convertidos
				return converted;
		}

		/// <summary>
		///		Escribe un mensaje en la consola
		/// </summary>
		public void ConsoleWriteLine(string message)
		{
			Results.Add(new	Models.ResultMessageModel(message));
		}

		/// <summary>
		///		Resultados de ejecución del script
		/// </summary>
		internal Models.ProgramResultModelCollection Results { get; } = new Models.ProgramResultModelCollection();
	}
}
