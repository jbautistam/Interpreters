using System;

using Bau.Libraries.LibInterpreter.Models.Symbols;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibDbScripts.Interpreter.Models.SqlTokens;
using Bau.Libraries.LibDbScripts.Interpreter.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Expressions;

namespace Bau.Libraries.LibDbScripts.Interpreter.Syntactic
{
	/// <summary>
	///		Intérprete de scripts SQL
	/// </summary>
	internal class ParserProcessor
	{
		/// <summary>
		///		Interpreta un script
		/// </summary>
		internal ProgramModel Parse(string script)
		{
			// Inicializa los objetos
			Program.Sentences.Clear();
			Program.Errors.Clear();
			// Interpreta el programa
			try
			{
				// Lee los tokens
				Lexer.Parse(script);
				// Transforma la lista de tokens en sentencias
				Program.Sentences.AddRange(ParseSentences());
			}
			catch (Exception exception)
			{
				Program.Errors.Add(new ParseErrorModel(0, exception.Message));
			}
			// Devuelve el programa
			return Program;
		}

		/// <summary>
		///		Interpreta una colección de sentencias
		/// </summary>
		private SentenceCollection ParseSentences()
		{
			SentenceCollection sentences = new SentenceCollection();

				// Mientras haya algún token que leer
				do
				{
					sentences.Add(ParseSentence());
				}
				while (!HasError && !(Lexer.LookUp() is TokenSqlEof));
				// Devuelve la colección de sentencias
				return sentences;
		}

		/// <summary>
		///		Interpreta un bloque de sentencias
		/// </summary>
		private SentenceCollection ParseBlockSentences(TokenSqlSeparator.SeparatorType endBlock)
		{
			SentenceCollection sentences = new SentenceCollection();
			TokenSqlBase token = Lexer.LookUp();

				// Mientras haya algún token que leer
				while (!HasError && !(token is TokenSqlEof) && !FoundToken(token, endBlock))
				{
					// Añade las sentencias
					sentences.Add(ParseSentence());
					// Mira el siguiente token (sin sacarlo de la pila)
					token = Lexer.LookUp();
				}
				// Si el token que hemos encontrado no es el de fin de bloque, añadimos un error
				if (!FoundToken(token, endBlock))
					AddError("Error when read sentences block. Waiting end block", token);
				else // quita el token de fin de sentencia de la pila
					Lexer.Next();
				// Devuelve la colección de sentencias
				return sentences;
		}

		/// <summary>
		///		Interpreta una sentencia
		/// </summary>
		private SentenceBase ParseSentence()
		{
			SentenceBase sentence = null;
			TokenSqlBase token = Lexer.Next();

				// Interpreta la sentencia
				switch (token)
				{
					case TokenSqlComment tokenInner:
							sentence = ParseSentenceComment(tokenInner);
						break;
					case TokenSqlCommand tokenInner:
							sentence = ParseSentenceCommand(tokenInner);
						break;
					case TokenSqlReservedWord tokenInner:
							sentence = ParseSentenceReservedWord(tokenInner);
						break;
					default:
							AddError("Unknown token", token);
						break;
				}
			// Devuelve la sentencia generada
			return sentence;
		}

		/// <summary>
		///		Interpreta una sentencia de una palabra reservada
		/// </summary>
		private SentenceBase ParseSentenceReservedWord(TokenSqlReservedWord token)
		{
			SentenceBase sentence = null;

				// Obtiene la sentencia a partir del tipo de token
				switch (token.Type)
				{
					case TokenSqlReservedWord.ReservedWordType.If:
							sentence = ParseSentenceIf();
						break;
					case TokenSqlReservedWord.ReservedWordType.Declare:
							sentence = ParseSentenceDeclare();
						break;
					case TokenSqlReservedWord.ReservedWordType.Set:
							sentence = ParseSentenceSet();
						break;
					case TokenSqlReservedWord.ReservedWordType.For:
						break;
					case TokenSqlReservedWord.ReservedWordType.While:
							sentence = ParseSentenceWhile();
						break;
					case TokenSqlReservedWord.ReservedWordType.Do:
							sentence = ParseSentenceDo();
						break;
					case TokenSqlReservedWord.ReservedWordType.Sub:
							sentence = ParseSentenceSubDefinition();
						break;
					case TokenSqlReservedWord.ReservedWordType.Call:
							sentence = ParseSentenceSubCall();
						break;
					case TokenSqlReservedWord.ReservedWordType.Return:
							sentence = ParseSentenceReturn();
						break;
					default:
							AddError("This reserved word cant start a sentence", token);
						break;
				}
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Interpreta la sentencia de declaración
		/// </summary>
		/// <remarks>
		///		declare type name = expression;
		///		declare type name;
		///	</remarks>
		private SentenceDeclare ParseSentenceDeclare()
		{
			SentenceDeclare sentence = new SentenceDeclare();
			TokenSqlBase token = Lexer.Next();

				// Obtiene el tipo: type name = expression;
				if (token is TokenSqlReservedWord tokenType)
				{
					// Obtiene el tipo de variable
					sentence.Variable.Type = GetVariableType(tokenType);
					// Comprueba antes de continuar
					if (sentence.Variable.Type == SymbolModel.SymbolType.Unknown)
						AddError("Declaration type unknown", token);
					else 
					{
						// Siguiente token: name = expression;
						token = Lexer.Next();
						// Debería ser una variable
						if (token is TokenSqlVariable tokenVariable && !string.IsNullOrWhiteSpace(tokenVariable.ReadToken.Value))
						{
							// Asigna el nombre de variable
							sentence.Variable.Name = tokenVariable.ReadToken.Value;
							// Continua con el resto: = expression; o bien ;
							token = Lexer.Next();
							// Comprueba que sea un igual o un punto y coma
							if (token is TokenSqlSeparator tokenSeparator)
							{
								if (tokenSeparator.Type == TokenSqlSeparator.SeparatorType.Assign)
								{
									// Lee las expresiones
									sentence.Expressions.AddRange(ParseExpression());
									// Comprueba que lo siguiente sea un fin de sentencia
									CheckEndSentenceToken();
								}
								else if (tokenSeparator.Type != TokenSqlSeparator.SeparatorType.EndSentence)
									AddError("Waiting end of sentence (;)", token);
							}
							else
								AddError("Waiting assign operator (=) or end of command (;)", token);
						}
						else
							AddError("Waiting variable name", token);
					}
				}
				else
					AddError("Waiting declaration type", token);
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Interpreta una sentencia de asignación
		/// </summary>
		/// <remarks>
		///		set name = expression;
		///	</remarks>
		private SentenceLet ParseSentenceSet()
		{
			SentenceLet sentence = new SentenceLet();
			TokenSqlBase token = Lexer.Next();

				// Obtiene el nombre de variable name = expression;
				if (token is TokenSqlVariable tokenVariable && !string.IsNullOrWhiteSpace(tokenVariable.ReadToken.Value))
				{
					// Asigna el nombre de variable
					sentence.VariableName = tokenVariable.ReadToken.Value;
					// Continua con el resto: = expression;
					token = Lexer.Next();
					// Lee la expresión detrás del signo igual
					if (FoundToken(token, TokenSqlSeparator.SeparatorType.Assign))
					{
						// Lee la expresión
						sentence.Expressions.AddRange(ParseExpression());
						// Comprueba que lo siguiente sea un fin de sentencia
						CheckEndSentenceToken();
					}
					else
						AddError("Waiting assign operator (=)", token);
				}
				else
					AddError("Waiting variable name", token);
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Interpreta una sentencia If
		/// </summary>
		/// <remarks>
		/// if expression
		/// {
		/// }
		/// [else
		/// {
		/// }
		/// ]
		/// </remarks>
		private SentenceBase ParseSentenceIf()
		{
			SentenceIf sentence = new SentenceIf();

				// Después del if, debería ir directamente la condición
				sentence.Condition.AddRange(ParseExpression());
				// Obtiene el contenido del bloque de sentencias
				if (!HasError)
				{
					TokenSqlBase token = Lexer.Next();

						if (!FoundToken(token, TokenSqlSeparator.SeparatorType.OpenBlock))
							AddError("Waiting start block after if", token);
						else
						{
							// Obtiene el bloque de sentencias del if
							sentence.SentencesThen.AddRange(ParseBlockSentences(TokenSqlSeparator.SeparatorType.CloseBlock));
							// Si no ha habido errores, comprueba si hay un else
							if (!HasError)
							{
								// Obtiene el siguiente token
								token = Lexer.LookUp();
								// Comprueba que sea un else
								if (token is TokenSqlReservedWord tokenElse && tokenElse.Type == TokenSqlReservedWord.ReservedWordType.Else)
								{
									// Quita el token de la pila (el else)
									Lexer.Next();
									// El siguiente token debería ser un comienzo de bloque
									token = Lexer.Next();
									// Lee las sentencias del else
									if (FoundToken(token, TokenSqlSeparator.SeparatorType.OpenBlock))
										sentence.SentencesElse.AddRange(ParseBlockSentences(TokenSqlSeparator.SeparatorType.CloseBlock));
									else
										AddError("Waiting start block after else", token);
								}
							}
						}
				}
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Interpreta una sentencia While
		/// </summary>
		/// <remarks>
		///		while condition
		///		{
		///			sentences
		///		}
		/// </remarks>
		private SentenceBase ParseSentenceWhile()
		{
			SentenceWhile sentence = new SentenceWhile();

				// Después del while debería ir directamente la condición
				sentence.Condition.AddRange(ParseExpression());
				// Obtiene el contenido del bloque de sentencias
				if (!HasError)
				{
					TokenSqlBase token = Lexer.Next();

						if (!FoundToken(token, TokenSqlSeparator.SeparatorType.OpenBlock))
							AddError("Waiting start block after if", token);
						else
							sentence.Sentences.AddRange(ParseBlockSentences(TokenSqlSeparator.SeparatorType.CloseBlock));
				}
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Interpreta una sentencia do ... while
		/// </summary>
		/// <remarks>
		///		do
		///		{
		///			sentences
		///		}
		///		while condition;
		/// </remarks>
		private SentenceBase ParseSentenceDo()
		{
			SentenceDo sentence = new SentenceDo();
			TokenSqlBase token = Lexer.Next();
				
				// Después del do debería haber un comienzo de bloque
				if (FoundToken(token, TokenSqlSeparator.SeparatorType.OpenBlock))
				{
					// Lee las sentencias
					sentence.Sentences.AddRange(ParseBlockSentences(TokenSqlSeparator.SeparatorType.CloseBlock));
					// Tras las sentencias debería haber un while
					if (!HasError)
					{
						// Siguiente token
						token = Lexer.Next();
						// Si realmente es un while obtiene la condición
						if (FoundReservedWordToken(token, TokenSqlReservedWord.ReservedWordType.While))
						{
							// Interpreta la condición
							sentence.Condition.AddRange(ParseExpression());
							// Comprueba el final de sentencia
							CheckEndSentenceToken();
						}
						else
							AddError("Waiting reserved word while", token);
					}
				}
				else
					AddError("Waiting start block after do", token);
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Interpreta una sentencia de definición de una rutina o función
		/// </summary>
		/// <remarks>
		///		sub Name(type param1, type param2, type param3)
		///		{
		///			sentences
		///		}
		/// </remarks>
		private SentenceBase ParseSentenceSubDefinition()
		{
			SentenceFunction sentence = new SentenceFunction();
			TokenSqlBase token = Lexer.Next();

				// Obtiene el nombre de la función
				if (token is TokenSqlVariable && !string.IsNullOrWhiteSpace(token.ReadToken.Value))
				{
					// Asigna el nombre de rutina a la sentencia
					sentence.Definition.Name = token.ReadToken.Value;
					// Lo siguiente debería ser un paréntesis de apertura
					token = Lexer.Next();
					if (!FoundToken(token, TokenSqlSeparator.SeparatorType.OpenParenthesis))
						AddError("Cant find open parenthesis after sub name", token);
					else
					{
						bool end = false;

							// Lee los diferentes argumentos
							while (!HasError && !end)
							{
								// Lee el token
								token = Lexer.Next();
								// Trata el token
								switch (token)
								{
									case TokenSqlReservedWord tokenWord:
											SymbolModel parameter = new SymbolModel();
											
												// Asigna el tipo
												parameter.Type = GetVariableType(tokenWord);
												// Obtiene el nombre de variable
												if (parameter.Type == SymbolModel.SymbolType.Unknown)
													AddError("Parameter declaration type unkwnown", token);
												else
												{
													// Siguiente token: nombre de parámetro
													token = Lexer.Next();
													// Comprueba y asigna
													if (token is TokenSqlVariable tokenVariable && !string.IsNullOrWhiteSpace(tokenVariable.ReadToken.Value))
														parameter.Name = tokenVariable.ReadToken.Value;
													else
														AddError("Cant find the parameter name", token);
												}
												// Añade el parámetro a la colección
												sentence.Arguments.Add(parameter);
										break;
									case TokenSqlSeparator tokenSeparator:
											switch (tokenSeparator.Type)
											{
												case TokenSqlSeparator.SeparatorType.CloseParenthesis:
														end = true;
													break;
												case TokenSqlSeparator.SeparatorType.Comma:
														if (sentence.Arguments.Count == 0) // ... comprueba si antes de la coma había algún parámetro
															AddError("Found comma before any parameter", token);
														else // ... comprueba si después de la coma hay un paréntesis de cierre
														{
															TokenSqlBase next = Lexer.LookUp();

																if (FoundToken(next, TokenSqlSeparator.SeparatorType.CloseParenthesis))
																	AddError("Cant find parameter after comma at method definition", token);
														}
													break;
											}
										break;
									default:
											AddError("Unknown token when parse method parameters", token);
										break;
								}
							}
							// Lee las sentencias de la rutina
							if (!HasError)
							{
								// Una vez leído los parámetros, lo siguiente debería ser un bloque de sentencias
								token = Lexer.Next();
								// Comprueba y lee el bloque de sentencias
								if (FoundToken(token, TokenSqlSeparator.SeparatorType.OpenBlock))
									sentence.Sentences.AddRange(ParseBlockSentences(TokenSqlSeparator.SeparatorType.CloseBlock));
								else
									AddError("Cant find start block character ({) when parse method definition", token);
							}
					}
				}
				else
					AddError("Cant find the sub name", token);
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Interpreta la sentencia de llamar a un procedimiento
		/// </summary>
		/// <remarks>
		///		call Name(expression1, expression2, expression3);
		/// </remarks>
		private SentenceBase ParseSentenceSubCall()
		{
			SentenceCallFunction sentence = new SentenceCallFunction();
			TokenSqlBase token = Lexer.Next();

				// Interpreta el nombre de la funcíón
				if (token is TokenSqlVariable tokenVariable && !string.IsNullOrWhiteSpace(tokenVariable.ReadToken.Value))
				{
					// Asigna el nombre del método
					sentence.Function = tokenVariable.ReadToken.Value;
					// Coge el siguiente token
					token = Lexer.Next();
					// Debería ser un paréntesis de apertura
					if (!FoundToken(token, TokenSqlSeparator.SeparatorType.OpenParenthesis))
						AddError("Cant find open parenthesis after methods's name", token);
					else
					{
						bool end = false;
						TokenSqlBase tokenNext = Lexer.LookUp();

							// Lee los argumentos de llamada (como expresiones) separadas por coma hasta que se encuentra un paréntesis de cierre
							do
							{
								// Si es un paréntesis de cierre, es el final
								if (FoundToken(tokenNext, TokenSqlSeparator.SeparatorType.CloseParenthesis))
								{	
									// Quita el token
									Lexer.Next();
									// Indica que se ha terminado
									end = true;
								}
								else if (FoundToken(tokenNext, TokenSqlSeparator.SeparatorType.Comma))
								{
									if (sentence.Arguments.Count == 0)
										AddError("Cant find argument before comma", token);
									else // quita el token
										Lexer.Next();
								}
								else
									sentence.Arguments.Add(ParseExpression());
								// Pasa al siguiente token
								tokenNext = Lexer.LookUp();
							}
							while (!HasError && !end);
							// Interpreta el fin de sentencia
							CheckEndSentenceToken();
					}
				}
				else
					AddError("Cant find the method name", token);
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Obtiene una sentencia para devolver el valor de una función
		/// </summary>		
		private SentenceBase ParseSentenceReturn()
		{
			SentenceReturn sentence = new SentenceReturn();

				// Añade las expresiones de retorno
				sentence.Expression.AddRange(ParseExpression());
				// Comprueba que el siguiente token sea un fin de línea
				if (!HasError)
				{
					TokenSqlBase token = Lexer.Next();

						if (!FoundToken(token, TokenSqlSeparator.SeparatorType.EndSentence))
							AddError("Waiting end of sentence (;)", token);
				}
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Lee una serie de expresiones hasta encontrar uno de los separadores de final
		/// </summary>
		private ExpressionsCollection ParseExpression()
		{
			ExpressionsCollection expressions = new ExpressionsCollection();
			TokenSqlBase token = Lexer.LookUp();
			int parenthesisOpen = 0;

				// Lee las expresiones
				while (!HasError && !(token is TokenSqlEof) && FoundTokenExpression(token, parenthesisOpen))
				{
					// Obtiene el token
					token = Lexer.Next();
					// Obtiene los datos de la expresión
					switch (token)
					{
						case TokenSqlNumber tokenExpression:
								expressions.Add(ParseExpression(tokenExpression));
							break;
						case TokenSqlString tokenExpression:
								expressions.Add(ParseExpression(tokenExpression));
							break;
						case TokenSqlVariable tokenExpression:
								expressions.Add(ParseExpression(tokenExpression));
							break;
						case TokenSqlSeparator tokenExpression:
								ExpressionBase expression = ParseExpression(tokenExpression);

									// Si la expresión es un paréntesis de apertura y/o cierre incrementa / decrementa el número de paréntesis abierto
									if (expression is ExpressionParenthesis expressionParenthesis)
									{
										// Incrementa / decrementa el número de paréntesis abierto
										if (expressionParenthesis.Open)
											parenthesisOpen++;
										else
											parenthesisOpen--;
										// Si el número de paréntesis es negativo, es porque hay más paréntesis de cierre que de apertura
										if (parenthesisOpen < 0)
											AddError("Erroneus parenthesis number", token);
									}
									// Añade la expresión
									expressions.Add(expression);
							break;
						default:
								AddError("Error evaluating expression. Waiting variable, number, string, operator...", token);
							break;
					}
					// Pasa al siguiente token (sin sacarlo de la pila)
					token = Lexer.LookUp();
				}
				// Si queda algún paréntesis abierto, es un error
				if (parenthesisOpen > 0)
					AddError("Erroneus parenthesis number", token);
				// Devuelve la colección de expresiones
				return expressions;
		}

		/// <summary>
		///		Comprueba si el token corresponde a un valor para una expresión. En el caso de los paréntesis de cierre, comprueba si hay algún paréntesis abierto (ciertas
		///	sentencias como la llamada a una función, tienen un paréntesis de cierre que no está en ninguna expresión
		/// </summary>
		private bool FoundTokenExpression(TokenSqlBase token, int openParenthesisNumber)
		{
			bool found = token is TokenSqlNumber || token is TokenSqlString || token is TokenSqlVariable;

				// Si no es un token numérico, cadena o variable, comprueba si es un separador válido para una expresión
				if (!found && token is TokenSqlSeparator tokenSeparator)
				{
					if (tokenSeparator.Type == TokenSqlSeparator.SeparatorType.CloseParenthesis) 
						found = openParenthesisNumber > 0;
					else 
						found = tokenSeparator.IsOperator();
				}
				// Devuelve el valor que indica si es un token de expresión
				return found;
		}

		/// <summary>
		///		Comprueba que el siguiente token sea un final de sentencia (punto y coma)
		/// </summary>
		private void CheckEndSentenceToken()
		{
			if (!HasError)
			{
				TokenSqlBase token = Lexer.Next();

					// Debería ser un final de sentencia
					if (!FoundToken(token, TokenSqlSeparator.SeparatorType.EndSentence))
						AddError("Waiting end of sentence (;)", token);
			}
		}

		/// <summary>
		///		Interpreta una expresión para un operador lógico, aritmético o relacional
		/// </summary>
		private ExpressionBase ParseExpression(TokenSqlSeparator token)
		{
			// Obtiene la expresión adecuada al tipo de operación
			switch (token.Type)
			{
				case TokenSqlSeparator.SeparatorType.OpenParenthesis:
					return new ExpressionParenthesis(true);
				case TokenSqlSeparator.SeparatorType.CloseParenthesis:
					return new ExpressionParenthesis(false);
				case TokenSqlSeparator.SeparatorType.Sum:
					return new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Sum);
				case TokenSqlSeparator.SeparatorType.Minus:
					return new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Substract);
				case TokenSqlSeparator.SeparatorType.Multiply:
					return new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Multiply);
				case TokenSqlSeparator.SeparatorType.Divide:
					return new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Divide);
				case TokenSqlSeparator.SeparatorType.Modulus:
					return new ExpressionOperatorMath(ExpressionOperatorMath.MathType.Modulus);
				case TokenSqlSeparator.SeparatorType.Less:
					return new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.Less);
				case TokenSqlSeparator.SeparatorType.Greater:
					return new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.Greater);
				case TokenSqlSeparator.SeparatorType.GreaterOrEqual:
					return new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.GreaterOrEqual);
				case TokenSqlSeparator.SeparatorType.LessOrEqual:
					return new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.LessOrEqual);
				case TokenSqlSeparator.SeparatorType.Equal:
					return new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.Equal);
				case TokenSqlSeparator.SeparatorType.Distinct:
					return new ExpressionOperatorLogical(ExpressionOperatorLogical.LogicalType.Distinct);
				case TokenSqlSeparator.SeparatorType.Or:
					return new ExpressionOperatorRelational(ExpressionOperatorRelational.RelationalType.Or);
				case TokenSqlSeparator.SeparatorType.And:
					return new ExpressionOperatorRelational(ExpressionOperatorRelational.RelationalType.And);
				case TokenSqlSeparator.SeparatorType.Not:
					return new ExpressionOperatorRelational(ExpressionOperatorRelational.RelationalType.Not);
			}
			// Si ha llegado hasta aquí es porque ha habido algún error
			AddError($"Error evaluating expression. Unknown operator: {token.ReadToken.Value}", token);
			return null;
		}

		/// <summary>
		///		Interpreta una expresión de una variable o llamada a función
		/// </summary>
		private ExpressionBase ParseExpression(TokenSqlVariable token)
		{
			TokenSqlBase tokenNext = Lexer.LookUp();

				// Si lo siguiente es un paréntesis, tenemos una llamada a función
				if (FoundToken(tokenNext, TokenSqlSeparator.SeparatorType.OpenParenthesis))
				{
					ExpressionFunction function = new ExpressionFunction(token.ReadToken.Value);
					bool end = false;

						// Quita el token de abrir paréntesis y mira el siguiente
						Lexer.Next();
						tokenNext = Lexer.LookUp();
						// Lee los argumentos de llamada (como expresiones) separadas por coma hasta que se encuentra un paréntesis de cierre
						do
						{
							// Si es un paréntesis de cierre, es el final
							if (tokenNext is TokenSqlEof)
								AddError("Cant find close parenthesis", tokenNext);
							else if (FoundToken(tokenNext, TokenSqlSeparator.SeparatorType.CloseParenthesis))
							{	
								// Quita el token
								Lexer.Next();
								// Indica que se ha terminado
								end = true;
							}
							else if (FoundToken(tokenNext, TokenSqlSeparator.SeparatorType.Comma))
							{
								if (function.Arguments.Count == 0)
									AddError("Cant find argument before comma", token);
								else // quita el token
									Lexer.Next();
							}
							else
								function.Arguments.Add(ParseExpression());
							// Pasa al siguiente token
							tokenNext = Lexer.LookUp();
						}
						while (!HasError && !end);
						// Devuelve la expresión de la función
						return function;
				}
				else
					return new ExpressionVariableIdentifier(token.ReadToken.Value);
		}

		/// <summary>
		///		Interpreta una expresión de cadena
		/// </summary>
		private ExpressionBase ParseExpression(TokenSqlString token)
		{
			return new ExpressionConstant(SymbolModel.SymbolType.String, token.ReadToken.Value);
		}

		/// <summary>
		///		Interpreta una expresión numérica
		/// </summary>
		private ExpressionBase ParseExpression(TokenSqlNumber token)
		{
			return new ExpressionConstant(SymbolModel.SymbolType.Numeric, GetDouble(token));
		}

		/// <summary>
		///		Obtiene un valor numérico de una cadena
		/// </summary>
		private double GetDouble(TokenSqlBase token)
		{
			if (double.TryParse(token.ReadToken.Value, out double result))
				return result;
			else
			{
				// Añade el error 
				AddError($"Cant translate {token.ReadToken.Value} to numeric", token);
				// Devuelve un valor (cualquiera porque ya tenemos un error)
				return 0;
			}
		}

		/// <summary>
		///		Comprueba si un token es de tipo <see cref="TokenSqlSeparator"/> con alguno de los tipos especificados
		/// </summary>
		private bool FoundToken(TokenSqlBase token, params TokenSqlSeparator.SeparatorType[] types)
		{
			// Comprueba si es un separador de alguno de los tipos especificados
			if (token is TokenSqlSeparator tokenSeparator)
				foreach (TokenSqlSeparator.SeparatorType type in types)
					if (tokenSeparator.Type == type)
						return true;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return false;
		}

		/// <summary>
		///		Comprueba si un token es de tipo <see cref="TokenSqlReservedWord"/> con alguna de las palabras clave especificadas
		/// </summary>
		private bool FoundReservedWordToken(TokenSqlBase token, params TokenSqlReservedWord.ReservedWordType[] words)
		{
			// Comprueba si es un separador de alguno de los tipos especificados
			if (token is TokenSqlReservedWord tokenWord)
				foreach (TokenSqlReservedWord.ReservedWordType word in words)
					if (tokenWord.Type == word)
						return true;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return false;
		}

		/// <summary>
		///		Obtiene el tipo de variable
		/// </summary>
		private SymbolModel.SymbolType GetVariableType(TokenSqlReservedWord token)
		{
			switch (token.Type)
			{
				case TokenSqlReservedWord.ReservedWordType.Date:
					return SymbolModel.SymbolType.Date;
				case TokenSqlReservedWord.ReservedWordType.Int:
				case TokenSqlReservedWord.ReservedWordType.Float:
					return SymbolModel.SymbolType.Numeric;
				case TokenSqlReservedWord.ReservedWordType.String:
					return SymbolModel.SymbolType.String;
				default:
					return SymbolModel.SymbolType.Unknown;
			}
		}

		/// <summary>
		///		Interpreta un comando SQL
		/// </summary>
		private SentenceBase ParseSentenceCommand(TokenSqlCommand token)
		{
			return new SentenceSql
							{
								Command = token.ReadToken.Value
							};
		}

		/// <summary>
		///		Interpreta una sentencia de comentario
		/// </summary>
		private SentenceBase ParseSentenceComment(TokenSqlComment token)
		{
			return new SentenceComment
							{
								Content = token.ReadToken.Value
							};
		}

		/// <summary>
		///		Añade un error al programa
		/// </summary>
		private void AddError(string message, TokenSqlBase token)
		{
			if (token != null && token.ReadToken != null)
				Program.Errors.Add(new ParseErrorModel(token.ReadToken.Row, $"{message} - {token.ReadToken.Type}: {token.ReadToken.Value}"));
			else
				Program.Errors.Add(new ParseErrorModel(0, message));
		}

		/// <summary>
		///		Analizador léxico
		/// </summary>
		private LexerProcessor Lexer { get; } = new LexerProcessor();

		/// <summary>
		///		Programa
		/// </summary>
		private ProgramModel Program { get; } = new ProgramModel();

		/// <summary>
		///		Indica si ha habido algún error
		/// </summary>
		private bool HasError
		{
			get { return Program.Errors.Count > 0; }
		}
	}
}