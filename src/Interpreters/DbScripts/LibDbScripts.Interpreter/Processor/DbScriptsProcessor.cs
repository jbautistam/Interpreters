using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibDbScripts.Interpreter.Models.Sentences;
using Bau.Libraries.LibInterpreter.Interpreter.Context.Functions;
using Bau.Libraries.LibInterpreter.Interpreter.Context.Variables;

namespace Bau.Libraries.LibDbScripts.Interpreter.Processor
{
	/// <summary>
	///		Proceso de interpretación de un script de base de datos
	/// </summary>
	public class DbScriptsProcessor : LibInterpreter.Interpreter.ProgramProcessor
	{
		public DbScriptsProcessor(DbScriptsInterpreter interpreter) : base(new LibInterpreter.Interpreter.ProcessorOptions())
		{
			Interpreter = interpreter;
		}

		/// <summary>
		///		Ejecuta un script 
		/// </summary>
		public async Task ExecuteAsync(ProgramModel program, Dictionary<string, object> arguments, CancellationToken cancellation)
		{
			// Inicializa la ejecución de scripts
			Initialize(arguments);
			// Inicializa las funciones
			Context.Actual.FunctionsTable.AddRange(GetImplicitFunctions());
			// Ejecuta las sentencias
			await ExecuteAsync(program.Sentences, cancellation);
		}

		/// <summary>
		///		Obtiene la función implícita para añadir un valor a una fecha
		/// </summary>
		private List<ImplicitFunctionModel> GetImplicitFunctions()
		{
			List<ImplicitFunctionModel> functions = new List<ImplicitFunctionModel>();

				// Genera las funciones implícitas
				functions.Add(new ImplicitFunctionBuilder("dateadd", LibInterpreter.Models.Symbols.SymbolModel.SymbolType.Unknown)
											.WithArgument("date",LibInterpreter.Models.Symbols.SymbolModel.SymbolType.Date)
											.WithArgument("increment",LibInterpreter.Models.Symbols.SymbolModel.SymbolType.Numeric)
											.WithArgument("interval", LibInterpreter.Models.Symbols.SymbolModel.SymbolType.String)
										.Build()
							 );
				functions.Add(new ImplicitFunctionBuilder("print", LibInterpreter.Models.Symbols.SymbolModel.SymbolType.Unknown)
											.WithArgument("message", LibInterpreter.Models.Symbols.SymbolModel.SymbolType.String)
										.Build()
							 );
				// Devuelve las funciones predefinidas
				return functions;
		}

		/// <summary>
		///		Ejecuta las sentencias particulares de base de datos
		/// </summary>
		protected override async Task ExecuteAsync(SentenceBase sentence, CancellationToken cancellationToken)
		{
			switch (sentence)
			{
				case SentenceSql executionSentence:
						await ExecuteSqlAsync(executionSentence, cancellationToken);
					break;
				default:
						AddError("Unknown sentence");
					break;
			}
		}

		/// <summary>
		///		Ejecuta las funciones implícitas
		/// </summary>
		protected override async Task<VariableModel> ExecuteAsync(ImplicitFunctionModel function, CancellationToken cancellationToken)
		{
			VariableModel result = null;

				// Evita el warning de async / await
				await Task.Delay(1);
				// Ejecuta la función adecuada
				if (string.IsNullOrWhiteSpace(function.Definition.Name))
					AddError("Function unreconized");
				else if (function.Definition.Name.Equals("print", StringComparison.CurrentCultureIgnoreCase))
					result = ExecutePrint(function);
				else
					AddError($"Function undefined: {function.Definition.Name}");
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Ejecuta una sentencia SQL
		/// </summary>
		private async Task ExecuteSqlAsync(SentenceSql sentence, CancellationToken cancellationToken)
		{
			try
			{
				(bool executed, List<string> errors) = await Interpreter.DbScriptExecutor.ExecuteAsync(sentence.Command, 
																									   GetParameters(Context.Actual.GetVariablesRecursive()), 
																									   cancellationToken);

					// Procesa los errores
					if (errors.Count > 0)
					{
						string total = string.Empty;

							// Añade todos los errores al total
							foreach (string error in errors)
								total += error + Environment.NewLine;
							// Añade el error al procesador
							AddError(total);
					}
			}
			catch (Exception exception)
			{
				AddError($"Error when execute sql command: {sentence.Command}", exception);
			}
		}

		/// <summary>
		///		Ejecuta una sentencia de impresión
		/// </summary>
		private VariableModel ExecutePrint(ImplicitFunctionModel function)
		{
			VariableModel result = Context.Actual.VariablesTable[function.Arguments[0].Name]; 
			
				// Imprime el resultado
				if (result == null)
					AddError($"Method Print: Cant find value for argument {function.Arguments[0].Name}");
				else
					Interpreter.DbScriptExecutor.ConsoleWriteLine(result.GetStringValue());
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Obtiene los parámetros a partir de una tabla de variables
		/// </summary>
		private Dictionary<string, object> GetParameters(TableVariableModel tableVariable)
		{
			Dictionary<string, object> parameters = new Dictionary<string, object>();

				// Transforma los parámetros
				foreach (KeyValuePair<string, VariableModel> variable in tableVariable.GetAll())
					parameters.Add(variable.Value.Name, variable.Value.Value);
				// Devuelve los parámetros
				return parameters;
		}

		/// <summary>
		///		Añade un mensaje de depuración
		/// </summary>
		protected override void AddDebug(string message, [CallerFilePath] string fileName = null, 
										 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			Interpreter.Logger.Default.LogItems.Debug(message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un mensaje informativo
		/// </summary>
		protected override void AddInfo(string message, [CallerFilePath] string fileName = null, 
										[CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			Interpreter.Logger.Default.LogItems.Info(message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade una cadena a la consola
		/// </summary>
		protected override void AddConsoleOutput(string message, [CallerFilePath] string fileName = null, 
												 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			Interpreter.DbScriptExecutor.ConsoleWriteLine(message);
			Interpreter.Logger.Default.LogItems.Console(message, fileName, methodName, lineNumber);
		}

		/// <summary>
		///		Añade un error
		/// </summary>
		protected override void AddError(string error, Exception exception = null, [CallerFilePath] string fileName = null, 
										 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0)
		{
			// Añade el error al log
			Interpreter.Logger.Default.LogItems.Error(error, exception, fileName, methodName, lineNumber);
			// Detiene el procesamiento
			Stopped = true;
			// Añade el error al intérprete
			Interpreter.AddError(error, exception);
		}

		/// <summary>
		///		Intérprete
		/// </summary>
		private DbScriptsInterpreter Interpreter { get; }
	}
}
