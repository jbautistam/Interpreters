using System;

using Bau.Libraries.LibInterpreter.Lexer;
using Bau.Libraries.LibInterpreter.Lexer.Rules;
using Bau.Libraries.LibInterpreter.Models.Tokens;
using Bau.Libraries.LibDbScripts.Interpreter.Models.SqlTokens;

namespace Bau.Libraries.LibDbScripts.Interpreter.Syntactic
{
	/// <summary>
	///		Analizador léxico
	/// </summary>
	internal class LexerProcessor
	{
		// Constantes privadas
		private const string RuleString = "String";
		private const string RuleComment = "Comment";
		private const string RuleSql = "Sql";
		private const string RuleNumber = "Number";
		private const string RuleReservedWord = "ReservedWord";
		private const string RuleVariable = "Variable";
		private const string RuleOperator = "Operator";
		private const string RuleSeparator = "Separator";
		// Variables privadas
		private int _position;

		/// <summary>
		///		Obtiene los tokens asociados a una cadena
		/// </summary>
		internal void Parse(string source)
		{
			// Limpia la lista de tokens
			TokensRead.Clear();
			// Inicializa la posición
			_position = 0;
			// y los lee
			TokensRead.AddRange(GetLexer().Parse(source));
		}

		/// <summary>
		///		Inicializa el analizador léxico
		/// </summary>
		private LexerManager GetLexer()
		{
			LexerManager manager = new LexerManager();

				// Crea las reglas
				manager.Rules.Add(new RuleDelimited(RuleString, "\"", "\"", false, false, false, false)
													{
														MustTrim = false
													}	
								); // ... cadenas
				manager.Rules.Add(new RuleDelimited(RuleComment, "/*", "*/", false, false, false, false)); // ... comentario
				manager.Rules.Add(new RuleDelimited(RuleComment, "--", string.Empty, true, false, false, false)); // ... comentario hasta final de línea
				manager.Rules.Add(new RuleDelimited(RuleSql, "<%", "%>", false, false, false, false)); // ... sentencias SQL
				manager.Rules.Add(new RulePattern(RuleNumber, "9", "9.")); // ... definición de números
				manager.Rules.Add(new RuleWordFixed(RuleReservedWord, new string[]
																			{ 
																				"if",
																				"else",
																				"declare",
																				"set",
																				"int",
																				"string",
																				"date",
																				"float",
																				"for",
																				"to",
																				"step",
																				"while",
																				"do",
																				"sub",
																				"call",
																				"return"
																			}
												   ));				
				manager.Rules.Add(new RulePattern(RuleVariable, "A", "A9_")); // ... definición de variables
				manager.Rules.Add(new RuleWordFixed(RuleSeparator, 
													new string[] { 
																   "(", ")",
																   "+", "-", "*", "/", "%",
																   "<", ">", ">=", "<=", "==", "!=",
																   "||", "&&", "!",
																   "=", ",",
																   "{", "}",
																   ";"
																 }
												   )
								 );
				// Devuelve el manager
				return manager;
		}

		/// <summary>
		///		Obtiene el siguiente token
		/// </summary>
		internal TokenSqlBase Next()
		{
			if (_position >= TokensRead.Count)
				return new TokenSqlEof(null);
			else
				return Transform(TokensRead[_position++]);
		}

		/// <summary>
		///		Obtiene el siguiente token (sin sacarlo de la pila)
		/// </summary>
		internal TokenSqlBase LookUp()
		{
			if (_position >= TokensRead.Count)
				return new TokenSqlEof(null);
			else
				return Transform(TokensRead[_position]);
		}

		/// <summary>
		///		Transforma el token
		/// </summary>
		private TokenSqlBase Transform(TokenBase token)
		{
			switch (token.Type)
			{
				case RuleString:
					return new TokenSqlString(token);
				case RuleComment:
					return new TokenSqlComment(token);
				case RuleSql:
					return new TokenSqlCommand(token);
				case RuleNumber:
					return new TokenSqlNumber(token);
				case RuleReservedWord:
					return new TokenSqlReservedWord(token);
				case RuleVariable:
					return new TokenSqlVariable(token);
				case RuleSeparator:
					return new TokenSqlSeparator(token);
				default:
					throw new ArgumentException($"Unknown token type: {token.Type}");
			}
		}

		/// <summary>
		///		Tokens interpretados
		/// </summary>
		private TokenCollection TokensRead { get; } = new TokenCollection();
	}
}
