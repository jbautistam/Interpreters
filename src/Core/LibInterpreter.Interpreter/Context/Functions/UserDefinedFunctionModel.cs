using System;
using System.Collections.Generic;

using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Symbols;

namespace Bau.Libraries.LibInterpreter.Interpreter.Context.Functions
{
	/// <summary>
	///		Función definida por el usuario (desde código)
	/// </summary>
	public class UserDefinedFunctionModel : BaseFunctionModel
	{
		public UserDefinedFunctionModel(SymbolModel definition, List<SymbolModel> arguments) : base(definition, arguments)
		{
		}

		/// <summary>
		///		Contenido de la función
		/// </summary>
		public SentenceCollection Sentences { get; } = new SentenceCollection();
	}
}
