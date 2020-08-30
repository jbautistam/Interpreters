using System;
using System.Collections.Generic;

using Bau.Libraries.LibInterpreter.Models.Symbols;

namespace Bau.Libraries.LibInterpreter.Interpreter.Context.Functions
{
	/// <summary>
	///		Generador para funciones implicitas
	/// </summary>
	public class ImplicitFunctionBuilder
	{
		public ImplicitFunctionBuilder(string name, SymbolModel.SymbolType type)
		{
			Name = name;
			Type = type;
		}

		/// <summary>
		///		Añade un argumento a la función
		/// </summary>
		public ImplicitFunctionBuilder WithArgument(string name, SymbolModel.SymbolType type)
		{
			// Añade el argumento
			Arguments.Add(new SymbolModel
									{
										Name = name,
										Type = type
									}
						 );
			// Devuelve el generador
			return this;
		}

		/// <summary>
		///		Genera los datos de la función implícita
		/// </summary>
		public ImplicitFunctionModel Build()
		{
			return new ImplicitFunctionModel(new SymbolModel
														{
															Name = Name,
															Type = Type
														},
											 Arguments);
		}

		/// <summary>
		///		Nombre de la función definida
		/// </summary>
		private string Name { get; }

		/// <summary>
		///		Tipo de la función definida
		/// </summary>
		private SymbolModel.SymbolType Type { get; }

		/// <summary>
		///		Argumentos
		/// </summary>
		private List<SymbolModel> Arguments { get; } = new List<SymbolModel>();
	}
}
