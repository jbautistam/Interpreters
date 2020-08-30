using System;
using System.Collections.Generic;

namespace Compilers.Testing.Processors
{
	/// <summary>
	///		Lector de un archivo de resultados de un scritp
	/// </summary>
	public class DbScritsResultReader
	{
		// Variables privadas
		private int _actualLine = 0;
		private string [] _lines;

		/// <summary>
		///		Carga los resultados de un script
		/// </summary>
		public Models.ProgramResultModelCollection Load(string script)
		{
			Models.ProgramResultModelCollection results = new Models.ProgramResultModelCollection();

				// Inicializa el número de línea actual y las línes del script
				_actualLine = 0;
				_lines = script.Split(Environment.NewLine);
				// Lee las líneas
				while (_actualLine < _lines.Length)
				{
					string line = NextLine();

						// Obtiene la línea
						if (IsCommand(line, "Sql"))
							results.Add(LoadCommandSql());
						else if (IsCommand(line, "Message"))
							results.Add(LoadCommandMessage());
						else if (!string.IsNullOrWhiteSpace(line)) // ... por el fin de archivo
							throw new ArgumentException($"Línea desconocida: {line}");
				}
				// Devuelve los resultados
				return results;
		}

		/// <summary>
		///		Carga el comando SQL
		/// </summary>
		private Models.ResultSqlCommandModel LoadCommandSql()
		{
			string line = NextLine();
			string sql = string.Empty;
			Dictionary<string, string> parameters = new Dictionary<string, string>();

				// Lee el cuerpo
				if (IsCommand(line, "Body"))
					sql = LoadCommandSqlBody();
				else
					throw new ArgumentException("Falta el cuerpo del comando SQL");
				// Lee los parámetros
				line = NextLine();
				if (IsCommand(line, "Parameter"))
				{
					// Lee los parámetros
					parameters = LoadCommandSqlParameters();
					// Lo siguiente debería ser EndSql
					line = NextLine();
					if (!IsCommand(line, "EndSql"))
						throw new ArgumentException($"Falta el final del comando SQL {line}");
				}
				else if (!IsCommand(line, "EndSql"))
					throw new ArgumentException($"Falta el final del comando SQL {line}");
				// Devuelve la sentencia del comando SQL
				return new Models.ResultSqlCommandModel(sql, parameters);
		}

		/// <summary>
		///		Carga el cuerpo del comando
		/// </summary>
		private string LoadCommandSqlBody()
		{
			string line = NextLine();
			string body = string.Empty;

				// Lee el cuerpo del mensaje
				while (!string.IsNullOrWhiteSpace(line) && !IsCommand(line, "EndBody"))
				{
					// Añade la línea al cuerpo
					body += line + Environment.NewLine;
					// Pasa a la siguiente línea
					line = NextLine();
				}
				// La línea final debe ser EndBody
				if (!IsCommand(line, "EndBody"))
					throw new ArgumentException("Falta EndBody en el comando SQL");
				else
					return body;
		}

		/// <summary>
		///		Carga los parámetros SQL
		/// </summary>
		private Dictionary<string, string> LoadCommandSqlParameters()
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			string line = NextLine();

				// Lee los parámetros del mensaje
				while (!string.IsNullOrWhiteSpace(line) && !IsCommand(line, "EndParameter"))
				{
					string [] parts = line.Split('#');

						// Añade los valores al diccionario
						if (parts.Length == 1 || (parts.Length == 2 && string.IsNullOrWhiteSpace(parts[1])))
							parameters.Add(parts[0].Trim().ToUpper(), string.Empty);
						else
							parameters.Add(parts[0].Trim().ToUpper(), parts[1].Trim());
						// Pasa a la siguiente línea
						line = NextLine();
				}
				// La línea final debe ser EndParameter
				if (!IsCommand(line, "EndParameter"))
					throw new ArgumentException("Falta EndParameter en la lista de parámetros");
				else
					return parameters;
		}

		/// <summary>
		///		Carga el mensaje del comando
		/// </summary>
		private Models.ResultMessageModel LoadCommandMessage()
		{
			string line = NextLine();
			string body = string.Empty;

				// Lee el cuerpo del mensaje
				while (!string.IsNullOrWhiteSpace(line) && !IsCommand(line, "EndMessage"))
				{
					// Añade la línea al cuerpo
					body += line + Environment.NewLine;
					// Pasa a la siguiente línea
					line = NextLine();
				}
				// La línea final debe ser EndMessage
				if (!IsCommand(line, "EndMessage"))
					throw new ArgumentException("Falta EndMessage en el comando");
				else
					return new Models.ResultMessageModel(body);
		}

		/// <summary>
		///		Se coloca en la siguiente línea no vacía
		/// </summary>
		private string NextLine()
		{
			string line = string.Empty;

				// Recorre las líneas buscando una línea que no esté vacía
				while (_actualLine < _lines.Length && string.IsNullOrWhiteSpace(line))
				{
					// Obtiene la línea
					line = _lines[_actualLine];
					if (!string.IsNullOrWhiteSpace(line))
						line = line.Trim();
					// Pasa a la siguiente línea
					_actualLine++;
				}
				// Devuelve la línea actual
				return line;
		}

		/// <summary>
		///		Comprueba si un texto es un comando
		/// </summary>
		private bool IsCommand(string line, string command)
		{
			if (string.IsNullOrWhiteSpace(line))
				return false;
			else
				return line.Equals(command, StringComparison.CurrentCultureIgnoreCase);
		}
	}
}
