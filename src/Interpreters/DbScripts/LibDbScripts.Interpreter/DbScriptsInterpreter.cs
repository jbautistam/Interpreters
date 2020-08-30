using System;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibLogger.Core;
using Bau.Libraries.LibDbScripts.Interpreter.Models.Sentences;

namespace Bau.Libraries.LibDbScripts.Interpreter
{
	/// <summary>
	///		Intérprete de un script de base de datos
	/// </summary>
	public class DbScriptsInterpreter
	{
		public DbScriptsInterpreter(Interfaces.IDbScriptExecutor dbScriptExecutor, LogManager logger)
		{
			DbScriptExecutor = dbScriptExecutor;
			Logger = logger;
		}

		/// <summary>
		///		Ejecuta un script 
		/// </summary>
		public async Task<bool> ExecuteAsync(string script, System.Collections.Generic.Dictionary<string, object> arguments, CancellationToken cancellationToken)
		{
			ProgramModel program = new Syntactic.ParserProcessor().Parse(script);

				// Si no ha habido errores de interpretación
				if (program.Errors.Count > 0)
					AddErrorsLexer(program);
				else
					try
					{
						await new Processor.DbScriptsProcessor(this).ExecuteAsync(program, arguments, cancellationToken);
					}
					catch (Exception exception)
					{
						Errors.Add($"Error when execute script: {exception.Message}");
					}
				// Devuelve un valor que indica si se ha ejecutado correctamente
				return Errors.Count == 0;
		}

		/// <summary>
		///		Añade los errores del analizador léxico
		/// </summary>
		private void AddErrorsLexer(ProgramModel program)
		{
			foreach (ParseErrorModel error in program.Errors)
				AddError($"Lexical error. Line {error.Line}. {error.Message}");
		}

		/// <summary>
		///		Añade un error
		/// </summary>
		internal void AddError(string error, Exception exception = null)
		{
			// Añade el contenido de la excepción al mensaje de error
			if (exception != null)
				error += Environment.NewLine + exception.Message;
			// Añade el error a la lista
			Errors.Add(error);
		}

		/// <summary>
		///		Clase para ejecutar los comandos SQL
		/// </summary>
		public Interfaces.IDbScriptExecutor DbScriptExecutor { get; }

		/// <summary>
		///		Manager de log
		/// </summary>
		public LogManager Logger { get; }

		/// <summary>
		///		Errores
		/// </summary>
		public System.Collections.Generic.List<string> Errors { get; } = new System.Collections.Generic.List<string>();
	}
}
