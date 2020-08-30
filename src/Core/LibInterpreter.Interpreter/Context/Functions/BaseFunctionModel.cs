using System;
using System.Collections.Generic;

using Bau.Libraries.LibInterpreter.Models.Symbols;

namespace Bau.Libraries.LibInterpreter.Interpreter.Context.Functions
{
	/// <summary>
	///		Clase base para las funciones
	/// </summary>
	public abstract class BaseFunctionModel
	{
		public BaseFunctionModel(SymbolModel definition, List<SymbolModel> arguments)
		{
			// Asigna la definición
			Definition.Name = definition.Name;
			Definition.Type = definition.Type;
			// Asigna los argumentos
			foreach (SymbolModel argument in arguments)
				Arguments.Add(new SymbolModel
										{
											Name = argument.Name,
											Type = argument.Type
										}
							 );
		}

		/// <summary>
		///		Nombre y tipo de la función
		/// </summary>
		public SymbolModel Definition { get; } = new SymbolModel();

		/// <summary>
		///		Argumentos de la función
		/// </summary>
		public List<SymbolModel> Arguments { get; } = new List<SymbolModel>();
	}
}
