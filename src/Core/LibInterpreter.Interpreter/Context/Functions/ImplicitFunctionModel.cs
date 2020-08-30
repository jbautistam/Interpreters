using System;
using System.Collections.Generic;

using Bau.Libraries.LibInterpreter.Models.Symbols;

namespace Bau.Libraries.LibInterpreter.Interpreter.Context.Functions
{
	/// <summary>
	///		Función implícita (definida por el intérprete) 
	/// </summary>
	public class ImplicitFunctionModel : BaseFunctionModel
	{
		public ImplicitFunctionModel(SymbolModel definition, List<SymbolModel> arguments) : base(definition, arguments)
		{
		}
	}
}
