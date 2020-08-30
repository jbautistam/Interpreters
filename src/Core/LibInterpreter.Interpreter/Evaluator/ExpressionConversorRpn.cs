using System;
using System.Collections.Generic;

using Bau.Libraries.LibInterpreter.Models.Expressions;

namespace Bau.Libraries.LibInterpreter.Interpreter.Evaluator
{
	/// <summary>
	///		Conversor de expresiones a notación polaca inversa
	/// </summary>
	internal class ExpressionConversorRpn
	{
		/// <summary>
		///		Convierte una colección de expresiones en una pila de expresiones en notación polaca inversa (sin paréntesis)
		/// </summary>
		internal ExpressionsCollection ConvertToRPN(ExpressionsCollection expressions)
		{
			ExpressionsCollection stackOutput = new ExpressionsCollection();
			Stack<ExpressionBase> stackOperators = new Stack<ExpressionBase>();

				// Convierte las expresiones en una pila
				foreach (ExpressionBase expressionBase in expressions)
					switch (expressionBase)
					{
						case ExpressionParenthesis expression:
								// El paréntesis izquierdo, se mete directamente en la pila de operadores
								if (expression.Open)
									stackOperators.Push(expression);
								else
								{
									bool end = false;

										// Paréntesis derecho. Saca todos los elementos del stack hasta encontrar un paréntesis izquierdo
										while (stackOperators.Count > 0 && !end)
										{
											ExpressionBase expressionOperator = stackOperators.Pop();

												if (expressionOperator is ExpressionParenthesis expressionStack)
												{
													if (!expressionStack.Open)
														end = true;
													else
														stackOutput.Add(expressionStack);
												}
												else
													stackOutput.Add(expressionOperator);
										}
								}
							break;
						case ExpressionOperatorBase expression:
								bool endOperator = false;

									// Recorre los operadores de la pila
									while (stackOperators.Count > 0 && !endOperator)
									{
										ExpressionBase lastOperator = stackOperators.Peek();

											// Si no hay ningún operador en la pila o la prioridad del operador actual
											// es mayor que la del último de la pila, se mete el último operador
											if (EndSearchOperator(expression, lastOperator))
												endOperator = true;
											else // ... si el operador tiene una prioridad menor que el último de la pila, se quita el último operador de la pila y se compara de nuevo
												stackOutput.Add(stackOperators.Pop());
									}
									// Añade el operador a la pila de operadores
									stackOperators.Push(expression);
							break;
						case ExpressionConstant expression:
								stackOutput.Add(expression);
							break;
						case ExpressionVariableIdentifier expression:
								stackOutput.Add(expression);
							break;
						case ExpressionFunction expression:
								stackOutput.Add(expression);
							break;
						default:
								stackOutput.Add(new ExpressionError("Unknown expression"));
							break;
					}
				// Añade todos los elementos que queden en el stack de operadores al stack de salida
				while (stackOperators.Count > 0)
					stackOutput.Add(stackOperators.Pop());
				// Devuelve la pila convertida a notación polaca inversa
				return stackOutput;
		}

		/// <summary>
		///		Comprueba si se ha encontrado un operador que indica el final de la operación
		/// </summary>
		private bool EndSearchOperator(ExpressionOperatorBase expression, ExpressionBase lastOperator)
		{
			if (lastOperator == null) // ... si no queda nada en la pila
				return true;
			else if (lastOperator is ExpressionParenthesis parenthesis && parenthesis.Open) // ... si es un paréntesis de apertura
				return true;
			else if (lastOperator is ExpressionOperatorBase lastExpression &&
					 expression.Priority > lastExpression.Priority) // si la prioridad es superior al último de la pila
				return true;
			else
				return false;
		}
	}
}
