using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibInterpreter.Interpreter.Context;
using Bau.Libraries.LibInterpreter.Interpreter.Context.Functions;
using Bau.Libraries.LibInterpreter.Interpreter.Context.Variables;
using Bau.Libraries.LibInterpreter.Models.Expressions;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Symbols;

namespace Bau.Libraries.LibInterpreter.Interpreter
{
	/// <summary>
	///		Clase para lectura y relleno de datos de un informe
	/// </summary>
	public abstract class ProgramProcessor
	{   
		public ProgramProcessor(ProcessorOptions options)
		{
			ExpressionEvaluator = new Evaluator.ExpressionCompute(this);
			Options = options;
		}

		/// <summary>
		///		Inicializa los datos de ejecución
		/// </summary>
		protected void Initialize(System.Collections.Generic.Dictionary<string, object> arguments)
		{
			// Crea el contexto inicial
			Context.Clear();
			Context.Add();
			// Añade los argumentos al contexto
			if (arguments != null)
				foreach (System.Collections.Generic.KeyValuePair<string, object> argument in arguments)
					Context.Actual.VariablesTable.Add(new VariableModel(argument.Key, argument.Value));
		}

		/// <summary>
		///		Ejecuta una serie de sentencias
		/// </summary>
		protected async Task ExecuteAsync(SentenceCollection sentences, CancellationToken cancellationToken)
		{
			foreach (SentenceBase abstractSentence in sentences)
				if (!Stopped && !cancellationToken.IsCancellationRequested)
					switch (abstractSentence)
					{
						case SentenceException sentence:
								ExecuteException(sentence);
							break;
						case SentenceDeclare sentence:
								await ExecuteDeclareAsync(sentence, cancellationToken);
							break;
						case SentenceLet sentence:
								await ExecuteLetAsync(sentence, cancellationToken);
							break;
						case SentenceFor sentence:
								await ExecuteForAsync(sentence, cancellationToken);
							break;
						case SentenceIf sentence:
								await ExecuteIfAsync(sentence, cancellationToken);
							break;
						case SentenceWhile sentence:
								await ExecuteWhileAsync(sentence, cancellationToken);
							break;
						case SentenceDo sentence:
								await ExecuteDoAsync(sentence, cancellationToken);
							break;
						case SentenceFunction sentence:
								ExecuteFunctionDeclare(sentence);
							break;
						case SentenceCallFunction sentence:
								await ExecuteFunctionCallAsync(sentence, cancellationToken);
							break;
						case SentenceReturn sentence:
								await ExecuteFunctionReturnAsync(sentence, cancellationToken);
							break;
						case SentenceComment sentence:
								ExecuteComment(sentence);
							break;
						default:
								await ExecuteAsync(abstractSentence, cancellationToken);
							break;
					}
		}

		/// <summary>
		///		Llama al procesador principal para ejecutar una sentencia desconocida
		/// </summary>
		protected abstract Task ExecuteAsync(SentenceBase abstractSentence, CancellationToken cancellationToken);

		/// <summary>
		///		Ejecuta una función implícita
		/// </summary>
		protected abstract Task<VariableModel> ExecuteAsync(ImplicitFunctionModel function, CancellationToken cancellationToken);

		/// <summary>
		///		Ejecuta una serie de sentencias creando un contexto nuevo
		/// </summary>
		protected async Task ExecuteWithContextAsync(SentenceCollection sentences, CancellationToken cancellationToken)
		{
			// Crea el contexto
			Context.Add();
			// Ejecuta las sentencias
			await ExecuteAsync(sentences, cancellationToken);
			// Elimina el contexto
			Context.Pop();
		}

		/// <summary>
		///		Ejecuta una sentencia de declaración
		/// </summary>
		private async Task ExecuteDeclareAsync(SentenceDeclare sentence, CancellationToken cancellationToken)
		{
			VariableModel variable = new VariableModel(sentence.Variable.Name, ConvertSymbolType(sentence.Variable.Type));

				// Si es un tipo conocido, añade la variable al contexto
				if (variable.Type == VariableModel.VariableType.Unknown)
					AddError($"Unknown variable type: {sentence.Variable.Name} - {sentence.Variable.Type}");
				else
				{
					// Ejecuta la expresión
					if (sentence.Expressions.Count != 0)
						variable.Value = (await ExecuteExpressionAsync(sentence.Expressions, cancellationToken)).Value;
					else
						variable.AssignDefault();
					// Si no hay errores, añade la variable a la colección
					if (!Stopped)
					{
						// Ejecuta la sentencia
						Context.Actual.VariablesTable.Add(variable);
						// Debug
						AddDebug($"Declare {sentence.Variable.Name} = " + variable.GetStringValue());
					}
				}
		}

		/// <summary>
		///		Convierte el tipo de símbolo en un tipo de variable
		/// </summary>
		private VariableModel.VariableType ConvertSymbolType(SymbolModel.SymbolType type)
		{
			switch (type)
			{
				case SymbolModel.SymbolType.Boolean:
					return VariableModel.VariableType.Boolean;
				case SymbolModel.SymbolType.Date:
					return VariableModel.VariableType.Date;
				case SymbolModel.SymbolType.Numeric:
					return VariableModel.VariableType.Numeric;
				case SymbolModel.SymbolType.String:
					return VariableModel.VariableType.String;
				default:
					return VariableModel.VariableType.Unknown;
			}
		}

		/// <summary>
		///		Ejecuta una sentencia de asignación
		/// </summary>
		private async Task ExecuteLetAsync(SentenceLet sentence, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(sentence.VariableName))
				AddError("Cant find the variable name");
			else
			{
				VariableModel variable = Context.Actual.VariablesTable.Get(sentence.VariableName);

					// Si no se ha definido la variable, añade un errorDeclara la variable si no existía
					if (variable == null)
						AddError($"Undefined variable {sentence.VariableName}");
					else
						variable.Value = (await ExecuteExpressionAsync(sentence.Expressions, cancellationToken)).Value;
			}
		}

		/// <summary>
		///		Ejecuta una sentencia for
		/// </summary>
		private async Task ExecuteForAsync(SentenceFor sentence, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(sentence.Variable.Name))
				AddError("Cant find the variable name for loop index");
			else if (sentence.StartExpression.Count == 0)
				AddError("Cant find the start expression for loop index");
			else if (sentence.EndExpression.Count == 0)
				AddError("Cant find the end expression for loop index");
			else
			{
				VariableModel start = await GetVariableValueAsync($"StartIndex_Context_{Context.Actual.ScopeIndex}", sentence.StartExpression, cancellationToken);
				VariableModel end = await GetVariableValueAsync($"EndIndex_Context_{Context.Actual.ScopeIndex}", sentence.EndExpression, cancellationToken);

					// Si se han podido evaluar las expresiones de inicio y fin
					if (!Stopped)
					{
						if (start.Type != end.Type)
							AddError("The types of start and end variable at for loop are distinct");
						else if (start.Type != VariableModel.VariableType.Numeric && start.Type != VariableModel.VariableType.Date)
							AddError("The value of start and end at for loop must be numeric or date");
						else
						{
							VariableModel step = await GetVariableValueAsync($"StepIndex_Context_{Context.Actual.ScopeIndex}", sentence.StepExpression, cancellationToken);

								// Asigna el valor a la expresión si no tenía
								if (step.Value == null)
									step.Value = 1;
								// Ejecuta el bucle for
								await ExecuteForLoopAsync(sentence, start, end, step, cancellationToken);
						}
					}
			}
		}

		/// <summary>
		///		Ejecuta el contenido de un bucle for
		/// </summary>
		private async Task ExecuteForLoopAsync(SentenceFor sentence, VariableModel start, VariableModel end, VariableModel step, CancellationToken cancellationToken)
		{
			VariableModel index = new VariableModel(sentence.Variable.Name, ConvertSymbolType(sentence.Variable.Type));
			bool isPositiveStep = step.IsGreaterThan(0);

				// Asigna el valor inicial a la variable de índice
				index.Value = start.Value;
				// Abre un nuevo contexto
				Context.Add();
				// Añade la variable al contexto
				Context.Actual.VariablesTable.Add(index);
				// Ejecuta las sentencias
				while (!IsEndForLoop(index, end, isPositiveStep) && !Stopped)
				{
					// Ejecuta las sentencias
					await ExecuteAsync(sentence.Sentences, cancellationToken);
					// Incrementa / decrementa el valor al índice (el step debería ser -x si es negativo, por tanto, siempre se suma)
					index.Sum(step);
					// y lo ajusta en el contexto
					Context.Actual.VariablesTable.Add(index);
				}
				// Elimina el contexto
				Context.Pop();
		}

		/// <summary>
		///		Comprueba si se ha terminado un bucle for
		/// </summary>
		private bool IsEndForLoop(VariableModel index, VariableModel end, bool isPositiveStep)
		{
			if (isPositiveStep)
				return index.IsGreaterThan(end);
			else
				return index.IsLessThan(end);
		}

		/// <summary>
		///		Obtiene el valor de una variable
		/// </summary>
		private async Task<VariableModel> GetVariableValueAsync(string name, ExpressionsCollection expressions, CancellationToken cancellationToken)
		{
			VariableModel variable = new VariableModel(name, VariableModel.VariableType.Unknown);

				// Asigna el valor
				if (expressions.Count > 0)
					variable.Value = await ExecuteExpressionAsync(expressions, cancellationToken);
				// Devuelve la variable
				return variable;
		}

		/// <summary>
		///		Ejecuta una sentencia de excepción
		/// </summary>
		private void ExecuteException(SentenceException sentence)
		{
			AddError(sentence.Message);
		}

		/// <summary>
		///		Ejecuta un comentario: no debería hacer nada, simplemente añade información de depuración
		/// </summary>
		private void ExecuteComment(SentenceComment sentence)
		{
			AddDebug($"Comment: {sentence.Content}");
		}

		/// <summary>
		///		Ejecuta una sentencia condicional
		/// </summary>
		private async Task ExecuteIfAsync(SentenceIf sentence, CancellationToken cancellationToken)
		{
			if (sentence.Condition.Empty)
				AddError("Cant find condition for if sentence");
			else
			{
				VariableModel result = await ExecuteExpressionAsync(sentence.Condition, cancellationToken);

					if (result != null)
					{
						if (result.Type != VariableModel.VariableType.Boolean || !(result.Value is bool resultLogical))
							AddError("If condition result is not a logical value");
						else
						{
							if (resultLogical && !sentence.SentencesThen.Empty)
								await ExecuteWithContextAsync(sentence.SentencesThen, cancellationToken);
							else if (!resultLogical && !sentence.SentencesElse.Empty)
								await ExecuteWithContextAsync(sentence.SentencesElse, cancellationToken);
						}
					}
					else
						AddError("Cant execute if condition");
			}
		}

		/// <summary>
		///		Ejecuta un bucle while
		/// </summary>
		private async Task ExecuteWhileAsync(SentenceWhile sentence, CancellationToken cancellationToken)
		{
			if (sentence.Condition.Empty)
				AddError("Cant find condition for while loop");
			else 
			{
				bool end = false;

					// Ejecuta el bucle
					while (!end && !Stopped)
					{
						VariableModel result = await ExecuteExpressionAsync(sentence.Condition, cancellationToken);

							if (result != null)
							{
								if (result.Type != VariableModel.VariableType.Boolean || !(result.Value is bool resultLogical))
									AddError("While condition result is not a logical value");
								else if (resultLogical)
									await ExecuteWithContextAsync(sentence.Sentences, cancellationToken);
								else
									end = true;
							}
					}
			}
		}

		/// <summary>
		///		Ejecuta un bucle do ... while
		/// </summary>
		private async Task ExecuteDoAsync(SentenceDo sentence, CancellationToken cancellationToken)
		{
			if (sentence.Condition.Empty)
				AddError("Cant find condition for do ... while loop");
			else
			{
				bool end = false;

					// Ejecuta el bucle
					do
					{
						VariableModel result;

							// Ejecuta las sentencias del bucle
							await ExecuteWithContextAsync(sentence.Sentences, cancellationToken);
							// Ejecuta la condición
							result = await ExecuteExpressionAsync(sentence.Condition, cancellationToken);
							// Comprueba el resultado
							if (result != null)
							{
								if (result.Type != VariableModel.VariableType.Boolean || !(result.Value is bool resultLogical))
									AddError("Do while condition result is not a logical value");
								else if (!resultLogical)
									end = true;
							}
					}
					while (!end && !Stopped);
			}
		}

		/// <summary>
		///		Ejecuta la declaración de una función: añade la función a la tabla de funciones del contexto
		/// </summary>
		private void ExecuteFunctionDeclare(SentenceFunction sentence)
		{
			if (string.IsNullOrWhiteSpace(sentence.Definition.Name))
				AddError("Cant find name for function declare");
			else
				Context.Actual.FunctionsTable.Add(sentence);
		}

		/// <summary>
		///		Ejecuta una llamada a una función
		/// </summary>
		private async Task ExecuteFunctionCallAsync(SentenceCallFunction sentence, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(sentence.Function))
				AddError("Cant find the name function for call");
			else
			{
				BaseFunctionModel function = Context.Actual.FunctionsTable.GetIfExists(sentence.Function);

					if (function == null)
						AddError($"Cant find the function to call: {sentence.Function}");
					else
						await ExecuteFunctionAsync(function, sentence.Arguments, false, cancellationToken);
			}
		}

		/// <summary>
		///		Ejecuta una función a partir de una expresión
		/// </summary>
		internal async Task<VariableModel> ExecuteFunctionAsync(ExpressionFunction expression, CancellationToken cancellationToken)
		{
			VariableModel result = null;

				// Busca la función y la ejecuta
				if (string.IsNullOrWhiteSpace(expression.Function))
					AddError("Cant find the name funcion for call");
				else
				{
					BaseFunctionModel function = Context.Actual.FunctionsTable.GetIfExists(expression.Function);

						if (function == null)
							AddError($"Cant find the function to call: {expression.Function}");
						else
							result = await ExecuteFunctionAsync(function, expression.Arguments, true, cancellationToken);
				}
				// Devuelve el resultado de la función
				return result;
		}

		/// <summary>
		///		Ejecuta una función
		/// </summary>
		private async Task<VariableModel> ExecuteFunctionAsync(BaseFunctionModel function, System.Collections.Generic.List<ExpressionsCollection> arguments, 
															   bool waitReturn, CancellationToken cancellationToken)
		{
			VariableModel result = null;

				// Crea un nuevo contexto
				Context.Add();
				// Añade los argumentos al contexto
				foreach (SymbolModel argument in function.Arguments)
					if (!Stopped)
					{
						int index = function.Arguments.IndexOf(argument);

							// Si el argumento corresponde a un parámetro, se añade al contexto esa variable con el valor
							if (arguments.Count > index)
							{
								VariableModel argumentResult = await ExecuteExpressionAsync(arguments[index], cancellationToken);

									if (argumentResult != null)
										Context.Actual.VariablesTable.Add(argument.Name, ConvertSymbolType(argument.Type), argumentResult.Value);
							}
							else
								AddError($"Cant find any call value for argument {argument.Name}");
					}
				// Si no ha habido errores al calcular los argumentos, ejecuta realmente las sentencias de la función (o llama a la función implícita)
				if (!Stopped)
				{
					// Ejecuta las sentencias de la función
					switch (function)
					{
						case ImplicitFunctionModel implicitFunction:
								result = await ExecuteAsync(implicitFunction, cancellationToken);
							break;
						case UserDefinedFunctionModel userDefinedFunction:
								// Añade el nombre que debe tener el valor de retorno
								Context.Actual.ScopeFuntionResultVariable = "Return_" + Guid.NewGuid().ToString();
								// Ejecuta la sentencia
								await ExecuteAsync(userDefinedFunction.Sentences, cancellationToken);
								// Si es una función, no una subrutina, obtiene el resultado
								if (waitReturn)
								{
									// y obtiene el resultado
									result = Context.Actual.VariablesTable.Get(Context.Actual.ScopeFuntionResultVariable);
									// Si no se ha obtenido ningún resultado (faltaba la sentencia return), añade un error
									if (result == null)
										AddError($"Cant find result for funcion {function.Definition.Name}. Check if define return sentence");
								}
							break;
					}
				}
				// Elimina el contexto
				Context.Pop();
				// Devuelve el resultado de la función
				return result;
		}

		/// <summary>
		///		Ejecuta la sentencia para devolver el resultado de una función
		/// </summary>
		private async Task ExecuteFunctionReturnAsync(SentenceReturn sentence, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(Context.Actual.ScopeFuntionResultVariable))
				AddError("Cant execute a return because there is not function block");
			else
			{
				VariableModel result = await ExecuteExpressionAsync(sentence.Expression, cancellationToken);

					// Si no hay error, añade el resultado al contexto
					if (result != null)
						Context.Actual.VariablesTable.Add(Context.Actual.ScopeFuntionResultVariable, result.Type, result.Value);
			}
		}

		/// <summary>
		///		Ejecuta una expresión
		/// </summary>
		protected async Task<VariableModel> ExecuteExpressionAsync(ExpressionsCollection expressions, CancellationToken cancellationToken)
		{
			(string error, VariableModel result) = await ExpressionEvaluator.EvaluateAsync(Context.Actual, expressions, cancellationToken);

				// Añade el error si es necesario
				if (!string.IsNullOrWhiteSpace(error))
				{
					// Añade el error
					AddError(error);
					// El resultado no es válido
					result = null;
				}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Añade un mensaje de depuración
		/// </summary>
		protected abstract void AddDebug(string message, [CallerFilePath] string fileName = null, 
										 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0);

		/// <summary>
		///		Añade un mensaje informativo
		/// </summary>
		protected abstract void AddInfo(string message, [CallerFilePath] string fileName = null, 
										[CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0);

		/// <summary>
		///		Añade una cadena a la consola
		/// </summary>
		protected abstract void AddConsoleOutput(string message, [CallerFilePath] string fileName = null, 
												 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0);

		/// <summary>
		///		Añade un error
		/// </summary>
		protected abstract void AddError(string error, Exception exception = null, [CallerFilePath] string fileName = null, 
										 [CallerMemberName] string methodName = null, [CallerLineNumber] int lineNumber = 0);

		/// <summary>
		///		Opciones de ejecución del procesador
		/// </summary>
		protected ProcessorOptions Options { get; }

		/// <summary>
		///		Contexto de ejecución
		/// </summary>
		protected ContextStackModel Context { get; } = new ContextStackModel();

		/// <summary>
		///		Indica si se ha detenido el programa por una excepción
		/// </summary>
		protected bool Stopped { get; set; }

		/// <summary>
		///		Evaluador de expresiones
		/// </summary>
		private Evaluator.ExpressionCompute ExpressionEvaluator { get; }
	}
}