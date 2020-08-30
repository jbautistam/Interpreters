using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bau.Libraries.LibDbScripts.Interpreter.Interfaces
{
	/// <summary>
	///		Interface que deben cumplir los sistemas que ejecuten un script de base de aatos
	/// </summary>
	public interface IDbScriptExecutor
	{
		/// <summary>
		///		Ejecuta un comando de SQL
		/// </summary>
		public Task<(bool executed, List<string> errors)> ExecuteAsync(string sql, Dictionary<string, object> parameters, CancellationToken cancellationToken);

		/// <summary>
		///		Escribe una línea en la consola
		/// </summary>
		public void ConsoleWriteLine(string message);
	}
}
