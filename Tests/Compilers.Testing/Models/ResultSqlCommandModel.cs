using System;
using System.Collections.Generic;

namespace Compilers.Testing.Models
{
	/// <summary>
	///		Resultado de un comando SQL
	/// </summary>
	public class ResultSqlCommandModel : BaseResultModel
	{
		public ResultSqlCommandModel(string sql, Dictionary<string, string> parameters)
		{
			Sql = sql;
			Parameters = parameters;
		}

		/// <summary>
		///		Comando SQL
		/// </summary>
		public string Sql { get; }

		/// <summary>
		///		Parámetros de ejecución
		/// </summary>
		public Dictionary<string, string> Parameters { get; }
	}
}
