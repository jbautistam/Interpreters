using System;
using Xunit;

using Bau.Libraries.LibDbScripts.Interpreter;
using Compilers.Testing.Models;
using System.Collections.Generic;

namespace Compilers.Testing
{
	public class DbScriptsInterpreter_Should
	{
		[Theory]
		[InlineData("FirstTest.sqlx", "FirstTest.rst", false)]
		[InlineData("Test2.sqlx", "Test2.rst", false)]
		[InlineData("ErrorTest.sqlx", "FirstTest.rst", true)]
		public void Parse_files(string sourceFile, string targetFile, bool hasError)
		{
			Processors.DbScriptFakeExecutor executor = new Processors.DbScriptFakeExecutor();
			DbScriptsInterpreter interpreter = new DbScriptsInterpreter(executor, new Bau.Libraries.LibLogger.Core.LogManager());
			ProgramResultModelCollection results = LoadResults(targetFile);
			bool executed;

				// Ejecuta el archivo
				executed = interpreter.Execute(LoadFile(sourceFile), null);
				// Comprueba si ha habido algún error
				Assert.Equal(hasError, !executed);
				// Comprueba los resultados
				if (executed)
					CheckResults(executor.Results, results);
		}

		/// <summary>
		///		Carga los resultados esperados de un archivo
		/// </summary>
		private ProgramResultModelCollection LoadResults(string fileName)
		{
			return new Processors.DbScritsResultReader().Load(LoadFile(fileName));
		}

		/// <summary>
		///		Carga un archivo
		/// </summary>
		private string LoadFile(string fileName)
		{
			return System.IO.File.ReadAllText($"ScriptFiles/{fileName}");
		}

		/// <summary>
		///		Compara los resultados de ejecución con los datos del archivo de resultados
		/// </summary>
		private void CheckResults(ProgramResultModelCollection executorResults, ProgramResultModelCollection fileResults)
		{
			// Compara el número de resultados
			Assert.Equal(executorResults.Count, fileResults.Count);
			// Compara cada uno de los elementos
			for (int index = 0; index < executorResults.Count; index++)
				switch (executorResults[index])
				{
					case ResultSqlCommandModel command:
							CheckSqlCommand(command, fileResults[index] as ResultSqlCommandModel);
						break;
					case ResultMessageModel command:
							CheckMessageCommand(command, fileResults[index] as ResultMessageModel);
						break;
				}
		}

		/// <summary>
		///		Comprueba un comando SQL
		/// </summary>
		private void CheckSqlCommand(ResultSqlCommandModel source, ResultSqlCommandModel target)
		{
			// Comprueba el contenido normalizado (sin saltos de líneas y demás)
			Assert.Equal(Normalize(source.Sql), Normalize(target.Sql));
			// Comprueba los parámetros
			CheckParameters(source.Parameters, target.Parameters);
		}

		/// <summary>
		///		Comprueba un mensaje
		/// </summary>
		private void CheckMessageCommand(ResultMessageModel source, ResultMessageModel target)
		{
			Assert.Equal(Normalize(source.Message), Normalize(target.Message));
		}

		/// <summary>
		///		Normaliza una cadena (quita espacios dobles, saltos de línea,... que no son importantes)
		/// </summary>
		private string Normalize(string value)
		{
			// Normaliza la cadena
			if (string.IsNullOrWhiteSpace(value))
				value = string.Empty;
			else
			{
				// Quita espacios, saltos de línea, tabuladores
				value = value.Trim();
				value = value.Replace('\n', ' ');
				value = value.Replace('\r', ' ');
				value = value.Replace('\t', ' ');
				// Quita espacios dobles
				while (!string.IsNullOrEmpty(value) && value.IndexOf("  ") >= 0)
					value = value.Replace("  ", " ");
			}
			// Devuelve la cadena normalizada
			return value;
		}

		/// <summary>
		///		Comprueba los parámetros
		/// </summary>
		private void CheckParameters(Dictionary<string, string> source, Dictionary<string, string> target)
		{
			// Tiene que haber el mismo número de parámetros
			Assert.Equal(source.Count, target.Count);
			// Compara los parámetros
			foreach (KeyValuePair<string, string> sourcePair in source)
				Assert.Equal(Normalize(sourcePair.Value), Normalize(target[sourcePair.Key]));
		}
	}
}
